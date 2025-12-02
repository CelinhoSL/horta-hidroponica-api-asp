namespace Horta_Api
{
    public class Key
    {
        public static string Secret = Environment.GetEnvironmentVariable("JWT_SECRET");
    }
}
