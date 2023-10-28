using System.Security.Cryptography;
using System.Text;

namespace UserAuthAPI.Helpers
{
    public static class EncryptedDataGenerator
    {

        public static string MD5Hash()
        {
            var random = new Random();
            var input = Guid.NewGuid().ToString() + "." + random.Next(9999, 99999).ToString();
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] dizi = Encoding.UTF8.GetBytes(input);
            dizi = md5.ComputeHash(dizi);
            StringBuilder sb = new StringBuilder();
            foreach (byte ba in dizi)
            {
                sb.Append(ba.ToString("x2").ToLower());
            }
            return sb.ToString();
        }

        public static string SHA1Hash()
        {
            var random = new Random();
            var input = DateTime.Now.ToShortTimeString().ToString().Replace(" ", "").Replace(":", "") + "." + Guid.NewGuid().ToString() + "." + random.Next(9999, 99999).ToString();
            using (SHA1 sha1Hash = SHA1.Create())
            {
                byte[] sourceBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
                return hash.ToString();
            }
        }

        public static string SHA256Hash()
        {
            var random = new Random();
            var input = DateTime.Now.ToShortTimeString().ToString().Replace(" ", "").Replace(":", "") + "_" + Guid.NewGuid().ToString() + "_" + random.Next(9999, 99999).ToString();
            using (SHA256 sha1Hash = SHA256.Create())
            {
                byte[] sourceBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
                return hash.ToString();
            }
        }

        public static string SHA384Hash()
        {
            var random = new Random();
            var input = DateTime.Now.ToShortTimeString().ToString().Replace(" ", "").Replace(":", "") + "x" + Guid.NewGuid().ToString() + "_" + random.Next(99999, 999999).ToString();
            using (SHA384 sha384Hash = SHA384.Create())
            {
                byte[] sourceBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha384Hash.ComputeHash(sourceBytes);
                string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
                return hash.ToString();
            }
        }

        public static string SHA512Hash()
        {
            var random = new Random();
            var input = DateTime.Now.ToShortTimeString().ToString().Replace(" ", "").Replace(":", "") + "#" + Guid.NewGuid().ToString() + "=" + random.Next(99999, 999999).ToString();
            using (SHA512 sha512Hash = SHA512.Create())
            {
                byte[] sourceBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha512Hash.ComputeHash(sourceBytes);
                string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
                return hash.ToString();
            }
        }

        public static string RandomGenerate(int KacKarakter)
        {
            char[] karakter = "0123456789AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz".ToCharArray();
            string result = string.Empty;
            Random r = new Random();
            for (int i = 0; i < KacKarakter; i++)
            {
                result += karakter[r.Next(0, karakter.Length - 1)].ToString();
            }
            return result;
        }

    }
}
