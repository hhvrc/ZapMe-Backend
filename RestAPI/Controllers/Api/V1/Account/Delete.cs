using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using ZapMe.Attributes;
using ZapMe.Database.Models;
using ZapMe.Helpers;

namespace ZapMe.Controllers.Api.V1;

public partial class AccountController
{
    /// <summary>
    /// Delete currently logged in account
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Empty</response>
    [RequestSizeLimit(1024)]
    [HttpDelete(Name = "DeleteAccount")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(
        [FromHeader][Password(true)] string password,
        [FromHeader][StringLength(1024)] string? reason,
        CancellationToken cancellationToken
        )
    {
        var user = await User.VerifyUserPasswordAsync(password, _dbContext, cancellationToken);
        if (user is null)
        {
            return HttpErrors.UnauthorizedActionResult;
        }

        using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        await _dbContext.Users
            .Where(u => u.Id == user.Id)
            .ExecuteDeleteAsync(cancellationToken);

        _dbContext.DeletedUsers.Add(new DeletedUserEntity
        {
            Id = user.Id,
            DeletedBy = user.Id,
            DeletionReason = reason,
            UserCreatedAt = user.CreatedAt,
        });

        await _dbContext.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return Ok();
    }
}