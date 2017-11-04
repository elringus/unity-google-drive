using UnityEngine;

public class AuthProviderYeildInstruction : CustomYieldInstruction
{
    public override bool keepWaiting { get { return !AuthProvider.IsDone; } }
    public IAuthProvider AuthProvider { get; private set; }

    public AuthProviderYeildInstruction (IAuthProvider authProvider)
    {
        AuthProvider = authProvider;
    }
}
