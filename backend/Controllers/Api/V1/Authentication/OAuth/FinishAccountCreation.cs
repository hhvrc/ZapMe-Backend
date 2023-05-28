using Microsoft.AspNetCore.Mvc;
using ZapMe.Attributes;
using ZapMe.Authentication.Models;
using ZapMe.Controllers.Api.V1.Authentication.OAuth.Models;

namespace ZapMe.Controllers.Api.V1;

public partial class AuthController
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="body"></param>
    /// <param name="cancellationToken"></param>
    /// <status code="200"></status>
    [AnonymousOnly]
    [RequestSizeLimit(1024)]
    [HttpPost("o/create", Name = "OAuth Create Account")]
    [ProducesResponseType(typeof(SignInOk), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    public async Task<IActionResult> OAuthFinishAccountCreation([FromBody] OAuthFinishAccountCreation body, CancellationToken cancellationToken)
    {
        // TODO: Implement OAuthFinishAccountCreation
        await Task.CompletedTask;
        throw new NotImplementedException();
    }
}