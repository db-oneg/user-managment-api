using System.ComponentModel.DataAnnotations;

namespace UserManagmentApi.Dtos
{
    public class UserDtoBase
    {
        [Required]
        [MaxLength(128)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(128)]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [RegularExpression(@"^(19|20)\d\d-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01])$", ErrorMessage = "Date of birth must be in the format yyyy-MM-dd.")]
        public string DateOfBirth { get; set; }
        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must have ten digits.")]
        public string PhoneNumber { get; set; }
    }
}
