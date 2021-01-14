using System;
using System.IO;
using System.Security.Cryptography;

namespace PA.File.Extensions
{
    public static class FileExtensions
    {
        public static string GetSignature(this FileStream stream)
        {
            using (var sha = new SHA256Managed())
            {
                var hash = sha.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", string.Empty);
            }
        }
    }
}