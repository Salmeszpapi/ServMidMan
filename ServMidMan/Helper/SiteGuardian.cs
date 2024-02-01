namespace ServMidMan.Helper
{
    public static class SiteGuardian
    {
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
