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
    public class BeneficiariesController : ControllerBase
    {
        private readonly BankDBC _context;

        public BeneficiariesController(BankDBC context)
        {
            _context = context;
        }

        // GET: api/Beneficiaries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Beneficiary>>> GetBeneficiaries()
        {
          if (_context.Beneficiaries == null)
          {
              return NotFound();
          }
            return await _context.Beneficiaries.ToListAsync();
        }

        // GET: api/Beneficiaries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Beneficiary>> GetBeneficiary(int id)
        {
          if (_context.Beneficiaries == null)
          {
              return NotFound();
          }
            var beneficiary = await _context.Beneficiaries.FindAsync(id);

            if (beneficiary == null)
            {
                return NotFound();
            }

            return beneficiary;
        }

        // PUT: api/Beneficiaries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBeneficiary(int id, Beneficiary beneficiary)
        {
            if (id != beneficiary.BeneficiaryId)
            {
                return BadRequest();
            }

            _context.Entry(beneficiary).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BeneficiaryExists(id))
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

        // NO DUPLICATE POST/ADD

        // POST: api/Beneficiaries
        [HttpPost]
        public async Task<ActionResult<Beneficiary>> PostBeneficiary(Beneficiary beneficiary)
        {
            /*if (_context.Beneficiaries == null)
            {
                return Problem("Entity set 'BDBContext.Beneficiaries'  is null.");
            }
              _context.Beneficiaries.Add(beneficiary);
              await _context.SaveChangesAsync();

              return CreatedAtAction("GetBeneficiary", new { id = beneficiary.BeneficiaryNumber }, beneficiary);
                 */

            if (ModelState.IsValid)

            {



                // Check if the Customer with the provided ID exists

                var existingCustomer = _context.Customers.Find(beneficiary.CustomerId);  // check weather the customerId of the benefiociary table is present inside the Customer Table that is being referenced


                if (existingCustomer != null)  // if cusotmer id exists

                {

                    // Customer exists, use the existing one

                    beneficiary.Customer = existingCustomer;



                    _context.Beneficiaries.Add(beneficiary);
                    _context.SaveChanges();

                    return Ok(beneficiary);


                }

                else

                {

                    // Customer does not exist, return a 404 Not Found response

                    return NotFound("Customer not found");

                }

            }

            return BadRequest(ModelState);





        }


        // DELETE: api/Beneficiaries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBeneficiary(int id)
        {
            if (_context.Beneficiaries == null)
            {
                return NotFound();
            }
            var beneficiary = await _context.Beneficiaries.FindAsync(id);
            if (beneficiary == null)
            {
                return NotFound();
            }

            _context.Beneficiaries.Remove(beneficiary);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BeneficiaryExists(int id)
        {
            return (_context.Beneficiaries?.Any(e => e.BeneficiaryId == id)).GetValueOrDefault();
        }
    }
}
