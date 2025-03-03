using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SmartBank.Models
{
    public class CreditCard
    {
        [Key]
        public int CreditCardId { get; set; }

        public int? AssetId { get; set; }
        [ForeignKey("AssetId")]
        public Asset Asset { get; set; }

        

        [Required, MaxLength(50)]
        public string Username { get; set; }
      

        [Required, MaxLength(14)]
        public string AccountNo { get; set; }
       
        public string CardNumber { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        [Required]
        public int CVV { get; set; }

        [Required]
        public decimal CreditLimit { get; set; }

        [Required]
        public int CibilScore { get; set; }
    }
}
