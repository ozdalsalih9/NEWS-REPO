using System.ComponentModel.DataAnnotations;

namespace NewsProject.Models
{
    public class User
    {
        public int Id { get; set; }
        [Display(Name ="Username")]
        public string UserName { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
        public string? Role { get; set; }
    }
}
