// Policies/Requirements/XpRequirement

using Microsoft.AspNetCore.Authorization;

public class XpRequirement : IAuthorizationRequirement
{
    public XpRequirement(int xp)
    {
        Xp = xp;
    }

    public int Xp { get; }
}
