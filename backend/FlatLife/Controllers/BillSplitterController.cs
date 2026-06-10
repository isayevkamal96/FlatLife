using System.Globalization;
using System.Threading.Tasks;
using FlatLife.Database.ApplicationDbContext;
using FlatLife.Database.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using FlatLife.Models.BillSplitterDTO;


namespace FlatLife.Controllers
{
    [Route("api/BillSplitter")]
    [ApiController]
    public class BillSplitterController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly PayloadReader _payloadReader;
        private readonly ILogger _iLogger;


         public BillSplitterController (ApplicationDbContext dbContext, PayloadReader payloadReader, ILogger<BillSplitterController> logger) {
            _dbContext = dbContext;
            _payloadReader =  payloadReader;
            _iLogger = logger;
         }

         private int GetUserIdFromPayload()
        {
            var token = Request.Headers["Authorization"].ToString();
            return _payloadReader.IDReader(token);
        }


         [HttpGet("bills")]
         public async Task<IActionResult> GetAllBills() {
            var userId = GetUserIdFromPayload();

            if(userId == null) {
                return Unauthorized("User is not authorized.");
            }

            var activeFlatId = await _dbContext
                .flatUser.Where(u => u.userid == userId && u.IsActive)
                .Select(u => u.flatID)
                .FirstOrDefaultAsync();

            if(activeFlatId == null) {
                return NotFound("User has no flat.");
            }

            var bills = await _dbContext
            .bill.Where(b => b.flatId == activeFlatId).ToListAsync();
            return Ok(bills);
         }

         [HttpPost("bills")]
         public async Task<IActionResult> AddBill ([FromBody] BillRequestBody request) {
            if (request == null) {
                return BadRequest("Invalid bill data");
            }

            try {
                var userId = GetUserIdFromPayload();

                if (userId == null) {
                    return Unauthorized("User is not authorized.");
                }

                var activeFlatId = await _dbContext.flatUser
                .Where(u => u.userid == userId && u.IsActive)
                .Select(u => u.flatID)
                .FirstOrDefaultAsync();

                if(activeFlatId == null) {
                    return NotFound("User has no flat.");
                }

                var userFirstName = await _dbContext.User
                .Where(u => u.Id == userId)
                .Select(u => u.FirstName)
                .FirstOrDefaultAsync();

                var bill = new Bill
                {
                    title = request.title,
                    buyer = userFirstName, // buyer is being selected automatically
                    amount = request.amount,
                    monthOfPurchase = request.monthOfPurchase,
                    userId = GetUserIdFromPayload(),
                    flatId = activeFlatId,
                    imageBase64 = ""
                };

                _dbContext.bill.Add(bill);
                await _dbContext.SaveChangesAsync();

                return Ok("Bill created successfully.");
            }

            catch (Exception ex) {
                _iLogger.LogError(ex, "Error while adding bill.");
                return StatusCode(500, "Internal Server Error");
            }
         }

         [HttpGet("bills/{yearMonth}")]
         public async Task<IActionResult> GetBillsByMonth([FromRoute] string yearMonth) {
             var userId = GetUserIdFromPayload();

            var activeFlatId = await _dbContext
                .flatUser.Where(u => u.userid == userId && u.IsActive)
                .Select(u => u.flatID)
                .FirstOrDefaultAsync();

             if(activeFlatId == null) {
                return NotFound("User has no flat.");
            }

            var parts = yearMonth.Split('-');
            if(parts.Length != 2 || !int.TryParse(parts[0], out var year) || !int.TryParse(parts[1], out var month)) {
                return BadRequest("Invalid format. Use 'YYYY-MM'!");
            }

            if(month < 1 || month > 12) {
                return BadRequest("Invalid month. Use a value between 1 and 12.");
            }

            var bills = await _dbContext.bill
                .Where(b => b.monthOfPurchase.Year == year && b.monthOfPurchase.Month == month)
                .Where(b => b.flatId == activeFlatId)
                .ToListAsync();

            if (!bills.Any())
            {
                return NotFound("No bills found for the specified month.");
            }

            var response = bills.Select(b => new BillResponseBody
            {
                title = b.title,
                buyer = b.buyer,
                amount = b.amount,
                monthOfPurchase = b.monthOfPurchase
            }).ToList();

            return Ok(response);    
         }

         [HttpGet("calculate-debts/{yearMonth}")]
         public async Task<IActionResult> CalculateDebts([FromRoute] string yearMonth) {
            var userId = GetUserIdFromPayload();

            var activeFlatId = await _dbContext.flatUser
                .Where(u => u.userid == userId && u.IsActive)
                .Select(u => u.flatID)
                .FirstOrDefaultAsync();

            if(activeFlatId == null) {
                return NotFound("User has no flat.");
            }
           
           var parts = yearMonth.Split('-');
            if (parts.Length != 2 || !int.TryParse(parts[0], out var year) || !int.TryParse(parts[1], out var month))
            {
                return BadRequest("Invalid format. Use 'YYYY-MM'!");
            }

            if (month < 1 || month > 12)
            {
                return BadRequest("Invalid month. Use a value between 1 and 12.");
            }

            var bills = await _dbContext.bill
                .Where(b => b.monthOfPurchase.Year == year && b.monthOfPurchase.Month == month && b.flatId == activeFlatId)
                .ToListAsync();

            if (!bills.Any())
            {
                return Ok("No bills found for the specified month.");
            }

            var totalExpenses = bills.Sum(b => (decimal)b.amount);
            var uniqueBuyers = bills.Select(b => b.buyer).Distinct().ToList();
            var fairShare = totalExpenses / uniqueBuyers.Count;

            if (uniqueBuyers.Count == 0)
            {
                return BadRequest("No users found for this flat.");
            }

            var balances = uniqueBuyers.Select(buyer => new Models.BillSplitterDetails
            {
                Buyer = buyer,
                Balance = fairShare - bills.Where(b => b.buyer == buyer).Sum(b => (decimal)b.amount)
            }).ToList();

            var creditors = balances.Where(b => b.Balance < 0).ToList();
            var debtors = balances.Where(b => b.Balance > 0).ToList();

            var transactions = new List<TransactionDTO>();

            foreach (var debtor in debtors)
            {
                decimal debtorBalance = debtor.Balance;

                while (debtorBalance > 0 && creditors.Any())
                {
                    var creditor = creditors.First();
                    var creditorBalance = Math.Abs(creditor.Balance);

                    var amount = Math.Min(debtorBalance, creditorBalance);

                    transactions.Add(new TransactionDTO
                    {
                        From = debtor.Buyer,
                        To = creditor.Buyer,
                        Amount = amount
                    });

                    debtorBalance -= amount;
                    creditor.Balance += amount;

                    if (Math.Abs(creditor.Balance) < 0.01m)
                    {
                        creditors.RemoveAt(0);
                    }
                }
            }

            return Ok(new
            {
                Transactions = transactions
            });
        }


        [HttpDelete("bills")]
        public async Task<IActionResult> DeleteBill(int id) {
             var userId = GetUserIdFromPayload();

            if(userId == null) {
                return Unauthorized("User is not authorized.");
            }

            var activeFlatId = await _dbContext
                .flatUser.Where(u => u.userid == userId && u.IsActive)
                .Select(u => u.flatID)
                .FirstOrDefaultAsync();

            if(activeFlatId == null) {
                return NotFound("User has no flat.");
            }
            
            if (id == null) {
                return BadRequest("ID not found.");
            }

            var bill = await _dbContext.bill
            .Where(u => u.userId == userId && u.flatId == activeFlatId)
            .FirstOrDefaultAsync(b => b.id == id);
            
            if (bill == null)
            {
                return NotFound("Bill not found or user is not authorized to delete it.");
            }

            _dbContext.bill.Remove(bill);
            await _dbContext.SaveChangesAsync();


            return NoContent();
        }
    }
}
