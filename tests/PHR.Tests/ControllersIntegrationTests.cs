using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json;
using System.Text;
using PHR.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using PHR.Api.Data;
using System.Linq;

namespace PHR.Tests
{
    public class ControllersIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ControllersIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    services.Remove(descriptor);
                    services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("TestDb"));
                });
            });
        }

        [Fact]
        public async Task Get_Patients_Unauthorized()
        {
            var client = _factory.CreateClient();
            var res = await client.GetAsync("/api/patients");
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, res.StatusCode);
        }
    }
}
