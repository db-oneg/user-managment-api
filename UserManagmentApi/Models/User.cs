using System.ComponentModel.DataAnnotations;

namespace UserManagmentApi.Models
{
    public class User
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(128)]
        public string LastName { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage = "Invalid email.")]
        public string Email { get; set; }

        [RegularExpression(@"^(19|20)\d\d-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01])$", ErrorMessage = "DOB must be in the format yyyy-MM-dd.")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone-number must have ten digits.")]
        public string PhoneNumber { get; set; }

        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Year;
                if (DateOfBirth.Date > today.AddYears(-age))
                {
                    age--;
                }
                return age;
            }
        }

        public User()
        {
            Email = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;            
            PhoneNumber = string.Empty;
        }
    }
}
