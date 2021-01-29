using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;

namespace UnityCommon
{
    /// <summary>
    /// Provides various helper and extension methods for <see cref="string"/> objects.
    /// </summary>
    public static class StringUtils
    {
        /// <summary>
        /// Characters used to represent new lines, cross-platform (Windows-Mac-Unix).
        /// </summary>
        public static readonly char[] NewLineChars = { '\n', '\r' };
        /// <summary>
        /// Character combinations used to represent new lines, cross-platform (Windows-Mac-Unix).
        /// </summary>
        public static readonly string[] NewLineSymbols = { "\r\n", "\n", "\r" };
        
        /// <summary>
        /// Checks whether provided string contains any line break characters (platform-agnostic).
        /// </summary>
        public static bool ContainsLineBreak (this string content)
        {
            if (content is null) throw new ArgumentNullException(nameof(content));
            return content.IndexOfAny(NewLineChars) >= 0;
        }
        
        /// <summary>
        /// Performs <see cref="string.Equals(string, string, StringComparison)"/> with <see cref="StringComparison.Ordinal"/>.
        /// </summary>
        public static bool EqualsFast (this string content, string comparedString)
        {
            return content.Equals(comparedString, StringComparison.Ordinal);
        }

        /// <summary>
        /// Performs <see cref="string.Equals(string, string, StringComparison)"/> with <see cref="StringComparison.OrdinalIgnoreCase"/>.
        /// </summary>
        public static bool EqualsFastIgnoreCase (this string content, string comparedString)
        {
            return content.Equals(comparedString, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Performs <see cref="string.EndsWith(string, StringComparison)"/> with <see cref="StringComparison.Ordinal"/>.
        /// </summary>
        public static bool EndsWithFast (this string content, string match)
        {
            return content.EndsWith(match, StringComparison.Ordinal);
        }

        /// <summary>
        /// Performs <see cref="string.StartsWith(string, StringComparison)"/> with <see cref="StringComparison.Ordinal"/>.
        /// </summary>
        public static bool StartsWithFast (this string content, string match)
        {
            return content.StartsWith(match, StringComparison.Ordinal);
        }

        /// <summary>
        /// Performs <see cref="string.StartsWith(string)"/> and <see cref="string.EndsWith(string)"/> with the provided match.
        /// </summary>
        public static bool WrappedIn (this string content, string match, StringComparison comp = StringComparison.Ordinal)
        {
            return content.StartsWith(match, comp) && content.EndsWith(match, comp);
        }

        /// <summary>
        /// Attempts to extract a subset of the provided <paramref name="source"/> string, starting at
        /// <paramref name="startIndex"/> and ending at <paramref name="endIndex"/>; returns <see langword="null"/> on fail.
        /// </summary>
        /// <param name="source">The string to extract the subset from.</param>
        /// <param name="startIndex">Start index of the subset.</param>
        /// <param name="endIndex">End index of the subset.</param>
        /// <returns>The extracted subset string or <see langword="null"/> if failed.</returns>
        public static string TrySubset (string source, int startIndex, int endIndex)
        {
            if (string.IsNullOrWhiteSpace(source)) return null;
            if (startIndex < 0 || startIndex >= source.Length) return null;
            if (endIndex < 0 || endIndex >= source.Length) return null;
            if (endIndex - startIndex < 0) return null;

            var length = endIndex - startIndex + 1;
            return source.Substring(startIndex, length);
        }

        /// <summary>
        /// Attempts to extract content between the specified matches (on first occurence).
        /// </summary>
        public static string GetBetween (this string content, string startMatch, string endMatch, StringComparison comp = StringComparison.Ordinal)
        {
            if (content.Contains(startMatch) && content.Contains(endMatch))
            {
                var startIndex = content.IndexOf(startMatch, comp) + startMatch.Length;
                var endIndex = content.IndexOf(endMatch, startIndex, comp);
                return content.Substring(startIndex, endIndex - startIndex);
            }
            else return null;
        }

        /// <summary>
        /// Attempts to extract content wrapped in the specified match (on first occurence).
        /// </summary>
        public static string GetBetween (this string content, string match, StringComparison comp = StringComparison.Ordinal)
        {
            return content.GetBetween(match, match, comp);
        }

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
            else return null;
        }

        /// <summary>
        /// Attempts to extract content before the specified match (on last occurence).
        /// </summary>
        public static string GetBeforeLast (this string content, string matchString, StringComparison comp = StringComparison.Ordinal)
        {
            if (content.Contains(matchString))
            {
                var endIndex = content.LastIndexOf(matchString, comp);
                return content.Substring(0, endIndex);
            }
            else return null;
        }

        /// <summary>
        /// Attempts to extract content after the specified match (on last occurence).
        /// </summary>
        public static string GetAfter (this string content, string matchString, StringComparison comp = StringComparison.Ordinal)
        {
            if (content.Contains(matchString))
            {
                var startIndex = content.LastIndexOf(matchString, comp) + matchString.Length;
                if (content.Length <= startIndex) return string.Empty;
                return content.Substring(startIndex);
            }
            else return null;
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
            else return null;
        }

        /// <summary>
        /// Splits the string using new line symbol as a separator.
        /// Will split by all type of new lines, independent of environment.
        /// </summary>
        public static string[] SplitByNewLine (this string content, StringSplitOptions splitOptions = StringSplitOptions.None)
        {
            if (string.IsNullOrEmpty(content)) return null;
            
            return content.Split(NewLineSymbols, splitOptions);
        }

        /// <summary>
        /// Removes matching trailing string.
        /// </summary>
        public static string TrimEnd (this string source, string value)
        {
            if (!source.EndsWithFast(value))
                return source;

            return source.Remove(source.LastIndexOf(value, StringComparison.InvariantCulture));
        }
        
        /// <summary>
        /// Invokes <see cref="string.Replace(string,string)"/> with an empty string.
        /// </summary>
        public static string Remove (this string source, char value)
        {
            return source?.Replace(value.ToString(), string.Empty);
        }
        
        /// <inheritdoc cref="Remove(string,char)"/>
        public static string Remove (this string source, string value)
        {
            return source?.Replace(value, string.Empty);
        }

        /// <summary>
        /// Checks whether string is null, empty or consists of whitespace chars.
        /// </summary>
        public static bool IsNullEmptyOrWhiteSpace (string content)
        {
            if (string.IsNullOrEmpty(content))
                return true;

            return string.IsNullOrEmpty(content.TrimFull());
        }

        /// <summary>
        /// Performs <see cref="string.Trim()"/> additionally removing any BOM and other service symbols.
        /// </summary>
        public static string TrimFull (this string source)
        {
            #if UNITY_WEBGL // WebGL build under .NET 4.6 fails when using Trim with UTF-8 chars. (should be fixed in Unity 2018.1)
            var whitespaceChars = new System.Collections.Generic.List<char> {
                '\u0009','\u000A','\u000B','\u000C','\u000D','\u0020','\u0085','\u00A0',
                '\u1680','\u2000','\u2001','\u2002','\u2003','\u2004','\u2005','\u2006',
                '\u2007','\u2008','\u2009','\u200A','\u2028','\u2029','\u202F','\u205F',
                '\u3000','\uFEFF','\u200B',
            };

            // Trim start.
            if (string.IsNullOrEmpty(source)) return source;
            var c = source[0];
            while (whitespaceChars.Contains(c))
            {
                if (source.Length <= 1) return string.Empty;
                source = source.Substring(1);
                c = source[0];
            }

            // Trim end.
            if (string.IsNullOrEmpty(source)) return source;
            c = source[source.Length - 1];
            while (whitespaceChars.Contains(c))
            {
                if (source.Length <= 1) return string.Empty;
                source = source.Substring(0, source.Length - 1);
                c = source[source.Length - 1];
            }

            return source;
            #else
            return source.Trim().Trim('\uFEFF', '\u200B');
            #endif
        }

        /// <summary>
        /// Given a file size (length in bytes), produces a human-readable string.
        /// </summary>
        /// <param name="size">Bytes length of the file.</param>
        /// <param name="unit">Minimum unit to use: { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" }.</param>
        public static string FormatFileSize (double size, int unit = 0)
        {
            string[] units = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

            while (size >= 1024)
            {
                size /= 1024;
                ++unit;
            }

            return $"{size:G4} {units[unit]}";
        }

        /// <summary>
        /// Modifies the string by inserting provided char (space by default) based on camel case scheme; eg, `SomeFancyName` becomes `Some Fancy Name`.
        /// </summary>
        /// <param name="source">The source string to modify.</param>
        /// <param name="insert">The string to insert.</param>
        /// <param name="preserveAcronyms">Whether to account acronyms; eg when enabled `BBCChannel` will result in `BBC Channel`.</param>
        public static string InsertCamel (this string source, char insert = ' ', bool preserveAcronyms = true)
        {
            if (string.IsNullOrWhiteSpace(source) || source.Length < 2)
                return source;

            bool IsUpperOrNumber (char ch) => char.IsUpper(ch) || char.IsNumber(ch);

            var builder = new StringBuilder(source.Length * 2);

            builder.Append(source[0]);
            for (int i = 1; i < source.Length; i++)
            {
                if (IsUpperOrNumber(source[i]))
                {
                    if (source[i - 1] != insert && !IsUpperOrNumber(source[i - 1]) || (preserveAcronyms && IsUpperOrNumber(source[i - 1]) && i < source.Length - 1 && !IsUpperOrNumber(source[i + 1])))
                        builder.Append(insert);
                }
                builder.Append(source[i]);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Changes first character in the provided string to lower invariant.
        /// </summary>
        public static string FirstToLower (this string source)
        {
            if (string.IsNullOrEmpty(source) || char.IsLower(source, 0))
                return source;

            if (source.Length <= 1) 
                return source.ToLowerInvariant();
            
            return char.ToLowerInvariant(source[0]) + source.Substring(1);
        }
        
        /// <summary>
        /// Changes first character in the provided string to upper invariant.
        /// </summary>
        public static string FirstToUpper (this string source)
        {
            if (string.IsNullOrEmpty(source) || char.IsUpper(source, 0))
                return source;

            if (source.Length <= 1) 
                return source.ToUpperInvariant();
            
            return char.ToUpperInvariant(source[0]) + source.Substring(1);
        }

        public static byte[] ZipString (string content)
        {
            using (var output = new MemoryStream())
            {
                using (var gzip = new DeflateStream(output, CompressionMode.Compress))
                {
                    using (var writer = new StreamWriter(gzip, Encoding.UTF8))
                    {
                        writer.Write(content);
                    }
                }

                return output.ToArray();
            }
        }

        public static string UnzipString (byte[] content)
        {
            using (var inputStream = new MemoryStream(content))
            {
                using (var gzip = new DeflateStream(inputStream, CompressionMode.Decompress))
                {
                    using (var reader = new StreamReader(gzip, Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }
    }
}
