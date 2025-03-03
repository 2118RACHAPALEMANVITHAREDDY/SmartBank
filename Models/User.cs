using System.ComponentModel.DataAnnotations;
using System.Transactions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using SmartBank.Models;

namespace SmartBank.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; }

        [Required, MaxLength(100)]
        public string Email { get; set; }

        [Required, MaxLength(255)]
        public string Password { get; set; }

        [Required, MaxLength(12)]
        public string Adharcard { get; set; }

        [Required, MaxLength(10)]
        public string Pancard { get; set; }

        [Required, MaxLength(15)]
        public string PhoneNo { get; set; }

        [Required, MaxLength(14)]
        public string AccountNo { get; set; }

        [Required]
        public string Role { get; set; } = "User";

        [Required]
        public decimal Balance { get; set; } = 0;

        
       //public ICollection<Payment> Payments { get; set; }
        public ICollection<Asset> Assets { get; set; }
        public ICollection<AdminRequest> AdminRequests { get; set; }
    }



}
//using System.ComponentModel.DataAnnotations.Schema;
//using System.ComponentModel.DataAnnotations;

//namespace SmartBank.Models
//{
//    public class Payment
//    {
//        [Key]
//        public int PaymentId { get; set; }

//        public int? UserId { get; set; }
//        [ForeignKey("UserId")]
//        public User User { get; set; }

//        [Required, MaxLength(50)]
//        public string PaymentType { get; set; }

//        [Required]
//        public decimal Amount { get; set; }
//        [Required, MaxLength(20)]
//        public string AccountNo { get; set; }

//        [Required, MaxLength(50)]
//        public string Username { get; set; }

//        public DateTime PaymentDate { get; set; } = DateTime.Now;
//    }
//}

