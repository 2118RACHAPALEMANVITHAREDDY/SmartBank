using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SmartBank.Models
{
    public class Asset
    {
        [Key]
        public int AssetId { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required, MaxLength(50)]
        public string AssetType { get; set; }

        [Required]

        public string Status { get; set; } = "Not Approved";

        public DateTime CreationDate { get; set; } = DateTime.Now;


    }

}

