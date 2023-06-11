﻿using System.Security.Claims;
using ZapMe.Database.Models;

namespace ZapMe.Authentication;

public sealed class ZapMePrincipal : ClaimsPrincipal
{
    public ZapMePrincipal(SessionEntity session) : base(new ZapMeIdentity(session))
    {
    }

    public new ZapMeIdentity Identity => (base.Identity as ZapMeIdentity)!;
}