using System.ComponentModel.DataAnnotations;

namespace OdevDagitim.ViewModel
{
    public class RegisterModel
    {

        [Display(Name = "Kullanıcı Adı")]
        [Required(ErrorMessage = "Kullanıcı Adı Giriniz!")]
        public string UserName { get; set; }



        [Display(Name = "E-Posta")]
        [Required(ErrorMessage = "E-Posta Giriniz!")]
        public string Email { get; set; }


        [Display(Name = "Parola")]
        [Required(ErrorMessage = "Parola Giriniz!")]
        public string PasswordHash { get; set; }


        [Display(Name = "Parola Tekrar")]
        [Required(ErrorMessage = "Parola Tekrar Giriniz!")]
        public string PasswordConfirm { get; set; }
    }
}
