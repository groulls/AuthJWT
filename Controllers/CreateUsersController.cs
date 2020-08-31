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

        public CreateUsersController(AppDbContext context)
        {
            _context = context;
        }

        #region Хэширование методом SHA256
        //public string CreateSalt (int size)
        //{
        //    var rng = new RNGCryptoServiceProvider();
        //    var buff = new byte[size];
        //    rng.GetBytes(buff);
        //    return Convert.ToBase64String(buff);

        //}
                
        //public string GenerateSHA256Hash (User user, string salt)
        //{

        //    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(user.Password + salt);
        //    SHA256Managed sHA256Managed = new SHA256Managed();

        //    byte[] hash = sHA256Managed.ComputeHash(bytes);

        //    return ByteArrayToHexString(hash);
        //}

        //private string ByteArrayToHexString(byte[] hash)
        //{
        //    StringBuilder hex = new StringBuilder(hash.Length * 2);
        //    foreach (byte b in hash)
        //        hex.AppendFormat("{0:x2}", b);
        //    return hex.ToString();
        //}
        #endregion

         // POST: api/Users
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost("registry")]
        //[ValidateAntiForgeryToken]
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
                
                Role userRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "user");
                user.Password = empty;
                if(userRole !=null)
                {
                    user.Role = userRole;
                }
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

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserID == id);
        }
    }
}
