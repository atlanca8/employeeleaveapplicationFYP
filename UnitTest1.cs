using LeavePortal.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using System.Text;

namespace LeavePortalTesting
{
    public class UnitTest1 : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _webApplicationFactory;
        public UnitTest1()
        {
            var webappfactory = new WebApplicationFactory<Program>();   
            _webApplicationFactory = webappfactory; 
        }
        [Fact]
        public async void TestingDepartmentIndex()
        {
            //Arrange
            var client = _webApplicationFactory.CreateClient();
            //Act 
            var responce = await client.GetAsync("/Departments/Index");
            int code = (int)responce.StatusCode;    
            //Assert 
            Assert.Equal(200, code);    

        }
        [Fact]
        public async Task AddNewDepartment()
        {
            // Arrange
            var client = _webApplicationFactory.CreateClient();
            var newDepartment = new Department
            {
                Name = "Test Department",
                Description = "Description for Test Department"
            };
            var content = new StringContent(JsonConvert.SerializeObject(newDepartment), Encoding.UTF8, "application/json");
            // Act
            var response = await client.PostAsync("/Departments/Create", content);
            int statusCode = (int)response.StatusCode;
            // Assert
            Assert.Equal(200, statusCode); 
            
        }
    }
}