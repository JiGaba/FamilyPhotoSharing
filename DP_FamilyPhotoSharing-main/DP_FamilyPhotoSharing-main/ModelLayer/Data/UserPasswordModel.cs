using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Data
{
    public class UserPasswordModel
    {
        public int Id {  get; set; }
        public string Login { get; set; }
        [Required(ErrorMessage = "Vyplňte prosím původní heslo, jedná se o povinnou položku.")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Vyplňte prosím heslo, jedná se o povinnou položku.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?!.*\s).{8,}$", ErrorMessage = "Heslo musí být bez mezer s délkou minimálně 8 znaků. Musí obsahovat číslo a velké písmeno.")]
        [MaxLength(200, ErrorMessage = "Heslo může mít maximálně 200 znaků.")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Potvrďte nové heslo")]
        [Compare("NewPassword", ErrorMessage = "Hesla se neshodují")]
        public string ConfirmNewPassword { get; set; }
    }
}
