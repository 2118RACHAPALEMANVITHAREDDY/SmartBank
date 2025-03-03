

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
    public class DebitCardController : ControllerBase
    {
        private readonly EFCoreDbContext _context;

        public DebitCardController(EFCoreDbContext context)
        {
            _context = context;
        }

        //GET: api/DebitCards
        [HttpGet]
        public IActionResult GetDebitCards()
        {
            var debitCards = _context.DebitCards
            .Include(d => d.Asset)
            .Select(d => new DebitCardDTO
            {
                DebitCardId = d.DebitCardId,
                AssetId = d.AssetId,
                CardNumber = d.CardNumber,
                ExpiryDate = d.ExpiryDate,
                AccountNo = d.AccountNo,
                Username = d.Username

            }).ToList();

            return Ok(debitCards);
        }

        // GET: api/DebitCards/user/{userId}
        [HttpGet("user/{userId}")]
        public IActionResult GetDebitCardsByUserId(int userId)
        {
                var debitCards = _context.DebitCards
                .Include(d => d.Asset)
                .Where(d => d.Asset.UserId == userId)
                .Select(d => new DebitCardDTO
                {
                    DebitCardId = d.DebitCardId,
                    AssetId = d.AssetId,
                    CardNumber = d.CardNumber,
                    ExpiryDate = d.ExpiryDate,
                    AccountNo = d.AccountNo,
                    Username = d.Username

                }).ToList();

            if (!debitCards.Any())
            {
                return NotFound();
            }

            return Ok(debitCards);
        }

        // POST: api/DebitCards
        [HttpPost]
        public IActionResult RequestDebitCard(int userId)
        {
            var existingDebitCard = _context.DebitCards
                .Where(c => c.Asset.UserId == userId && c.ExpiryDate > DateTime.Now)
                .OrderByDescending(c => c.DebitCardId) // Order by primary key in descending order
                .FirstOrDefault();

            if (existingDebitCard != null)
            {
                return BadRequest("You cannot request a new debit card until the existing one expires.");
            }

            var requestedAsset = _context.Asset
                .Where(a => a.UserId == userId && a.AssetType == "Debit Card")
                .OrderByDescending(a => a.AssetId) // Order by primary key in descending order
                .FirstOrDefault();
            var requestUser = _context.Users
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.UserId) // Order by primary key in descending order
                .FirstOrDefault();

            if (requestedAsset == null)
            {
                return BadRequest("You must request a debit card from the assets first.");
            }

            if (requestedAsset.Status != "Approved")
            {
                return BadRequest("The requested asset is not approved.");
            }

            var debitCard = new DebitCard
            {
                AssetId = requestedAsset.AssetId,
                CardNumber = GenerateCardNumber(),
                ExpiryDate = DateTime.Now.AddYears(3),
                AccountNo = requestUser.AccountNo,
                Username = requestUser.Username // Ensure this property is set
            };

            _context.DebitCards.Add(debitCard);
            _context.SaveChanges();

            // Delete the asset request after creating the debit card
            _context.Asset.Remove(requestedAsset);
            _context.SaveChanges();

            return Ok(new DebitCardDTO
            {
                DebitCardId = debitCard.DebitCardId,
                AssetId = debitCard.AssetId,
                CardNumber = debitCard.CardNumber,
                ExpiryDate = debitCard.ExpiryDate,
                AccountNo = debitCard.AccountNo,
                Username = debitCard.Username
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