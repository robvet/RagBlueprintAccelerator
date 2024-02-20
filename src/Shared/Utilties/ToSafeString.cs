namespace Shared.Utilties
{
    public static class SafeString
    {
        public static string ToSafeString(this object obj)
        {
            return (obj ?? string.Empty).ToString();
        }
    }
}