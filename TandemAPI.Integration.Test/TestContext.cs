using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using TandemAPI.Models;
using TandemAPI.ViewModels;
using Xunit;

namespace TandemAPI.Integration.Test
{
    public class TestContext
    {
        private TestServer server;
        public HttpClient client { get; private set; }

        public TestContext()
        {
            payload = File.ReadAllText("payload.json");
            SetupClient();
        }

        string payload;

        [Fact]
        public async Task ShouldAddPerson()
        {
            var httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/person", httpContent);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task ShouldReturnPersonByEmail()
        {
            //Act
            var response = await client.GetAsync("api/person/firstName@awesomedomain.com");

            var result = await response.Content.ReadAsStringAsync();

            var personsViewModel = JsonConvert.DeserializeObject <List<PersonViewModel>> (result);

            //Asset
            Assert.NotEmpty(personsViewModel);
        }

        [Fact]
        public async Task ShouldNotReturnPersonByEmail()
        {
            //Act
            var response = await client.GetAsync("api/person/notExiste@awesomedomain.com");

            //Asset
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        private void SetupClient()
        {
            var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .Build();
            var webhostBuilder = new WebHostBuilder();
            webhostBuilder.UseConfiguration(config);
            
       
            server = new TestServer(webhostBuilder.UseStartup<Startup>());

            client = server.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        }
    }
}
