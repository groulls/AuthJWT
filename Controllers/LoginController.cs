using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthReg.Data;
using AuthReg.Models;
using Microsoft.AspNetCore.Identity;
using AuthReg.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;

namespace AuthReg.Controllers
{
    [Route ("[controller]")]
    [ApiController]
    [Authorize]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAuthenticationJWT _authenticationJWT;

        public LoginController(AppDbContext context, IAuthenticationJWT authenticationJWT)
        {
            _context = context;
            _authenticationJWT = authenticationJWT;
        }

        // GET: api/Login
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Login/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Login/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.UserID)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Login
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [AllowAnonymous]
        [HttpPost("auth")]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            PasswordHasher<string> hasher = new PasswordHasher<string>();
            var empty = await _context.Users.Where(u=>u.UserName == user.UserName).ToArrayAsync();

            var arr = empty[0].Password;

            if (empty.Length != 0)
            {
                var indef = hasher.VerifyHashedPassword(user.UserName, arr, user.Password);

                if(indef.ToString() == "Success")
                {
                    //return CreatedAtAction("GetUser", new { id = user.UserID }, user);
                    var token = _authenticationJWT.Authenticate(user.UserName, user.Password);
                    if (token == null)
                        return Unauthorized();
                    else
                        HttpContext.Response.Cookies.Append(
               ".AspNetCore.Application.Id",token,
              
               new CookieOptions { MaxAge = TimeSpan.FromMinutes(60) });

                    return Ok(token);

                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return NotFound();
            }

            //_context.Users.Add(user);
            //await _context.SaveChangesAsync();

            
        }

        // DELETE: api/Login/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserID == id);
        }
    }
}
