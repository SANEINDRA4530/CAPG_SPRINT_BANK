using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BANKPROJECT8.Models;
using System.Globalization;

namespace BANKPROJECT8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly BankDBC _context;

        public TransactionsController(BankDBC context)
        {
            _context = context;
        }

        // GET: api/Transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
          if (_context.Transactions == null)
          {
              return NotFound();
          }
            return await _context.Transactions.ToListAsync();
        }

        // GET: api/Transactions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(long id)
        {
          if (_context.Transactions == null)
          {
              return NotFound();
          }
            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null)
            {
                return NotFound();
            }

            return transaction;
        }

        // PUT: api/Transactions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransaction(long id, Transaction transaction)
        {
            if (id != transaction.TransactionId)
            {
                return BadRequest("Transaction id not Found");
            }

            _context.Entry(transaction).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        //NO DUPLICATE POST

        // POST: api/Transactions
        [HttpPost]
        public async Task<ActionResult<Transaction>> PostTransaction(Transaction transaction)
        {
            /*     if (_context.Transactions == null)
                 {
                     return Problem("Entity set 'BDBContext.Transactions'  is null.");
                 }
                 _context.Transactions.Add(transaction);
                 await _context.SaveChangesAsync();

                 return CreatedAtAction("GetTransaction", new { id = transaction.TransactionId }, transaction);
     */

            if (ModelState.IsValid)

            {



                // Check if the Customer with the provided ID exists

                var existingCustomer = _context.Customers.Find(transaction.By);  // CHECKING FOR customerId (named as BY) wheather it is present inside dbcontxt.customer database table or not
                                                                                 // if id is found then retrive the whole customer
                                                                                 // public int? By { get; set; } 



                if (existingCustomer != null)  // if that customer found and same customer is saed into existing customervaraible then

                {

                    // Customer exists, use the existing one

                    transaction.ByNavigation = existingCustomer;  // use that exiusting customer into ByNavigation.
                                                                  // public virtual Customer? ByNavigation { get; set; }



                    _context.Transactions.Add(transaction);
                    _context.SaveChanges();

                    return Ok(transaction);


                }

                else

                {

                    // Customer does not exist, return a 404 Not Found response

                    return NotFound("Customer not found");

                }

            }

            return BadRequest(ModelState);




        }

       
                // DELETE: api/Transactions/5
                [HttpDelete("{id}")]
                public async Task<IActionResult> DeleteTransaction(long id)
                {
                    if (_context.Transactions == null)
                    {
                        return NotFound();
                    }
                    var transaction = await _context.Transactions.FindAsync(id);
                    if (transaction == null)
                    {
                        return NotFound();
                    }

                    _context.Transactions.Remove(transaction);
                    await _context.SaveChangesAsync();

                    return NoContent();
                }

          
        
        
        
        
        
        
        //--------------------CUSTOM METHODS---------------------------------   
        
        
        
        
        
        
        
        
        
        
        
        //GET: MINI STATEMENT
                [HttpGet("MiniStatement_By_AccountId/{accountId}/{numberOfTransactions}")]
                public ActionResult<List<Transaction>> GetMiniStatement(
            long accountId,
            int numberOfTransactions)
                {
                    try
                    {
                        var transactions = _context.Transactions
                            .Where(t => t.SourceAccount == accountId || t.DestinationAccount == accountId)
                            .OrderByDescending(t => t.Date)
                            .Take(numberOfTransactions)
                            .ToList();

                        if (!transactions.Any())
                        {
                            return NotFound($"No transactions found for the account {accountId}.");
                        }

                        return transactions;
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle it appropriately based on your application's needs.
                        return StatusCode(500, "An error occurred while processing the request.");
                    }
                }


        

                // GET: api/Transactions/Range/5/2023-01-01/2023-12-31
                [HttpGet("Transactions_By_AccountId_DateRange/{accountId}/{startDate}/{endDate}")]
                public ActionResult<List<Transaction>> GetTransactionsInDateRange(
            long accountId,
            string startDate,
            string endDate)
                {
                    try
                    {
                        // Define the expected date format
                        string dateFormat = "yyyy-MM-dd";

                        if (!DateTime.TryParseExact(startDate, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedStartDate) ||
                            !DateTime.TryParseExact(endDate, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedEndDate))
                        {
                            return BadRequest("Invalid date parameters. Dates must be in the format 'yyyy-MM-dd'.");
                        }

                        // Ensure that the end date includes the entire day by adding one day and setting time to midnight.
                        DateTime endDateWithTime = parsedEndDate.AddDays(1);

                        var transactionsInRange = _context.Transactions
                            .Where(t => (t.SourceAccount == accountId || t.DestinationAccount == accountId)
                                        && t.Date >= parsedStartDate && t.Date < endDateWithTime)
                            .OrderByDescending(t => t.Date)
                            .ToList();

                        if (!transactionsInRange.Any())
                        {
                            return NotFound("No transactions found in the specified date range.");
                        }

                        return transactionsInRange;
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle it appropriately based on your application's needs.
                        return StatusCode(500, "An error occurred while processing the request.");
                    }
                }

        

                // GET : GET ALL TRANSACTIONS BY CUSTOMER ID   -----------WORKING
                [HttpGet("GetTransactionsByCustomer/{customerId}")]
                public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactionsByCustomer(int customerId)
                {
                    try
                    {
                        var transactions = await _context.Transactions
                            .Where(t => t.By == customerId)
                            .ToListAsync();

                        if (transactions == null || transactions.Count == 0)
                        {
                            return NotFound($"No transactions found for customer with ID {customerId}");
                        }

                        return transactions;
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle it as needed
                        return StatusCode(500, "Internal server error");
                    }
                }

                // GET : GET N TRANSACTIONS BY CUSTOMER ID
                [HttpGet("GetAllTransactionsByCustomer/{customerId}/{n}")]
                public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactionsByCustomer(int customerId, int n)
                {
                    try
                    {
                        var transactions = await _context.Transactions
                            .Where(t => t.By == customerId)
                            .Take(n) // Take only the first 'n' transactions
                            .ToListAsync();

                        if (transactions == null || transactions.Count == 0)
                        {
                            return NotFound($"No transactions found for customer with ID {customerId}");
                        }

                        return transactions;
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle it as needed
                        return StatusCode(500, "Internal server error");
                    }
                }

/*
                // GET LAST TRANSACTION BY ID 
                [HttpGet("MiniStatement_ByAccountId/SP{accountId}/{numberOfTransactions}")]
                public ActionResult<List<Transaction>> GetMiniStatement_SP(
            long accountId,
            int numberOfTransactions)
                {
                    try
                    {
                        var transactions = _context.Transactions
                            .FromSqlRaw("EXEC GetMiniStatement @p0, @p1", accountId, numberOfTransactions)
                            .ToList();

                        if (!transactions.Any())
                        {
                            return NotFound($"No transactions found for the account {accountId}.");
                        }

                        return transactions;
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle it appropriately based on your application's needs.
                        return StatusCode(500, "An error occurred while processing the request.");
                    }
                }*/
/*
                // UPDATE DATABASE BY ACCOUNT ID
                [HttpPost("Deposit_SP/{accountId}")]
                public ActionResult<string> DepositAmount_SP(
            long accountId,
            [FromBody] decimal depositAmount)
                {
                    try
                    {
                        var result = _context.Database.ExecuteSqlRaw("EXEC DepositAmount @p0, @p1", accountId, depositAmount);

                        if (result > 0)
                        {
                            return Ok($"Successfully deposited {depositAmount} to the account {accountId}.");
                        }
                        else
                        {
                            return NotFound($"Account {accountId} not found or deposit failed.");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle it appropriately based on your application's needs.
                        return StatusCode(500, "An error occurred while processing the request.");
                    }
                }
*/





                /*// GET : CALL ALL TRANSACTIONS BY CUSTOMER ID
                [HttpGet("GetTransactionsByCustomer_SP/{customerId}/{n}")]
                public ActionResult<IEnumerable<Transaction>> GetTransactionsByCustomer_SP(int customerId, int n)
                {
                    try
                    {
                        var transactions = _context.Transactions
                            .FromSqlRaw("EXEC GetTransactionsByCustomer @CustomerId={0}, @N={1}", customerId, n)
                            .ToList();

                        if (transactions == null || transactions.Count == 0)
                        {
                            return NotFound($"No transactions found for customer with ID {customerId}");
                        }

                        return transactions;
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle it as needed
                        return StatusCode(500, "Internal server error");
                    }
                }



*/








                // GET: api/Transactions/Customer/{customerId}/LastN/{numberOfTransactions}
                [HttpGet("Customer/{customerId}/LastN/{numberOfTransactions}")]
                public ActionResult<List<Transaction>> GetNTransactionsByCustomer(int customerId, int numberOfTransactions)
                {
                    try
                    {
                        var transactions = _context.Transactions
                            .OrderByDescending(t => t.Date)
                            .Take(numberOfTransactions)
                            .ToList();

                        if (!transactions.Any())
                        {
                            return NotFound($"No transactions found for the customer with ID {customerId}.");
                        }

                        return transactions;
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle it appropriately based on your application's needs.
                        return StatusCode(500, "An error occurred while processing the request.");
                    }
                }



        


        private bool TransactionExists(long id)
        {
            return (_context.Transactions?.Any(e => e.TransactionId == id)).GetValueOrDefault();
        }

    }
}
