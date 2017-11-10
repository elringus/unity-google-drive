using System.Collections.Generic;

/// <summary>
/// Query parameters that apply to all Google Drive API operations.
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

    public string GenerateRequestPayload ()
    {
        var queryParams = new List<string>();

        if (!string.IsNullOrEmpty(Alt))
            queryParams.Add(string.Format("alt={0}", Alt));
        if (Fields != null && Fields.Count > 0)
            queryParams.Add(string.Format("fields={0}", string.Join("%2C", Fields.ToArray())));
        if (PrettyPrint.HasValue)
            queryParams.Add(string.Format("prettyPrint={0}", PrettyPrint.Value ? "true" : "false"));
        if (!string.IsNullOrEmpty(QuotaUser))
            queryParams.Add(string.Format("quotaUser={0}", QuotaUser));
        if (!string.IsNullOrEmpty(UserIp))
            queryParams.Add(string.Format("userIp={0}", UserIp));

        return queryParams.Count > 0 ? string.Format("?{0}", string.Join("&", queryParams.ToArray())) : string.Empty;
    }
}
