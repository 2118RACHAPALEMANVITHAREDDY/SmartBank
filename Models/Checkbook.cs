using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SmartBank.Models
{
    public class Checkbook
    {
        [Key]
        public int CheckbooksId { get; set; }

        [Required, MaxLength(14)]
        public string AccountNo { get; set; }


        [Required, MaxLength(50)]
        public string Username { get; set; }

        public int? AssetId { get; set; }
        [ForeignKey("AssetId")]
        public Asset Asset { get; set; }

        [Required, MaxLength(20)]
        public string CheckbookNumber { get; set; }

        [Required, MaxLength(50)]



        public DateTime IssueDate { get; set; }
        
    }

}

 
    

