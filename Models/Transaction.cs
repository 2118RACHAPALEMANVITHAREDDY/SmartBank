//using System.ComponentModel.DataAnnotations.Schema;
//using System.ComponentModel.DataAnnotations;

//namespace SmartBank.Models
//{
//    public class Transaction
//    {
//        [Key]
//        public int TransactionId { get; set; }

//        public int? UserId { get; set; }
//        [ForeignKey("UserId")]
//        public User User { get; set; }

//        [Required, MaxLength(50)]
//        public string Username { get; set; }



//        [Required, MaxLength(50)]
//        public string TransactionType { get; set; }

//        [Required]
//        public decimal Amount { get; set; }

//        public DateTime TransactionDate { get; set; } = DateTime.Now;

//        [Required, MaxLength(50)]
//        public string ModeOfTransaction { get; set; }
//    }
//}
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SmartBank.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; }

        [Required, MaxLength(50)]
        public string TransactionType { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [Required, MaxLength(50)] 
        public string ModeOfTransaction { get; set; }

        // Method to calculate the balance
        public static decimal CheckBalance(int userId, List<Transaction> transactions)
        {
            var userTransactions = transactions.Where(t => t.UserId == userId);
            var balance = userTransactions.Sum(t => t.TransactionType == "Credit" ? t.Amount : -t.Amount);
            return balance;
        }
    }
}



