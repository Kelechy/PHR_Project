using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PHR.Api.Data;
using PHR.Api.DTOs;
using PHR.Api.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PHR.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _cfg;
        public AuthService(AppDbContext db, IConfiguration cfg)
        {
            _db = db;
            _cfg = cfg;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
                throw new ApplicationException("Email already exists");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RoleId = (await _db.Roles.FirstAsync()).Id
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var token = await GenerateTokenAsync(user);
            var refresh = await CreateRefreshToken(user.Id);
            return new AuthResponseDto(token, refresh);
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return null;

            var token = await GenerateTokenAsync(user);
            var refresh = await CreateRefreshToken(user.Id);
            return new AuthResponseDto(token, refresh);
        }

        public async Task<AuthResponseDto?> RefreshTokenAsync(string token)
        {
            var rt = await _db.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token && !r.Revoked && r.Expires > DateTime.UtcNow);
            if (rt == null) return null;
            var user = await _db.Users.FindAsync(rt.UserId);
            if (user == null) return null;
            var newToken = await GenerateTokenAsync(user);
            var newRefresh = await CreateRefreshToken(user.Id);
            rt.Revoked = true;
            await _db.SaveChangesAsync();
            return new AuthResponseDto(newToken, newRefresh);
        }

        private async Task<string> GenerateTokenAsync(User user)
        {
            var dbUser = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == user.Id);
            var roleId = dbUser?.RoleId ?? user.RoleId;
            var permissionNames = await (from rp in _db.RolePermissions
                                         join p in _db.Permissions on rp.PermissionId equals p.Id
                                         where rp.RoleId == roleId
                                         select p.Name).ToListAsync();

            var key = Encoding.UTF8.GetBytes(_cfg["Jwt:Key"] ?? "");
            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, dbUser?.Role?.Name ?? "")
            };

            foreach (var perm in permissionNames)
            {
                claims.Add(new Claim("permission", perm));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_cfg["Jwt:ExpiryMinutes"] ?? "60")),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private async Task<string> CreateRefreshToken(Guid userId)
        {
            var rt = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                Expires = DateTime.UtcNow.AddDays(7)
            };
            _db.RefreshTokens.Add(rt);
            await _db.SaveChangesAsync();
            return rt.Token;
        }
    }
}
