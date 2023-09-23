using CupsellCloneAPI.Authentication.Models;
using CupsellCloneAPI.Authentication.Services;
using Microsoft.AspNetCore.Mvc;

namespace CupsellCloneAPI.Controllers
{
    [ApiController]
    [Route("cupsellclone/account")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("register")]
        public ActionResult Register([FromBody] RegisterUserDto dto)
        {
            _accountService.RegisterUser(dto);
            return Ok();
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginUserDto dto)
        {
            var token = _accountService.GenerateJwt(dto);
            return Ok(token);
        }

        // [HttpPost("refresh")]
        // public ActionResult Refresh(string refreshToken)
        // {
        //     var isValidToken  = _accountService.
        // }
    }
}