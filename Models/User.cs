using System.ComponentModel.DataAnnotations;

namespace AuthReg.Models
{
    public class User
    {
        [Key]
        public int UserID { get; private set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
