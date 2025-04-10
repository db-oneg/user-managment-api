using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using UserManagmentApi.Controllers;
using UserManagmentApi.Dtos;
using UserManagmentApi.Factories;
using UserManagmentApi.Models;
using UserManagmentApi.Repositories;

namespace UserManagmentApi.Test.Controllers
{
    public class UsersControllerTests
    {
        private readonly IMemoryCache _cache;
        private readonly UsersController _controller;
        private readonly IUserRepository _repository;
        private readonly IUserFactory _userFactory;
        private readonly ILogger<UsersController> _logger;

        public UsersControllerTests()
        {
            _userFactory = new UserFactory();
            _repository = new InMemoryUserRepository();
         
            _cache = new MemoryCache(new MemoryCacheOptions());
            _controller = new UsersController(_repository, _userFactory, _logger, _cache);

            _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<UsersController>();           
        }

        [Fact]
        public async Task CreateUser_ReturnsCreatedAtAction_ForValidUser()
        {
            var createUserDto = new CreateUserDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@email.com",
                DateOfBirth = "2000-01-01",
                PhoneNumber = "1234567890"
            };

            var result = await _controller.CreateUser(createUserDto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var user = Assert.IsType<User>(createdResult.Value);
            Assert.Equal("John", user.FirstName);
            Assert.Equal("Doe", user.LastName);
            Assert.Equal("john.doe@email.com", user.Email);
        }

        [Fact]
        public async Task CreateUser_ReturnsBadRequest_ForUnderageUser()
        {
            var createUserDto = new CreateUserDto
            {
                FirstName = "Jhoana",
                LastName = "Doe",
                Email = "Jhoana.Doe@email.com",
                DateOfBirth = DateTime.Today.AddYears(-17).ToString("yyyy-MM-dd"),
                PhoneNumber = "1234567890"
            };

            var result = await _controller.CreateUser(createUserDto);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
    }
}