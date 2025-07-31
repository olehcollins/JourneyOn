using Application.Models;
using Infrastructure.Identity;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IAccessTokenService tokenService
    ) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("sign-in")]
    [ProducesResponseType(typeof(ResponseModel<Dictionary<string,string?>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseModel<string>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SignIn([FromBody] LoginModel model)
    {
        var user = await userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return Unauthorized(new ResponseModel<string>(null, "User non existent"));
        }

        var result = await signInManager.PasswordSignInAsync(
            model.Email,
            model.Password,
            isPersistent: false,
            lockoutOnFailure: false
        );

        if (!result.Succeeded)
        {
            return Unauthorized(new ResponseModel<string>(null, "Invalid login credentials"));
        }

        var newTokens = await tokenService.GenerateTokensAsync(user);

        return Ok(
            new ResponseModel<Dictionary<string, string?>>(
                newTokens, "Successfully sign in."));
    }

    [HttpGet("sign-out/{userId}")]
    public async Task<ActionResult<ResponseModel<string>>> SignOut(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);

        await Task.WhenAll(
            userManager.RemoveAuthenticationTokenAsync(user!, "No Provider", "RefreshToken"),
            signInManager.SignOutAsync());

        return Ok(new ResponseModel<string>(null, "Successfully sign out."));
    }

    [AllowAnonymous]
    [HttpGet("confirm-email")]
    public ActionResult<ResponseModel<string>> ConfirmEmail()
        => Ok(new ResponseModel<string>(null, "Email confirmed"));

    [AllowAnonymous]
    [HttpGet("get-reset-token/{email}")]
    public async Task<ActionResult<ResponseModel<string>>> GetResetToken(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return BadRequest(new ResponseModel<string>(null, "User non existent"));
        }

        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);

        return Ok(new ResponseModel<string>(resetToken, "Reset password."));
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<ActionResult<ResponseModel<string>>> ResetPassword([FromBody] PasswordResetDetails details)
    {
        var user = await userManager.FindByEmailAsync(details.Email);
        var resetResult = await userManager.ResetPasswordAsync(user, details.Token, details.Password);

        return Ok(new ResponseModel<string>(null, "Reset password successful"));
    }

    [AllowAnonymous]
    [HttpGet("health")]
    public async Task<ActionResult> Health() => Ok();
}