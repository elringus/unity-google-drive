using System.Collections.Generic;

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

    public string GenerateRequestPayload ()
    {
        var queryParameters = new List<string>();

        if (!string.IsNullOrEmpty(Alt))
            queryParameters.Add(string.Format("alt={0}", Alt));
        if (Fields != null && Fields.Count > 0)
            queryParameters.Add(string.Format("fields={0}", string.Join("%2C", Fields.ToArray())));
        if (PrettyPrint.HasValue)
            queryParameters.Add(string.Format("prettyPrint={0}", PrettyPrint.Value ? "true" : "false"));
        if (!string.IsNullOrEmpty(QuotaUser))
            queryParameters.Add(string.Format("quotaUser={0}", QuotaUser));
        if (!string.IsNullOrEmpty(UserIp))
            queryParameters.Add(string.Format("userIp={0}", UserIp));

        AddQueryParameters(ref queryParameters);

        return queryParameters.Count > 0 ? string.Format("?{0}", string.Join("&", queryParameters.ToArray())) : string.Empty;
    }

    protected virtual void AddQueryParameters (ref List<string> queryParameters) { }
}
