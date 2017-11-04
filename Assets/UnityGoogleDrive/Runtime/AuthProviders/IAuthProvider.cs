using System;

public interface IAuthProvider
{
    event Action<IAuthProvider> OnDone;

    bool IsDone { get; }
    bool IsError { get; }
    string AccessToken { get; }
    string RefreshToken { get; }

    AuthProviderYeildInstruction ProvideAuth (AuthCredentials authCredentials);
}
