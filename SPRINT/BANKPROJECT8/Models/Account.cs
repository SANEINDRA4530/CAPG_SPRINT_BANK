using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BANKPROJECT8.Models
{

    public class Account
    {

        [Key]
        // [Range(1000000000, 9999999999, ErrorMessage = "Account Number must be a 10-digit number.")]

        public long AccountNumber { get; set; }  // primary key

        public DateTime? DateOfOpening { get; set; }
        public string? Status { get; }
        public string? AccountType { get; set; }
        public decimal Balance { get; set; }    // default amount should be 1000 : MIN BALANCE   ----//works only if get property is used but we cant update balance further  just with get

        // Foreign key linking Account to Customer
        [ForeignKey("CustomerId")]
        public int? CustomerId { get; set; }
        [JsonIgnore]
        public virtual Customer? Customer { get; set; } = null;

        public Account()
        {
            Status = "Active";
            Balance = (decimal)1000.10;

        }


    }
}
