using System.ComponentModel.DataAnnotations;

namespace BANKPROJECT8.Models
{
    public class Customer
    {

        [Key]
        public int CustomerId { get; set; }

        [Required]
        public string? Password { get; set; }

        [Required]
        public string? CustomerName { get; set; }

        [StringLength(50)]
        public string? EmailId { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone must be a 10-digit number.")]    // phone number should take 10 digits

        public long? Phone { get; set; }

        [StringLength(50)]
        public string? Address { get; set; }

        public string? Status { get; set; }     // default ---get

        public DateTime? DOJ { get; set; }  // default ---get


        // DEFAULT DUMMY PASSWORD FOR NEW CUSTOMRER
        public Customer()
        {
            Password = "9999";
            DOJ = DateTime.Now;
            Status = "Active";

        }



    }
}
