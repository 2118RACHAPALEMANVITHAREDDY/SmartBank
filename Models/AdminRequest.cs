using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SmartBank.Models
{
    public class AdminRequest
    {
        [Key]
        public int RequestId { get; set; }

        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required, MaxLength(50)]
        public string RequestType { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Pending";
    }
}
