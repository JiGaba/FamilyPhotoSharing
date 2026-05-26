using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Data
{
    public class UserModel
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int RoleId { get; set; }
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
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?!.*\s).{8,}$", ErrorMessage = "Heslo musí být bez mezer s délkou minimálně 8 znaků. Musí obsahovat číslo a velké písmeno.")]
        [MaxLength(200, ErrorMessage = "Heslo může mít maximálně 200 znaků.")]
        public required string UserPasswordPlain { get; set; }
        [Required(ErrorMessage = "Potvrďte nové heslo")]
        [Compare("UserPasswordPlain", ErrorMessage = "Hesla se neshodují")]
        public string ConfirmNewPassword { get; set; }
        [Required(ErrorMessage = "Vyplňte prosím popis, jedná se o povinnou položku.")]
        public required string UserDescription { get; set; }
        public int SystemImagesId { get; set; }
        public int CreateAuthor { get; set; }
        public DateTime CreateDateTime { get; set; }
        public bool Active { get; set; }
        public bool HasBackupCode { get => (!string.IsNullOrWhiteSpace(BackupKey)); }
        public bool Activated { get; set; }
        public string BackupKey { get; set; } = string.Empty;
        public string UserPasswordHash {  get; set; } = string.Empty;
        public string GetName() => $"{UserName} {UserSurname}";
    }
}
