using System;

public class RedirectAuthProvider : IAuthProvider
{
    public event Action<IAuthProvider> OnDone;

    public bool IsDone { get; private set; }
    public bool IsError { get; private set; }
    public string AccessToken { get; private set; }
    public string RefreshToken { get; private set; }

    public void ProvideAuth (AuthCredentials authCredentials)
    {
        if (OnDone != null) OnDone.Invoke(this);
        throw new NotImplementedException();
    }
}
