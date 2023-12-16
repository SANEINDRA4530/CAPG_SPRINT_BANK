using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BANKPROJECT8.Models
{

    public class Transaction
    {

        [Key]
        // [Range(1000000, 9999999, ErrorMessage = "Transaction ID must be a 7-digit number.")]

        public long TransactionId { get; set; } // primary key

        public long? SourceAccount { get; set; }
        public long? DestinationAccount { get; set; }
        public DateTime? Date { get; set; }
        public decimal? Amount { get; set; }
        public string? TransactionType { get; set; }
        public string? Description { get; set; }

        // Foreign key linking Transaction to Customer
        [ForeignKey("By")]
        public int? By { get; set; } // foreign key
        [JsonIgnore]
        public virtual Customer? ByNavigation { get; set; } = null;




        public Transaction() { Date = DateTime.Now; }
    }
}
