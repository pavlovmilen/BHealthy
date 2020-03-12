using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthSocialMediaApp.Models;
using HealthSocialMediaApp.Data;
using Microsoft.AspNetCore.Authorization;

namespace HealthSocialMediaApp.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUsersController
     : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApplicationUsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/applicationusers/a0e61ab9-ef88-470e-b0f0-9e06b1542c4f
        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationUser>> GetUser(string idOfProfileToGet, [FromBody] ApplicationUser userAttemptingGet)
        {
            if (idOfProfileToGet != userAttemptingGet.Id)
            {
                return BadRequest();
            }

            ApplicationUser user = await _context.Users.FindAsync(idOfProfileToGet);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/applicationuser/a0e61ab9-ef88-470e-b0f0-9e06b1542c4f
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutApplicationUser(string idOfProfileToEdit, [FromBody] ApplicationUser newUserInformation)
        {

            if (idOfProfileToEdit != newUserInformation.Id)
            {
                return BadRequest();
            }

            ApplicationUser userToEdit = await _context.Users.FindAsync(idOfProfileToEdit);
            string idOfUserToEdit = userToEdit.Id;
            string emailOfUserToEdit = userToEdit.Email;
            if (newUserInformation.Id != idOfUserToEdit || newUserInformation.Email != emailOfUserToEdit)
            {
                newUserInformation.Id = idOfUserToEdit;
                newUserInformation.Email = emailOfUserToEdit;
            }

            _context.Entry(newUserInformation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApplicationUserExists(idOfProfileToEdit))
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


        private bool ApplicationUserExists(string id)
        {
            return _context.Users.Any(a => a.Id == id);
        }

    }


}