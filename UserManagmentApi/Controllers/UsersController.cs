using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using UserManagmentApi.Dtos;
using UserManagmentApi.Factories;
using UserManagmentApi.Helpers;
using UserManagmentApi.Models;
using UserManagmentApi.Repositories;

namespace UserManagmentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private const string UsersCacheKey = "users_all";

        private readonly IMemoryCache _cache;
        private readonly IUserFactory _userFactory;
        private readonly IUserRepository _repository;
        private readonly ILogger<UsersController> _logger;       

        public UsersController(IUserRepository repository, IUserFactory userFactory, ILogger<UsersController> logger, IMemoryCache cache)
        {
            _cache = cache;
            _logger = logger;
            _repository = repository;
            _userFactory = userFactory;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            try
            {
                if (!_cache.TryGetValue(UsersCacheKey,
                                        out IEnumerable<User> cachedUsers))
                {
                    cachedUsers = await _repository.GetAllAsync();

                    var cacheEntryOptions 
                        = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(1));

                    _cache.Set(UsersCacheKey, cachedUsers, cacheEntryOptions);
                }
                return Ok(cachedUsers);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting users.");

                return StatusCode(500);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            try
            {
                string userCacheKey = $"user_{id}";

                if (!_cache.TryGetValue(userCacheKey,
                                        out User cachedUser))
                {
                    cachedUser = await _repository.GetByIdAsync(id);

                    if (cachedUser == null)
                        return NotFound();

                    var cacheEntryOptions 
                        = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(1));

                    _cache.Set(userCacheKey, cachedUser, cacheEntryOptions);
                }
                return Ok(cachedUser);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting user with id {UserId}.", id);

                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = _userFactory.CreateUser(createUserDto);

                if (UserHelper.CalculateAge(user.DateOfBirth) < 18)
                    return BadRequest("User must be at least 18 years old.");

                if (await _repository.EmailExistsAsync(createUserDto.Email))
                    return BadRequest("Email already exists.");

                var createdUser = await _repository.CreateAsync(user);

                _cache.Remove(UsersCacheKey);
                _cache.Remove($"user_{createdUser.Id}");

                return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating user.");

                return StatusCode(500);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var existingUser = await _repository.GetByIdAsync(id);
                if (existingUser == null)
                    return NotFound();

                if (await _repository.EmailExistsAsync(updateUserDto.Email, id))
                    return BadRequest("Email already exists.");

                _userFactory.UpdateUser(existingUser, updateUserDto);

                if (UserHelper.CalculateAge(existingUser.DateOfBirth) < 18)
                    return BadRequest("User must be at least 18 years old.");

                var updated = await _repository.UpdateAsync(existingUser);
                if (!updated)
                    return StatusCode(500, "Update failed.");

                _cache.Remove(UsersCacheKey);
                _cache.Remove($"user_{id}");

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating user with id {UserId}.", id);

                return StatusCode(500);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(Guid id)
        {
            try
            {
                var user = await _repository.GetByIdAsync(id);

                if (user == null)
                    return NotFound();

                var deleted = await _repository.DeleteAsync(id);
                if (!deleted)
                    return StatusCode(500, "Deletion failed.");

                _cache.Remove(UsersCacheKey);
                _cache.Remove($"user_{id}");

                return NoContent();
            }
            catch (Exception E)
            {
                _logger.LogError(E, "Error deleting user with id {UserId}.", id);
                return StatusCode(500);
            }
        }
    }
}
