using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthReg.Data;
using AuthReg.Models;
using System.Security.Cryptography;
using System;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;

namespace AuthReg.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CreateUsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        //private readonly UserManager<User> _userManager;
        //private readonly SignInManager<User> _signInManager;
     

        public CreateUsersController(AppDbContext context)
        {
            _context = context;
            //_userManager = userManager;
            //_signInManager = signInManager;
        }

        public string CreateSalt (int size)
        {
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[size];
            rng.GetBytes(buff);
            return Convert.ToBase64String(buff);

        }

        public string GenerateSHA256Hash (User user, string salt)
        {
     
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(user.Password + salt);
            SHA256Managed sHA256Managed = new SHA256Managed();

            byte[] hash = sHA256Managed.ComputeHash(bytes);

            return ByteArrayToHexString(hash);
        }

        private string ByteArrayToHexString(byte[] hash)
        {
            StringBuilder hex = new StringBuilder(hash.Length * 2);
            foreach (byte b in hash)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
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

        // PUT: api/Users/5
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


         // POST: api/Users
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost("registry")]
        public async Task<ActionResult<User>> PostAuth(User user)
        {

            //string salt = CreateSalt(10);
            //string hash = GenerateSHA256Hash(user, salt);

            //user.Password = hash;

            var nickname = await _context.Users.Where(u => u.UserName == user.UserName || u.Email == user.Email).ToArrayAsync();

            if(nickname.Length!= 0)
            {
                return BadRequest();
            }
            else
            {
                PasswordHasher<string> pswHash = new PasswordHasher<string>();

                var empty = pswHash.HashPassword(user.UserName, user.Password);


                var valid = pswHash.VerifyHashedPassword(user.UserName, empty, user.Password);


                user.Password = empty;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetUser", new { id = user.UserID }, user);
            }

      
            



            //if (ModelState.IsValid)
            //{
            //                  // добавляем пользователя
            //    var result = await _userManager.CreateAsync(user);
            //    if (result.Succeeded)
            //    {
            //        // установка куки
            //        await _signInManager.SignInAsync(user, false);
            //        return RedirectToAction("Index", "Home");
            //    }
            //    else
            //    {
            //        foreach (var error in result.Errors)
            //        {
            //            ModelState.AddModelError(string.Empty, error.Description);
            //        }
            //    }
            //}
            //return CreatedAtAction("GetUser", new { id = user.UserID }, user);

            
        }

        // DELETE: api/Users/5
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
