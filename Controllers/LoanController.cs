
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using SmartBank.DTOs;
using SmartBank.Models;
using System.Linq;

namespace SmartBank.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
    public class LoanController : ControllerBase
    {
        private readonly EFCoreDbContext _context;

        public LoanController(EFCoreDbContext context)
        {
            _context = context;
        }

        [HttpPost("apply")]
        public async Task<IActionResult> ApplyForLoan([FromBody] LoanDTO loanDto)
        {
            if (loanDto == null)
            {
                return BadRequest("loanDto is required.");
            }

            var user = await _context.Users.FindAsync(loanDto.UserId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Define loan eligibility criteria and interest rates based on CIBIL score and income
            double interestRate;
            if (loanDto.CibilScore >= 750)
            {
                interestRate = 8.5; // Example interest rate for high CIBIL score
            }
            else if (loanDto.CibilScore > 600)
            {
                interestRate = 10.5; // Example interest rate for medium CIBIL score
            }
            else
            {
                return BadRequest("Loan cannot be approved. CIBIL score must be greater than 600.");
            }

            var loan = new Loan
            {
                UserId = user.UserId,
                InterestRate = interestRate,
                LoanAmount = loanDto.LoanAmount,
                LoanTerm = loanDto.LoanTerm,
                AccountNo = user.AccountNo,
                CibilScore = loanDto.CibilScore,
                Income = loanDto.Income
            };

            loan.CalculatePayments();

            _context.Loan.Add(loan);
            await _context.SaveChangesAsync();

            var response = new
            {
                loan.LoanId,
                loan.UserId,
                loan.InterestRate,
                loan.LoanAmount,
                loan.LoanTerm,
                loan.MonthlyPayment,
                loan.TotalPayment,
                loan.AccountNo,
                loan.CibilScore,
                loan.Income,
                loan.isApproved
            };

            return Ok($"Loan Request is submitted Successfully with Loan ID : {loan.LoanId}");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("ApproveLoan/{id}")]
        public IActionResult ApproveLoan(int id)
        {
            var loan = _context.Loan.Find(id);
            if (loan == null)
            {
                return NotFound("Loan not found.");
            }

            if (loan.isApproved)
            {
                return BadRequest("Loan is already approved.");
            }
            var user = _context.Users.Find(loan.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            loan.isApproved = true;
            user.Balance += (decimal)loan.LoanAmount; // Credit the loan amount to the user's account balance

            var trans = new Transaction
            {
                UserId = user.UserId,
                Username = "SMARTBANK",
                TransactionType = "Credit",
                Amount = (decimal)loan.LoanAmount,
                TransactionDate = DateTime.Now,
                ModeOfTransaction = "BULK POSTING:LOAN"
            };
            _context.Transactions.Add(trans);
            _context.Loan.Update(loan);
            _context.Users.Update(user);
            _context.SaveChanges();

            return Ok("Loan approved successfully and amount credited to the user's account.");
        }
    }
 }

