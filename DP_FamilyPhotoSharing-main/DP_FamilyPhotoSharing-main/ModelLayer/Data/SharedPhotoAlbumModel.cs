using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Data
{
    public class SharedPhotoAlbumModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Vyplňte prosím název fotoalba, jedná se o povinnou položku.")]
        [MaxLength(255, ErrorMessage = "Název může mít maximálně 255 znaků.")]
        public string AlbumName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vyplňte prosím popis fotoalba, jedná se o povinnou položku.")]
        public string AlbumDescription { get; set; } = string.Empty;
        public int TitlePhotoId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public int CreateAuthor { get; set; }
        public int OwnerUserId { get; set; }
        public int UserGroupsId { get; set; }
    }
}
