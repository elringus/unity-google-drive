namespace UnityGoogleDrive
{
    public interface IClientCredentials
    {
        string AuthUri { get; }
        string TokenUri { get; }
        string ClientId { get; }
        string ClientSecret { get; }
    }
}
