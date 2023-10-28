namespace UserAuthAPI.Helpers
{
    public static class OtpGenerator
    {
        public static int CreateOtp(int min = 100000, int max = 999999)
        {
            var random = new Random();
            return random.Next(min, max);
        }

        public static string CreateOtpToken()
        {
            return EncryptedDataGenerator.RandomGenerate(8) + "-" + EncryptedDataGenerator.RandomGenerate(16) + "-" + EncryptedDataGenerator.RandomGenerate(8);
        }
    }
}
