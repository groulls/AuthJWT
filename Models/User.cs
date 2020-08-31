using System.ComponentModel.DataAnnotations;

namespace AuthReg.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }
        [Required(ErrorMessage = "Не указано Имя пользователя")]
        public string UserName { get; set; }
        public string Email { get; set; }
        [Required(ErrorMessage = "Не указан Пароль")]
        public string Password { get; set; }

        public int? RoleId { get; set; }
        public Role Role { get; set; }
    }
}
