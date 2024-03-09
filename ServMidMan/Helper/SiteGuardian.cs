using MySqlX.XDevAPI;

namespace ServMidMan.Helper
{
    public static class SiteGuardian
    {
        public static Dictionary<string, string> ClientWithSignalRKey { get; set; } = new Dictionary<string, string>();
        public static string? ClientType { get; set; }
        public static int? CurrentClientId { get; set; }
        public static bool CheckSession(HttpContext mySession)
        {
            var result = mySession.Session.GetString("Login");
            if (result == "True")
            {
                return true;
            }
            return false;
        }
    }
}
