namespace ServMidMan.Helper
{
    public static class SiteGuardian
    {
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
