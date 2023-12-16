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
    public class CustomersController : ControllerBase
    {
        private readonly BankDBC _context;

        public CustomersController(BankDBC context)
        {
            _context = context;
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
          if (_context.Customers == null)
          {
              return NotFound();
          }
            return await _context.Customers.ToListAsync();
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
          if (_context.Customers == null)
          {
              return NotFound();
          }
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }

        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
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

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
          if (_context.Customers == null)
          {
              return Problem("Entity set 'BankDBC.Customers'  is null.");
          }
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCustomer", new { id = customer.CustomerId }, customer);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            if (_context.Customers == null)
            {
                return NotFound();
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // UPDATE PASSWORD  ---> LINQ METHOD


        [HttpPut("updateCustomerPassword/{id}")]
        public async Task<IActionResult> UpdateCustomerPassword(int id, string newPassword)
        {
            if (id <= 0 || string.IsNullOrEmpty(newPassword))
            {
                return BadRequest("Invalid input");
            }

            var cust = await _context.Customers.FindAsync(id);

            if (cust == null)
            {
                return NotFound();
            }

            cust.Password = newPassword;     // change password

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))  // validate if found any
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

        //UPDATE PHONE NUMBER
        // PUT: api/Customers/UpdatePhoneNumber/5
        [HttpPut("UpdateCustomerPhoneNumber/{id}")]
        public async Task<IActionResult> UpdateCustomerPhoneNumber(int id, long newPhoneNumber)

        {

            if (id <= 0 || newPhoneNumber < 1000000000 || newPhoneNumber > 9999999999)
            {
                return BadRequest("Invalid input. Phone number must be a 10-digit positive integer.");
            }


            var existingCustomer = await _context.Customers.FindAsync(id);

            if (existingCustomer == null)
            {
                return NotFound();
            }

            // Update only the phone number
            existingCustomer.Phone = newPhoneNumber; // Assuming Phone is of type long

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))  // if id not found in any of the customer then return notfound
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



        // PUT: UPDATE ADDRESS
        // PUT: api/Customers/UpdateAddress/5
        [HttpPut("UpdateCustomerAddress/{id}")]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] string newAddress)
        {
            if (id <= 0 || string.IsNullOrEmpty(newAddress))
            {
                return BadRequest("Invalid input");
            }
            var existingCustomer = await _context.Customers.FindAsync(id);

            if (existingCustomer == null)
            {
                return NotFound();
            }

            // Update only the address
            existingCustomer.Address = newAddress;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
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





        private bool CustomerExists(int id)
        {
            return (_context.Customers?.Any(e => e.CustomerId == id)).GetValueOrDefault();
        }
    }
}
