using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SmartBank.Models
{
    public class DebitCard
    {
        [Key]
        public int DebitCardId { get; set; }

        public int? AssetId { get; set; }
        [ForeignKey("AssetId")]
        public Asset Asset { get; set; }

        [Required, MaxLength(16)]
        public string CardNumber { get; set; }
        [Required, MaxLength(50)]
        public string Username { get; set; }

        [Required]
        public string AccountNo { get; set; }



        [Required]
        public DateTime ExpiryDate { get; set; }

        [Required]
        public int CVV { get; set; }
    }
}
