using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZapMe.BusinessLogic.Users;
using ZapMe.DTOs;

namespace ZapMe.Controllers.Api.V1;

public partial class AccountController
{
    [HttpGet(Name = "AccountGet")]
    public Task<AccountDto> Get(CancellationToken cancellationToken) => UserFetchingLogic.FetchAccountDto_ById(_dbContext, User.GetUserId(), cancellationToken);
}