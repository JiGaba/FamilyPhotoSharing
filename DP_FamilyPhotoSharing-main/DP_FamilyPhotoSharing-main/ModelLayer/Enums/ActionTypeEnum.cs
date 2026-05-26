using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Enums
{
    public enum ActionTypeEnum : int
    {
        [Description("Vše.")]
        All = 0,
        [Description("Systémové informace.")]
        Info = 1,
        [Description("Uživatelská skupina.")]
        Group = 10,
        [Description("Vytvoření uživatelské skupiny.")]
        AddGroup = 11,
        [Description("Editace uživatelské skupiny.")]
        EditGroup = 12,
        [Description("Smazání uživatelské skupiny.")]
        RemoveGroup = 13,
        [Description("Výběr uživatelské skupiny.")]
        SelectGroup = 14,
        [Description("Uživatel.")]
        User = 20,
        [Description("Vytvoření uživatele.")]
        AddUser = 21,
        [Description("Editace uživatele.")]
        EditUser = 22,
        [Description("Smazání uživatele.")]
        RemoveUser = 23,
        [Description("Výběr uživatele.")]
        SelectUser = 24,
        [Description("Přihlášení uživatele.")]
        LoginUser = 25,
        [Description("Odhlášení uživatele.")]
        LogoutUser = 26,
        [Description("Zobrazení obnovovacího tokenu.")]
        ShowRefreshToken = 27,
        [Description("Zneplatnění obnovovacího tokenu.")]
        UpdateRefreshToken = 28,
        [Description("Vytvoření obnovovacího tokenu.")]
        AddRefreshToken = 29,
        [Description("Fotografie.")]
        Photo = 30,
        [Description("Přidání fotografie.")]
        AddPhoto = 31,
        [Description("Uložení fotografie do souborového systému.")]
        AddPhotoFS = 32,
        [Description("Uložení náhledu fotografie do souborového systému.")]
        AddPhotoFSThumbnail = 33,
        [Description("Úprava fotografie.")]
        EditPhoto = 34,
        [Description("Odstranění fotografie.")]
        RemovePhoto = 35,
        [Description("Výběr fotografie.")]
        SelectPhoto = 36,
        [Description("Foto album.")]
        PhotoAlbum = 40,
        [Description("Vytvoření fotoalba.")]
        AddPhotoAlbum = 41,
        [Description("Editace fotoalba.")]
        EditPhotoAlbum = 42,
        [Description("Smazání fotoalba")]
        RemovePhotoAlbum = 43,
        [Description("Výběr photo alba.")]
        SelectPhotoAlbum = 44,
        [Description("Šifrování.")]
        PhotoEncrypt = 50,
        [Description("Přidání šifrovacích klíčů.")]
        AddPhotoEncrypt = 51,
        [Description("Odstranění šifrovacích klíčů.")]
        RemovePhotoEncrypt = 52,
        [Description("Výběr šifrovacích klíčů.")]
        SelectPhotoEncrypt = 53,
        [Description("Systémový obrázek.")]
        SystemImages = 60,
        [Description("Přidání systémového obrázku.")]
        AddSystemImages = 61,
        [Description("Editace systémového obrázku.")]
        EditSystemImages = 62,
        [Description("Odstranění systémového obrázku.")]
        RemoveSystemImages = 63,
        [Description("Výběr systémového obrázku.")]
        SelectSystemImages = 64,
        [Description("Šifrovací klíče.")]
        UserKeys = 70,
        [Description("Přidání šifrovacích klíčů.")]
        AddUserKeys = 71,
        [Description("Editace šifrovacích klíčů.")]
        EditUserKeys = 72,
        [Description("Odstranění šifrovacích klíčů.")]
        RemoveUserKeys = 73,
        [Description("Výběr šifrovacích klíčů.")]
        SelectUserKeys = 74,
        [Description("Sdílené fotoalbum.")]
        SharedPhotoAlbum = 80,
        [Description("Vytvoření sdíleného fotoalba.")]
        AddSharedPhotoAlbum = 81,
        [Description("Editace sdíleného fotoalba.")]
        EditSharedPhotoAlbum = 82,
        [Description("Smazání sdíleného fotoalba")]
        RemoveSharedPhotoAlbum = 83,
        [Description("Výběr sdíleného photo alba.")]
        SelectSharedPhotoAlbum = 84,
        [Description("Jiné")]
        Other = 100
    }
}
