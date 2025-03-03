using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartBank.DTOs;
using SmartBank.Models;
using System.Collections.Generic;
using System.Linq;

namespace SmartBank.Controllers
{
    [Route("api/[controller]")]//define rotuing for the controller
    [ApiController]//
    public class TransactionsController : ControllerBase
    {
        private readonly EFCoreDbContext _context;
        private const decimal MinimumBalance = 500m;
        private const decimal PenaltyAmount = 50m;

        public TransactionsController(EFCoreDbContext context)
        {
            _context = context;
        }

        // POST: api/Transactions/TransferMoney
        [HttpPost("TransferMoney")]
        public IActionResult TransferMoney([FromBody] TransferRequestDTO transferRequest)
        {
            if (transferRequest == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Fetch the sender and receiver from the user table
            var sender = _context.Users.Find(transferRequest.SenderUserId);
            var receiver = _context.Users.Find(transferRequest.ReceiverUserId);

            if (sender == null || receiver == null)
            {
                return NotFound("Sender or receiver not found.");
            }

            // Check if the sender has sufficient balance
            if (sender.Balance < transferRequest.Amount)
            {
                return BadRequest("Insufficient balance.");
            }

            // Deduct amount from sender's balance
            sender.Balance -= transferRequest.Amount;

            // Add amount to receiver's balance
            receiver.Balance += transferRequest.Amount;

            //Create transaction records
           var senderTransaction = new Transaction
           {
               UserId = transferRequest.SenderUserId,
               Username = receiver.Username,
               TransactionType = "Debit",
               Amount = transferRequest.Amount,
               TransactionDate = DateTime.Now,
               ModeOfTransaction = transferRequest.ModeOfTransaction
           };
            _context.Transactions.Add(senderTransaction);

            var receiverTransaction = new Transaction
            {
                UserId = transferRequest.ReceiverUserId,
                Username = sender.Username,
                TransactionType = "Credit",
                Amount = transferRequest.Amount,
                TransactionDate = DateTime.Now,
                ModeOfTransaction = transferRequest.ModeOfTransaction
            };
            _context.Transactions.Add(receiverTransaction);

            //Save changes to the database
            _context.SaveChanges();

            // Check if sender's balance is below the minimum balance and apply penalty if necessary
            if (sender.Balance < MinimumBalance)
            {
                sender.Balance -= PenaltyAmount;
                var penaltyTransaction = new Transaction
                {
                    UserId = transferRequest.SenderUserId,
                   // Username = transferRequest.SenderUsername,
                    TransactionType = "Debit",
                    Amount = PenaltyAmount,
                    TransactionDate = DateTime.Now,
                    ModeOfTransaction = "Penalty"
                };
                _context.Transactions.Add(penaltyTransaction);
                _context.SaveChanges();
            }

            return Ok(new { senderTransaction, receiverTransaction });
        }

        // POST: api/Transactions/DepositMoney
        [HttpPost("DepositMoney")]
        public IActionResult DepositMoney([FromBody] DepositRequestDTO depositRequest)
        {
            if (depositRequest == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Fetch the user from the user table
            var user = _context.Users.Find(depositRequest.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Add amount to user's balance
            user.Balance += depositRequest.Amount;

            // Create transaction record
            var depositTransaction = new Transaction
            {
                UserId = depositRequest.UserId,
                Username = "Self" ,
                TransactionType = "Credit",
                Amount = depositRequest.Amount,
                TransactionDate = DateTime.Now,
                ModeOfTransaction = "Deposit"
            };
            _context.Transactions.Add(depositTransaction);

            // Save changes to the database
            _context.SaveChanges();

            return Ok(new TransactionDTO
            {
                TransactionId = depositTransaction.TransactionId,
                UserId = depositTransaction.UserId,
                //Username = depositTransaction.Username,
                TransactionType = depositTransaction.TransactionType,
                Amount = depositTransaction.Amount,
                TransactionDate = depositTransaction.TransactionDate,
                ModeOfTransaction = depositTransaction.ModeOfTransaction
            });
        }

        // GET: api/Transactions/CheckBalance/{userId}
        [HttpGet("CheckBalance/{userId}")]
        public IActionResult CheckBalance(int userId)
        {
            // Fetch the user's balance from the user table
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var currentBalance = user.Balance;

            return Ok(currentBalance);
        }
    }

    public class TransferRequestDTO
    {
        public int SenderUserId { get; set; }
        public int ReceiverUserId { get; set; }
        public decimal Amount { get; set; }
        public string ModeOfTransaction { get; set; }
    }

    public class DepositRequestDTO
    {
        public int UserId { get; set; }
        //public string Username { get; set; }
        public decimal Amount { get; set; }
    }
}