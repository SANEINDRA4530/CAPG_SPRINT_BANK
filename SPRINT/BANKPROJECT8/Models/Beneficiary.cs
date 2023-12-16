using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BANKPROJECT8.Models
{

    public class Beneficiary
    {
        [Key]
        public int BeneficiaryId { get; set; } // primary key

        public long? BeneficiaryAccountNumber { get; set; } // the account number of the customer given below

        [ForeignKey("CustomerId")]
        public int? CustomerId { get; set; } // FOREIGN KEY
        [JsonIgnore]
        public virtual Customer? Customer { get; set; } = null;
    }
}
