using System.ComponentModel.DataAnnotations;

namespace BANKPROJECT8.Models
{

    public class Employee
    {

        [Key]

        public int EmployeeId { get; set; } // primary key

        [Required]
        public string? Password { get; set; }

        [Required]
        public string? EmployeeName { get; set; }

        [Required]
        public string? EmailId { get; set; }

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone must be a 10-digit number.")]    // phone number should take 10 digits

        public long Phone { get; set; }

        [Required]
        public string? Address { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public DateTime DOJ { get; set; }

        //--constructor
        public Employee()
        {
            Status = "Active";
            DOJ = DateTime.Now;
        }
    }
}
