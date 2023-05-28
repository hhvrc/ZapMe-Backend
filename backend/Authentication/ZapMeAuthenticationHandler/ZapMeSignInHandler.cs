using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Json;
using ZapMe.Authentication.Models;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Data;
using ZapMe.Data.Models;
using ZapMe.Options;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;

namespace ZapMe.Authentication;

public partial class ZapMeAuthenticationHandler
{
    public Task ZapMeSignInAsync(ClaimsPrincipal claimsIdentity, AuthenticationProperties? properties)
    {
        return FinishSignInAsync((claimsIdentity as ZapMePrincipal)!.Identity.Session);
    }
}