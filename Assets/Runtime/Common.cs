using System;

internal static class Common
{
    /// <summary>
    /// Attempts to extract content before the specified match (on first occurence).
    /// </summary>
    public static string GetBefore (this string content, string matchString, StringComparison comp = StringComparison.Ordinal)
    {
        if (content.Contains(matchString))
        {
            var endIndex = content.IndexOf(matchString, comp);
            return content.Substring(0, endIndex);
        }
        return null;
    }



    /// <summary>
    /// Attempts to extract content after the specified match (on first occurence).
    /// </summary>
    public static string GetAfterFirst (this string content, string matchString, StringComparison comp = StringComparison.Ordinal)
    {
        if (content.Contains(matchString))
        {
            var startIndex = content.IndexOf(matchString, comp) + matchString.Length;
            if (content.Length <= startIndex) return string.Empty;
            return content.Substring(startIndex);
        }
        return null;
    }
}
