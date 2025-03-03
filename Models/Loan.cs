using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartBank.Models
{
    public class Loan
    {
        [Key]
        public int LoanId { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public double InterestRate { get; set; }

        [Required]
        public double LoanAmount { get; set; }

        [Required]
        public int LoanTerm { get; set; } // in months

        public double MonthlyPayment { get;  set; }

        public double TotalPayment { get;  set; }

        [Required, MaxLength(14)]
        public string AccountNo { get; set; }

        public int CibilScore { get; set; }

        [Required] 
        public double Income { get; set; }

        public bool isApproved { get; set; } = false;

        public void CalculatePayments()
        {
            double monthlyInterestRate = InterestRate / 12 / 100;
            double denominator = Math.Pow(1 + monthlyInterestRate, LoanTerm) - 1;
            MonthlyPayment = LoanAmount * monthlyInterestRate * Math.Pow(1 + monthlyInterestRate, LoanTerm) / denominator;
            TotalPayment = MonthlyPayment * LoanTerm;
        }
    }
}


