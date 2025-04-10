using System.Collections.Concurrent;
using UserManagmentApi.Models;

namespace UserManagmentApi.Repositories
{
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly ConcurrentDictionary<Guid, User> _users = new ConcurrentDictionary<Guid, User>();
        public Task<User> CreateAsync(User user)
        {
            user.Id = Guid.NewGuid();
            _users[user.Id] = user;
            return Task.FromResult(user);
        }
        public Task<bool> DeleteAsync(Guid id)
        {
            return Task.FromResult(_users.TryRemove(id, out _));
        }
        public Task<IEnumerable<User>> GetAllAsync()
        {
            return Task.FromResult(_users.Values.AsEnumerable());
        }
        public Task<User> GetByIdAsync(Guid id)
        {
            _users.TryGetValue(id, out var user);
            return Task.FromResult(user);
        }
        public Task<bool> UpdateAsync(User user)
        {
            if (!_users.ContainsKey(user.Id))
                return Task.FromResult(false);
            _users[user.Id] = user;
            return Task.FromResult(true);
        }
        public Task<bool> EmailExistsAsync(string email, Guid? excludeUserId = null)
        {
            var exists = _users.Values.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)
                                                    && (!excludeUserId.HasValue || u.Id != excludeUserId.Value));
            return Task.FromResult(exists);
        }
    }
}
