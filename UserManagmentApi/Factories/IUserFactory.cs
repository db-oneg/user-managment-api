using UserManagmentApi.Dtos;
using UserManagmentApi.Models;

namespace UserManagmentApi.Factories
{
    public interface IUserFactory
    {
        User CreateUser(CreateUserDto createUserDto);

        User UpdateUser(User existingUser, UpdateUserDto updateUserDto);
    }
}
