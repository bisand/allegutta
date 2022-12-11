namespace AlleGutta.Api;

public static class StringExtensions
{
    public static bool StartsWith(this string str, IEnumerable<string> conditions)
    {
        foreach (var item in conditions)
        {
            if (str.StartsWith(item))
                return true;
        }
        return false;
    }
}