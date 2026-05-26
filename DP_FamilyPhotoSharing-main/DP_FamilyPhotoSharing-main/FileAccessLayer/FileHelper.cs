using SkiaSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace FileAccessLayer
{
    public static class FileHelper
    {
        private const int _maxSize = 200;
        private static readonly Random _random = new Random();
        private const string EXTENSION_ENCRYPTED = "fpscrypt";

        /*
        Jak metoda funguje
        Načte obrázek(JPG / JPEG / PNG / WebP)
        Zvětší nebo zmenší tak, aby menší strana ≥ 150 px
        Vycentruje a ořeže střed na 150 x 150 px
        */

        public static byte[] CreateThumbnail150x150(byte[] data)
        {
            const int thumbSize = 150;

            using var inputStream = new SKManagedStream(new MemoryStream(data));
            using var original = SKBitmap.Decode(inputStream);

            if (original == null)
                throw new Exception("Nepodporovaný formát obrázku.");

            // scale tak, aby menší strana >= thumbSize
            float scale = Math.Max((float)thumbSize / original.Width, (float)thumbSize / original.Height);

            int scaledWidth = (int)(original.Width * scale);
            int scaledHeight = (int)(original.Height * scale);

            using var resizedBitmap = new SKBitmap(scaledWidth, scaledHeight);

            // resize s kvalitním filtrem
            original.ScalePixels(resizedBitmap, new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear));

            // crop středem na 300x300
            int x = (scaledWidth - thumbSize) / 2;
            int y = (scaledHeight - thumbSize) / 2;
            var cropRect = new SKRectI(x, y, x + thumbSize, y + thumbSize);

            using var thumbnail = new SKBitmap(thumbSize, thumbSize);
            resizedBitmap.ExtractSubset(thumbnail, cropRect);

            using var image = SKImage.FromBitmap(thumbnail);
            using var output = new SKDynamicMemoryWStream();

            // uložení jako WebP
            using var encoded = image.Encode(SKEncodedImageFormat.Webp, 80);
            return encoded.ToArray();
        }

        public static string GetFolderName(string folderName)
        {
            if (string.IsNullOrWhiteSpace(folderName)) 
                return string.Empty; 
            
            var chars = folderName.ToCharArray();
            var length = Math.Min(chars.Length, 10);

            if(chars.Length < 10)
            {
                var newChars = new char[10];

                for (int i = 0; i < newChars.Length; i++) 
                { 
                    if (i < chars.Length) 
                        newChars[i] = chars[i]; 
                    else 
                        newChars[i] = GetRandomAsciiLetter(); 
                }

                chars = newChars;
            }

            for (int i = 0; i < length; i++) 
            { 
                if (!IsAsciiLetter(chars[i])) 
                    chars[i] = GetRandomAsciiLetter(); 
            }

            var newFolderName = new string(chars);

            return $"{newFolderName.Substring(0,10)}{GetDateTime()}";
        }

        public static string GetRandomFolderName(int size)
        {
            if (size > 15)
                size = 15;
            
            var chars = new char[size];
            
            for (int i = 0; i < size; i++)
                chars[i] = GetRandomAsciiLetter();

            var folderName = new string(chars);

            return $"{folderName}{GetDateTime()}";
        }

        public static string GetFileName(string fileName, string prefix = "", string originalExtension = "")
        {
            var extension = Path.GetExtension(fileName);
            var nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            var chars = nameWithoutExt.ToCharArray();
            var length = chars.Length > 10 ? 10 : chars.Length;

            for (int i = 0; i < length; i++)
                if (!IsAsciiLetter(chars[i]))
                    chars[i] = GetRandomAsciiLetter();

            var safeExtension = string.IsNullOrWhiteSpace(originalExtension)
                ? EXTENSION_ENCRYPTED
                : originalExtension.TrimStart('.');

            return $"{prefix}{new string(chars, 0, length)}{GetDateTime()}.{safeExtension}";
        }

        private static bool IsAsciiLetter(char c)
            => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');

        private static char GetRandomAsciiLetter()
        {
            if (_random.Next(2) == 0)
                return (char)('A' + _random.Next(26));
            else
                return (char)('a' + _random.Next(26));
        }

        private static string GetDateTime()
            => DateTime.Now.ToString("yyMMddHHmmssfff");
    }
}
