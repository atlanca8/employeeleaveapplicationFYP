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
    public class UnitTest4 : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _webApplicationFactory;
        public UnitTest4()
        {
            var webappfactory = new WebApplicationFactory<Program>();
            _webApplicationFactory = webappfactory;
        }
        [Fact]
        public async void TestingLeaveTypeIndex()
        {
            //Arrange
            var client = _webApplicationFactory.CreateClient();
            //Act 
            var responce = await client.GetAsync("/Admin/LeaveTypes");
            int code = (int)responce.StatusCode;
            //Assert 
            Assert.Equal(200, code);
        }
        [Fact]
        public async Task AddLeaveTypes()
        {
            //Arrange
            var client = _webApplicationFactory.CreateClient();
            var newHoliday = new LeaveType
            {
                LeaveTypeName = "Test Holiday",
                Status = true,            
            };
            var content = new StringContent(JsonConvert.SerializeObject(newHoliday), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("/Admin/AddLeaveType", content);
            int statusCode = (int)response.StatusCode;

            // Assert
            Assert.Equal(200, statusCode);

        }
    }
}
