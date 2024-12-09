using System.ComponentModel.DataAnnotations;

namespace OdevDagitim.ViewModel
{
    public class LoginModel
    {
        [Display(Name = "Kullanıcı Adı")]
        [Required(ErrorMessage = "Kullanıcı Adı Giriniz!")]
        public string UserName { get; set; }

        [Display(Name = "Parola")]
        [Required(ErrorMessage = "Parola Giriniz!")]
        public string Password { get; set; }

      
    }
}
