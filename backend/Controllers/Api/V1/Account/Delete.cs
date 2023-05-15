using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.ComponentModel.DataAnnotations;
using ZapMe.Attributes;
using ZapMe.Authentication;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Data.Models;
using ZapMe.Helpers;
using ZapMe.Utils;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class AccountController
{
    /// <summary>
    /// Delete currently logged in account
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Empty</response>
    /// <response code="400"></response>
    /// <response code="500"></response>
    [RequestSizeLimit(1024)]
    [HttpDelete(Name = "DeleteAccount")]
    [Produces(Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(
        [FromHeader][Password(true)] string password,
        [FromHeader][StringLength(1024)] string? reason,
        CancellationToken cancellationToken
        )
    {
        UserEntity user = (User as ZapMePrincipal)!.Identity.User;

        if (!PasswordUtils.CheckPassword(password, user.PasswordHash))
        {
            return CreateHttpError.InvalidPassword().ToActionResult();
        }

        using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        await _dbContext.Users
            .Where(u => u.Id == user.Id)
            .ExecuteDeleteAsync(cancellationToken);

        await _dbContext.DeletedUsers.AddAsync(new DeletedUserEntity
        {
            Id = user.Id,
            DeletedBy = user.Id,
            DeletionReason = reason,
            UserCreatedAt = user.CreatedAt,
            UserDeletedAt = DateTime.UtcNow,
        }, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return Ok();
    }
}