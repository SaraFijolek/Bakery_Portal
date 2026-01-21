using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using WebApplication2.Backery.Services.Services.Interfaces;
using WebApplication2.Properties.DTOs;
using WebApplication2.Properties.Models;

namespace WebApplication2.Backery.API.Controllers;


[Authorize]
[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IJwtTokenService _tokenService;

    public AccountController(IJwtTokenService tokenService,
        UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult> LoginAsync([FromBody] LoginDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
        {
            return BadRequest();
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!passwordValid)
        {
            return BadRequest();
        }

        if (await _userManager.GetTwoFactorEnabledAsync(user))
        {
            var code = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);

            var content = $"Nowe logowanie. Token uwierzytelniajacy - {code}";
            //// wysylka mailem
            //var smtpClient = new SmtpClient("dsadsa@gmail.com", "dsadsa");
            //smtpClient.Send("dsadsa@gmail.com", user.Email, $"Logowanie {DateTime.UtcNow.ToString()}", content);

            return Ok(new { requires2FA = true });
        }

        var token = _tokenService.CreateToken(user.Id);
        return Ok(new { access_token = token });
    }

    [AllowAnonymous]
    [HttpPost("login2fa")]
    public async Task<ActionResult> Login2FAAsync([FromBody] Login2FADto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
        {
            return BadRequest();
        }

        var valid = await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider, model.Code2FA);
        if (!valid)
        {
            return BadRequest();
        }

        var token = _tokenService.CreateToken(user.Id);
        return Ok(new { access_token = token });
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult> RegisterAsync([FromBody] RegisterDto model)
    {
        var user = new User
        {
            Email = model.Email,
            UserName = model.Username,
            TwoFactorEnabled = true
        };

        if (model.Password != model.ConfirmPassword)
        {
            return BadRequest();
        }

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var content = $"Nowe logowanie. Token = {token}, userId= {user.Id}";
        //// wysylka mailem
        //var smtpClient = new SmtpClient("dsadsa@gmail.com", "dsadsa");
        //smtpClient.Send("dsadsa@gmail.com", user.Email, $"Logowanie {DateTime.UtcNow.ToString()}", content);

        return Ok();
    }

    [HttpPost("confirm-account")]
    public async Task<IActionResult> CofirmEmail(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return BadRequest();
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded)
        {
            return BadRequest();
        }

        return Ok();
    }
}
