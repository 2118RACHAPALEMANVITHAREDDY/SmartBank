

using Microsoft.AspNetCore.Authorization;
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
    public class AssetController : ControllerBase
    {
        private readonly EFCoreDbContext _context;
        private readonly ILogger<AssetController> _logger;

        public AssetController(EFCoreDbContext context, ILogger<AssetController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Assets
        [HttpGet]
        public IActionResult GetAssets()
        {
            var assets = _context.Asset
                .Include(a => a.User)
                .Select(a => new
                {
                    a.AssetId,
                    a.UserId,
                    a.AssetType,
                    a.CreationDate,
                    UserName = a.User.Username,
                    AccountNo = a.User.AccountNo
                }).ToList();

            return Ok(assets);
        }

        // GET: api/Assets/user/{id}
        [HttpGet("user/{id}")]
        public IActionResult GetAssetsByUserId(int id)
        {
            var assets = _context.Asset
                .Include(a => a.User)
                .Where(a => a.UserId == id)
                .Select(a => new
                {
                    a.AssetId,
                    a.UserId,
                    a.AssetType,
                    a.CreationDate,
                    UserName = a.User.Username,
                    AccountNo = a.User.AccountNo
                }).ToList();

            if (!assets.Any())
            {
                return NotFound();
            }

            return Ok(assets);
        }

        // GET: api/Assets/NotApproved
        [HttpGet("NotApproved")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetNotApprovedAssets()
        {
            var notApprovedAssets = _context.Asset
                .Include(a => a.User)
                .Where(a => a.Status == "Not Approved")
                .Select(a => new
                {
                    a.AssetId,
                    a.UserId,
                    a.AssetType,
                    a.CreationDate,
                    a.Status,
                    UserName = a.User.Username,
                    AccountNo = a.User.AccountNo
                }).ToList();

            return Ok(notApprovedAssets);
        }

        // POST: api/Assets/Approve/{id}
        [HttpPost("Approve/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult ApproveAsset(int id)
        {
            var asset = _context.Asset.Find(id);
            if (asset == null)
            {
                return NotFound("Asset not found.");
            }

            if (asset.Status == "Approved")
            {
                return BadRequest("Asset is already approved.");
            }

            asset.Status = "Approved";
            _context.SaveChanges();

            return Ok("Asset approved successfully.");
        }

        // POST: api/Assets/RequestAsset
        [HttpPost("RequestAsset")]
        [Authorize(Roles ="User")]
        public IActionResult RequestAsset(int userId, string requestAsset)
        {
            _logger.LogInformation($"RequestAsset called with userId: {userId}, requestAsset: {requestAsset}");

            var user = _context.Users
                .Where(u => u.UserId == userId)
                .Select(u => new UserDTO
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    AccountNo = u.AccountNo
                })
                .FirstOrDefault();

            if (user == null)
            {
                return NotFound("Invalid account number.");
            }

            if (requestAsset.Equals("Checkbook", StringComparison.OrdinalIgnoreCase))
            {
                return CheckCheckbookRequest(userId);
            }
            else if (requestAsset.Equals("Credit Card", StringComparison.OrdinalIgnoreCase))
            {
                return CheckCreditCardRequest(userId);
            }
            else if (requestAsset.Equals("Debit Card", StringComparison.OrdinalIgnoreCase))
            {
                return CheckDebitCardRequest(userId);
            }
            else
            {
                return BadRequest("Invalid request asset type. Please provide 'Checkbook', 'Credit Card', or 'Debit Card'.");
            }
        }

        private IActionResult CheckCheckbookRequest(int userId)
        {
            var existingCheckbook = _context.Checkbooks
                .Include(c => c.Asset)
                .Where(c => c.Asset.UserId == userId)
                .FirstOrDefault();

            if (existingCheckbook != null)
            {
                return BadRequest("You already have a checkbook.");
            }
            var asset = new Asset
            {
                UserId = userId,
                AssetType = "Checkbook",
                CreationDate = DateTime.Now,
                Status = "Not Approved"
            };
            _context.Asset.Add(asset);
            _context.SaveChanges();
            return Ok("Checkbook requested successfully.");
        }

        private IActionResult CheckCreditCardRequest(int userId)
        {
            var existingCreditCard = _context.CreditCards
                .Where(c => c.Asset.UserId == userId && c.ExpiryDate > DateTime.Now)
                .FirstOrDefault();

            if (existingCreditCard != null)
            {
                return BadRequest($"You cannot request a new credit card until the existing one expires on {existingCreditCard.ExpiryDate.ToShortDateString()}.");
            }
            var asset = new Asset
            {
                UserId = userId,
                AssetType = "Credit Card",
                CreationDate = DateTime.Now,
                Status = "Not Approved"
            };
            _context.Asset.Add(asset);
            _context.SaveChanges();
            return Ok("Credit card requested successfully.");
        }

        private IActionResult CheckDebitCardRequest(int userId)
        {
            var existingDebitCard = _context.DebitCards
                .Where(d => d.Asset.UserId == userId && d.ExpiryDate > DateTime.Now)
                .FirstOrDefault();

            if (existingDebitCard != null)
            {
                return BadRequest($"You cannot request a new debit card until the existing one expires on {existingDebitCard.ExpiryDate.ToShortDateString()}.");
            }
            var asset = new Asset
            {
                UserId = userId,
                AssetType = "Debit Card",
                CreationDate = DateTime.Now,
                Status = "Not Approved"
            };
            _context.Asset.Add(asset);
            _context.SaveChanges();
            return Ok("Debit card requested successfully.");
        }
    }
}
