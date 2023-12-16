using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BANKPROJECT8.Models;

namespace BANKPROJECT8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly BankDBC _context;

        public AccountsController(BankDBC context)
        {
            _context = context;
        }

        // GET: api/Accounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
          if (_context.Accounts == null)
          {
              return NotFound();
          }
            return await _context.Accounts.ToListAsync();
        }

        // GET: api/Accounts/5  -------ACCOUNT DEATILS BY ACCOUNT ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccount(long id)
        {
          if (_context.Accounts == null)
          {
              return NotFound();
          }
            var account = await _context.Accounts.FindAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            return account;
        }


        // GET: ACCOUNTS BY CUSTOMER ID -----ALL ACCOUNT DEATILS BY CUSTOMER ID
        [HttpGet("Accounts_BY_Custid/{id}")]
        public ActionResult<Account> GetAccountsByCustomerId(long id)
        {
            if (_context.Accounts == null)
            {
                return NotFound();
            }
            var account = _context.Accounts.Where(acc => acc.CustomerId == id).FirstOrDefault();

            if (account == null)
            {
                return NotFound();
            }

            return account;
        }


        // PUT: api/Accounts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccount(long id, Account account)
        {
            if (id != account.AccountNumber)
            {
                return BadRequest();
            }

            _context.Entry(account).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
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

        //NO DUPLICATE ADDING OF ACCOUNT

        // POST: api/Accounts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Account>> PostAccount(Account account)
        {
            /*if (_context.Accounts == null)
            {
                return Problem("Entity set 'BDBContext.Accounts'  is null.");
            }
              _context.Accounts.Add(account);
              await _context.SaveChangesAsync();

              return CreatedAtAction("GetAccount", new { id = account.AccountNumber }, account);*/



            if (ModelState.IsValid)

            {



                // Check if the Customer with the provided ID exists

                var existingCustomer = _context.Customers.Find(account.CustomerId);  // check weather the customerId of the Accouint table is present inside the Customer Table that is being referenced



                if (existingCustomer != null)  // if cusotmer with the same id is found then

                {

                    // Customer exists, use the existing one

                    account.Customer = existingCustomer;   //public virtual Customer? Customer { get; set; } = existingCustomer

                    //account.AccountNumber += 1000000000;

                    _context.Accounts.Add(account); // use that existing cutomer and add only the account(not a new Customer--no duplicate customer)

                    _context.SaveChanges();

                    return Ok(account);

                }

                else

                {

                    // Customer does not exist, return a 404 Not Found response

                    return NotFound("Customer not found");

                }

            }

            return BadRequest(ModelState);



        }


        // DELETE: api/Accounts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(long id)
        {
            if (_context.Accounts == null)
            {
                return NotFound();
            }
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        //--------------------------------------------------------------DEPOSIT---------------------------------------------------
        // PUT: deposit 
        // api/ACCOUNT/
        [HttpPut("DepositAmount/{id}")]
        public async Task<IActionResult> DepositAmount(long id, decimal deposit)
        {
            if (deposit <= 0)
            {
                return BadRequest("Invalid deposit amount. It should be a positive value with optional precision of 2 decimal places.");
            }

            // Find the account with the given ID
            var existingAccount = await _context.Accounts.FindAsync(id);

            // Check if the account exists
            if (existingAccount == null)
            {
                return NotFound("Account not found");
            }

            // Update the customer's amount with the deposited value
            existingAccount.Balance += deposit;

            try
            {
                // Save changes to the database
                await _context.SaveChangesAsync();

                // Return a simplified response indicating success
                return Ok(new { Balance = existingAccount.Balance, Message = "Deposit successful" });
            }
            catch (DbUpdateConcurrencyException)
            {
                // Log the exception details for debugging purposes
                // Log.Error("Concurrency exception: " + ex.Message);

                // Return a more informative error response for concurrency issues
                return StatusCode(409, "Concurrency conflict: Another user may have modified the same data.");
            }
            catch (Exception)
            {
                // Log other exception details for debugging purposes
                // Log.Error("An error occurred while updating the account.");

                // Return a generic error response for other server errors
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }



        //--------------------------------------------------------END------DEPOSIT---------------------------------------------------
        //_____________________________________________________WITHDRAW-------------------------------------------------------
        // api/Account/Withdraw/id
        // PUT: api/Accounts/WithdrawAmount/5
        [HttpPut("WithdrawAmount/{id}")]
        public async Task<IActionResult> WithdrawAmount(long id, decimal withdrawal)
        {
            // Find the account with the given ID
            var existingAccount = await _context.Accounts.FindAsync(id);

            // Check if the account exists
            if (existingAccount == null)
            {
                return NotFound("Account not found");
            }

            // Validate the withdrawal amount
            if (withdrawal <= 0)
            {
                return BadRequest("Invalid withdrawal amount. Withdrawal amount should be a positive value ");
            }
            if (withdrawal > existingAccount.Balance)
            {
                return BadRequest("Amount should not exceed the account balance. ");
            }


            // Update the customer's amount by deducting the withdrawn value
            existingAccount.Balance -= withdrawal;

            try
            {
                // Save changes to the database
                await _context.SaveChangesAsync();

                // Return the updated account details or a success message
                return Ok(existingAccount); // You can customize this based on your needs
            }
            catch (DbUpdateConcurrencyException)
            {
                // If there's a concurrency exception (e.g., another user modified the same data),
                // check if the account still exists
                if (!AccountExists(id))
                {
                    return NotFound("Account not found");
                }
                else
                {
                    // Log the exception details for debugging purposes
                    // Log.Error("Concurrency exception: " + ex.Message);
                    // Return a more informative error response
                    return StatusCode(500, "An error occurred while updating the account.");
                }
            }
        }



        //__________________________________________________END___WITHDRAW-------------------------------------------------------


        // FUND TRANSFER---------------TRANSFER FUNDS WITHOUT NEED OF BENEFICIARY
        // PUT: api/Customers/TransferAmount
        [HttpPut("GLOBAL_TransferAmount")]
        public IActionResult Any_TransferAmount(long sourceAccountId, long destinationAccountId, decimal amount)
        {
            if (amount <= 0)
            {
                return BadRequest("Amount should be a positive value");
            }
            // Find the source account
            var sourceAccount = _context.Accounts
                .FirstOrDefault(a => a.AccountNumber == sourceAccountId);

            // Find the destination account
            var destinationAccount = _context.Accounts
                .FirstOrDefault(a => a.AccountNumber == destinationAccountId);

            // Check if both accounts exist
            if (sourceAccount == null || destinationAccount == null)
            {
                return NotFound("One or both accounts not found");
            }

            // Check if the source account has sufficient balance
            if (sourceAccount.Balance < amount)
            {
                return BadRequest("Insufficient balance in the source account");
            }

            // Update the balances
            sourceAccount.Balance -= amount;
            destinationAccount.Balance += amount;

            // Create transactions for the source and destination accounts
            var transactionSource = new Transaction         //------------------------TABLE1:DEDUCTION FROM SOURCE 
            {
                SourceAccount = sourceAccountId,
                DestinationAccount = destinationAccountId,
                // Date = DateTime.Now,
                Amount = -amount, // Negative amount for withdrawal
                TransactionType = "Transfer",
                Description = $"Amount transferred to Destination : {destinationAccountId}",
                By = destinationAccount.CustomerId,

            };


            var transactionDestination = new Transaction     //------------------------TABLE2:ADDED TO DESTINATION
            {
                SourceAccount = sourceAccountId,
                DestinationAccount = destinationAccountId,
                // Date = DateTime.Now,
                Amount = amount,
                TransactionType = "Transfer",
                Description = $"Amount received from Source Account: {sourceAccountId}",
                By = destinationAccount.CustomerId,

            };

            _context.Transactions.AddRange(transactionSource, transactionDestination);

            try
            {
                // Save changes to the database
                _context.SaveChanges();

                // Return the updated source and destination accounts or a success message
                return Ok(new
                {
                    SourceAccount = sourceAccount,
                    DestinationAccount = destinationAccount
                });
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error transferring amount: {ex}");

                // Return a meaningful error response
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }




        // FUND TRANSFER
        // PUT: api/BENEFICIARY/TransferAmount

        [HttpPut("BENEFICIARY_TransferAmount")]
        public async Task<IActionResult> Beneficiary_TransferAmount(long sourceAccountId, long destinationAccountId, decimal amount)
        {
            if (amount <= 0)
            {
                return BadRequest("Amount should be a positive value");
            }

            var sourceAccount = await _context.Accounts.FindAsync(sourceAccountId);
            var destinationAccount = await _context.Accounts.FindAsync(destinationAccountId);

            if (sourceAccount == null || destinationAccount == null)
            {
                return NotFound("One or more accounts not found");
            }

            // Check if the destination account is a valid beneficiary for the customer
            var beneficiaryExists = _context.Beneficiaries
                .Any(b => b.CustomerId == sourceAccount.CustomerId && b.BeneficiaryAccountNumber == destinationAccount.AccountNumber);

            if (!beneficiaryExists)
            {
                return BadRequest("Beneficiary not found for the source account");
            }

            // Check if the source account has sufficient balance
            if (sourceAccount.Balance < amount)
            {
                return BadRequest("Insufficient balance in the source account");
            }

            // Perform the transfer
            sourceAccount.Balance -= amount;
            destinationAccount.Balance += amount;

            // Create a new transaction for the source account
            var sourceTransaction = new Transaction
            {
                SourceAccount = sourceAccountId,
                DestinationAccount = destinationAccountId,
                // Date = DateTime.Now,
                Amount = amount,
                TransactionType = "Transfer",
                Description = $"Amount transferred to Destination Account: {destinationAccountId}",
                By = destinationAccount.CustomerId,



            };

            // Create a new transaction for the destination account
            var destinationTransaction = new Transaction
            {
                SourceAccount = sourceAccountId,
                DestinationAccount = destinationAccountId,
                Amount = amount,
                // Date = DateTime.Now,
                TransactionType = "Transfer",
                Description = $"Amount received from Source Account: {sourceAccountId}",
                By = destinationAccount.CustomerId,
            };

            _context.Transactions.Add(sourceTransaction);
            _context.Transactions.Add(destinationTransaction);

            await _context.SaveChangesAsync();

            return Ok("Transfer successful");
        }

        // PUT : DEPOSIT AMOUNT ------------->STORED PROCEDURE FOR DEPOSITING AMOUNT
        // api/depositamount_sp/
        [HttpPut("DepositAmount_SP/{id}")]
        public async Task<IActionResult> DepositAmount_SP(long id, decimal Deposit)
        {
            if (id <= 0 || Deposit <= 0)
            {
                return BadRequest("Invalid input");
            }

            try
            {
                _context.Database.ExecuteSqlRaw("EXEC DepositAmount @AccountNumber={0}, @Deposit={1}", id, Deposit);
                return NoContent();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error depositing amount: {ex}");

                // Return a meaningful error response
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        private bool AccountExists(long id)
        {
            return (_context.Accounts?.Any(e => e.AccountNumber == id)).GetValueOrDefault();
        }
    }
}
