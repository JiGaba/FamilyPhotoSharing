using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Data
{
    public class UserBackupKeyModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Vyplňte prosím záchranný klíč, jedná se o povinnou položku.")]
        public string BackupKey { get; set; }
        [Required(ErrorMessage = "Vyplňte prosím svůj login, jedná se o povinnou položku.")]
        public string Login {  get; set; }
        [Required(ErrorMessage = "Vyplňte prosím heslo, jedná se o povinnou položku.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?!.*\s).{8,}$", ErrorMessage = "Heslo musí být bez mezer s délkou minimálně 8 znaků. Musí obsahovat číslo a velké písmeno.")]
        [MaxLength(200, ErrorMessage = "Heslo může mít maximálně 200 znaků.")]
        public string NewPassword { get; set; }
    }
}
