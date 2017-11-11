using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Query parameters that apply to all Google Drive API methods.
/// Prototype: https://developers.google.com/drive/v3/web/query-parameters.
/// </summary>
public class GoogleDriveQueryParameters 
{
    public const string ALT_MEDIA_VALUE = "media";
    public const string ALT_JSON_VALUE = "json";

    /// <summary>
    /// Used to alternate between returned content types.
    /// </summary>
    public string Alt { get; set; }
    /// <summary>
    /// Selector specifying a subset of fields to include in the response.
    /// </summary>
    public List<string> Fields { get; set; }
    /// <summary>
    /// Returns response with indentations and line breaks.
    /// </summary>
    public bool? PrettyPrint { get; set; }
    /// <summary>
    /// Allows to enforce per-user quotas from a server-side application even 
    /// in cases when the user's IP address is unknown. 
    /// </summary>
    public string QuotaUser { get; set; }
    /// <summary>
    /// IP address of the end user for whom the API call is being made.
    /// </summary>
    public string UserIp { get; set; }

    /// <summary>
    /// Generates an HTML request query string using declared properties.
    /// </summary>
    public string ToQueryString ()
    {
        // Get all properties on the object.
        var properties = GetType().GetProperties()
            .Where(property => property.CanRead)
            .Where(property => property.GetValue(this, null) != null)
            .ToDictionary(property => 
            // Convert first letter of the property name to lower case.
            Char.ToLowerInvariant(property.Name[0]) + property.Name.Substring(1), 
            property => property.GetValue(this, null));

        // Get names for all IEnumerable properties (excl. string).
        var propertyNames = properties
            .Where(property => !(property.Value is string) && property.Value is IEnumerable)
            .Select(property => property.Key).ToList();

        // Concat all IEnumerable properties into a comma separated string.
        foreach (var propertyName in propertyNames)
        {
            var valueType = properties[propertyName].GetType();
            var valueElemType = valueType.IsGenericType
                                    ? valueType.GetGenericArguments()[0]
                                    : valueType.GetElementType();
            if (valueElemType.IsPrimitive || valueElemType == typeof(string))
            {
                var enumerable = properties[propertyName] as IEnumerable;
                properties[propertyName] = string.Join(",", enumerable.Cast<string>().ToArray());
            }
        }

        // Concat all key/value pairs into a string separated by ampersand.
        return string.Join("&", properties.Select(x => string.Concat(
            Uri.EscapeDataString(x.Key), "=", 
            Uri.EscapeDataString(x.Value.ToString()))).ToArray());
    }
}
