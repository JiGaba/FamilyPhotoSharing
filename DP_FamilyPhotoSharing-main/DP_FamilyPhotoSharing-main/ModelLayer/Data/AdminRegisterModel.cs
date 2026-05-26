using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Data
{
    public class AdminRegisterModel
    {
        [Required(ErrorMessage = "Vyplňte prosím uživatelský login, jedná se o povinnou položku.")]
        [MaxLength(100, ErrorMessage = "Login může mít maximálně 100 znaků.")]
        public required string UserLogin { get; set; }
        [Required(ErrorMessage = "Vyplňte prosím jméno, jedná se o povinnou položku.")]
        [MaxLength(100, ErrorMessage = "Jméno může mít maximálně 100 znaků.")]
        public required string UserName { get; set; }
        [Required(ErrorMessage = "Vyplňte prosím příjmení, jedná se o povinnou položku.")]
        [MaxLength(255, ErrorMessage = "Příjmení může mít maximálně 255 znaků.")]
        public required string UserSurname { get; set; }
        [Required(ErrorMessage = "Vyplňte prosím heslo, jedná se o povinnou položku.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?!.*\s).{12,}$", ErrorMessage = "Heslo musí být bez mezer s délkou minimálně 12 znaků. Musí obsahovat číslo a velké písmeno.")]
        [MaxLength(200, ErrorMessage = "Heslo může mít maximálně 200 znaků.")]
        public required string UserPasswordPlain { get; set; }
        [Required(ErrorMessage = "Zopakujte prosím heslo.")]
        [Compare("UserPasswordPlain", ErrorMessage = "Hesla se neshodují.")]
        public required string UserPasswordRepeat { get; set; }
    }
}
