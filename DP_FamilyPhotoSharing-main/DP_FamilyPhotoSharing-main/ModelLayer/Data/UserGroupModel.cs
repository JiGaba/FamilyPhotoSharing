using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Data
{
    public class UserGroupModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Vyplňte prosím název rodiny, jedná se o povinnou položku.")]
        public required string GroupName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vyplňte prosím popis rodiny, jedná se o povinnou položku.")]
        [MaxLength(100, ErrorMessage = "Název může mít maximálně 100 znaků.")]
        public required string GroupDescription { get; set; } = string.Empty;
        public string FolderName { get; set; } = string.Empty;
        public DateTime CreateDateTime { get; set; }
        public int CreateAuthor {  get; set; }
    }
}
