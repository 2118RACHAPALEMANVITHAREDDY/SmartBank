

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartBank.DTOs;
using SmartBank.Models;
using System.Linq;

namespace SmartBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreditCardController : ControllerBase
    {
        private readonly EFCoreDbContext _context;

        public CreditCardController(EFCoreDbContext context)
        {
            _context = context;
        }

        // GET: api/CreditCards
        [HttpGet]
        public IActionResult GetCreditCards()
        {
            var creditCards = _context.CreditCards
                .Include(c => c.Asset)
                .Select(c => new CreditCardDTO
                {
                    CreditCardId = c.CreditCardId,
                    AssetId = c.AssetId,
                    CardNumber = c.CardNumber,
                    ExpiryDate = c.ExpiryDate,
                    AccountNo = c.AccountNo,
                    Username = c.Username,
                    CibilScore = c.CibilScore// Include CIBIL score
                }).ToList();

            return Ok(creditCards);
        }

        // GET: api/CreditCards/user/{userId}
        [HttpGet("user/{userId}")]
        public IActionResult GetCreditCardsByUserId(int userId)
        {
            var creditCards = _context.CreditCards
                .Include(c => c.Asset)
                .Where(c => c.Asset.UserId == userId)
                .Select(c => new CreditCardDTO
                {
                    CreditCardId = c.CreditCardId,
                    AssetId = c.AssetId,
                    CardNumber = c.CardNumber,
                    ExpiryDate = c.ExpiryDate,
                    AccountNo = c.AccountNo,
                    Username = c.Username,
                    CibilScore = c.CibilScore// Include CIBIL score
                }).ToList();

            if (!creditCards.Any())
            {
                return NotFound();
            }

            return Ok(creditCards);
        }

        // POST: api/CreditCards
        [HttpPost]
        public IActionResult RequestCreditCard(int userId)
        {
            var existingCreditCard = _context.CreditCards
                .Where(c => c.Asset.UserId == userId && c.ExpiryDate > DateTime.Now)
                .OrderByDescending(c => c.CreditCardId) // Order by primary key in descending order
                .FirstOrDefault();

            if (existingCreditCard != null)
            {
                return BadRequest("You cannot request a new credit card until the existing one expires.");
            }

            var requestedAsset = _context.Asset
                .Where(a => a.UserId == userId && a.AssetType == "Credit Card")
                .OrderByDescending(a => a.AssetId) // Order by primary key in descending order
                .FirstOrDefault();
            var requestUser = _context.Users
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.UserId) // Order by primary key in descending order
                .FirstOrDefault();

            if (requestedAsset == null)
            {
                return BadRequest("You must request a credit card from the assets first.");
            }

            if (requestedAsset.Status != "Approved")
            {
                return BadRequest("The requested asset is not approved.");
            }

            var creditCard = new CreditCard
            {
                AssetId = requestedAsset.AssetId,
                CardNumber = GenerateCardNumber(),
                ExpiryDate = DateTime.Now.AddYears(3),
                AccountNo = requestUser.AccountNo,
                Username = requestUser.Username, // Ensure this property is set
                CibilScore = 300 // Initial CIBIL score
            };

            _context.CreditCards.Add(creditCard);
            _context.SaveChanges();

            // Delete the asset request after creating the credit card
            _context.Asset.Remove(requestedAsset);
            _context.SaveChanges();

            return Ok(new CreditCardDTO
            {
                CreditCardId = creditCard.CreditCardId,
                AssetId = creditCard.AssetId,
                CardNumber = creditCard.CardNumber,
                ExpiryDate = creditCard.ExpiryDate,
                AccountNo = creditCard.AccountNo,
                Username = creditCard.Username,
                CibilScore = creditCard.CibilScore // Include CIBIL score
            });
        }

        // POST: api/CreditCards/updateCIBIL
        [HttpPost("updateCIBIL")]
        public IActionResult UpdateCIBILScore(int creditCardId, double usageAmount)
        {
            var creditCard = _context.CreditCards.Find(creditCardId);
            if (creditCard == null)
            {
                return NotFound("Credit card not found");
            }

            // Update CIBIL score based on usage amount
            if (usageAmount > 0)
            {
                creditCard.CibilScore = Math.Min(900, creditCard.CibilScore + (int)(usageAmount / 100));
            }

            _context.CreditCards.Update(creditCard);
            _context.SaveChanges();

            return Ok(new CreditCardDTO
            {
                CreditCardId = creditCard.CreditCardId,
                AssetId = creditCard.AssetId,
                CardNumber = creditCard.CardNumber,
                ExpiryDate = creditCard.ExpiryDate,
                AccountNo = creditCard.AccountNo,
                Username = creditCard.Username,
                CibilScore = creditCard.CibilScore// Include updated CIBIL score
            });
        }

        private string GenerateCardNumber()
        {
            // Generate a unique card number
            var random = new Random();
            var cardNumber = new char[16];
            for (int i = 0; i < cardNumber.Length; i++)
            {
                cardNumber[i] = (char)('0' + random.Next(10));
            }
            return new string(cardNumber);
        }
    }
}


