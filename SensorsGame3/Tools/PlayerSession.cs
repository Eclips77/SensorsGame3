namespace SensorsGame3.Tools
{
    public static class PlayerSession
    {
        public static string Username { get; private set; }
        public static bool IsAdmin =>
            !string.IsNullOrEmpty(Username) && Username.Equals("admin", System.StringComparison.OrdinalIgnoreCase);

        public static void Login(string username)
        {
            Username = username;
        }

        public static void Logout()
        {
            Username = null;
        }
    }
}
