using System.Globalization;
using UserManagmentApi.Dtos;
using UserManagmentApi.Models;

namespace UserManagmentApi.Factories
{
    public class UserFactory : IUserFactory
    {
        public User CreateUser(CreateUserDto createUserDto)
        {
            if (!DateTime.TryParseExact(
                    createUserDto.DateOfBirth,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime dob))
            {
                throw new FormatException("DOB must be in the format yyyy-MM-dd.");
            }

            return new User
            {
                DateOfBirth = dob,
                Email = createUserDto.Email,
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,               
                PhoneNumber = createUserDto.PhoneNumber
            };
        }

        public User UpdateUser(User existingUser, UpdateUserDto updateUserDto)
        {
            if (!DateTime.TryParseExact(
                    updateUserDto.DateOfBirth,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime dob))
            {
                throw new FormatException("DOB must be in the format yyyy-MM-dd.");
            }

            existingUser.DateOfBirth = dob;
            existingUser.Email = updateUserDto.Email;
            existingUser.FirstName = updateUserDto.FirstName;
            existingUser.LastName = updateUserDto.LastName;            
            existingUser.PhoneNumber = updateUserDto.PhoneNumber;

            return existingUser;
        }
    }
}
