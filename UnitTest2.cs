using LeavePortal.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeavePortalTesting
{
    public class UnitTest2 : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _webApplicationFactory;
        public UnitTest2()
        {
            var webappfactory = new WebApplicationFactory<Program>();
            _webApplicationFactory = webappfactory;
        }
        [Fact]
        public async void TestingHolidaysIndex()
        {
            //Arrange
            var client = _webApplicationFactory.CreateClient();
            //Act 
            var responce = await client.GetAsync("/Admin/Holidays");
            int code = (int)responce.StatusCode;
            //Assert 
            Assert.Equal(200, code);
        }
        [Fact]
        public async Task AddNewHoliday()
        {
            //Arrange
            var client = _webApplicationFactory.CreateClient();
            var newHoliday = new Holiday
            {
                Name = "Test Holiday",
                Description = "Description for Test Holiday",
                IsPublic = true,
                Date = DateTime.UtcNow
            };
            var content = new StringContent(JsonConvert.SerializeObject(newHoliday), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("/Admin/AddHoliday", content);
            int statusCode = (int)response.StatusCode;

            // Assert
            Assert.Equal(200, statusCode);

        }
    }
}
