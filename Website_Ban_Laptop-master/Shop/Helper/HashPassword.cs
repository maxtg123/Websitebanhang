using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Helper
{
    public static class HashPassword
    {
        public static string EncodePasswordToBase64(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }

        public static string ToMD5(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            var loweredBytes = Encoding.Default.GetBytes(input.ToLower());

            using (var md5 = MD5.Create())
            {
                var output = md5.ComputeHash(loweredBytes);
                var sb = new StringBuilder(output.Length * 2);
                for (var i = 0; i < output.Length; i++)
                {
                    sb.Append(output[i].ToString("X2"));
                }

                var result = EncodePasswordToBase64(sb.ToString().ToLower());

                return result;
            }
        }

        public static string DecodeFrom64(string encodedData)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encodedData);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            return result;
        }
    }
}
