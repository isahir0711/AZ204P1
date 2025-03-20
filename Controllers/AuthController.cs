using System.Net.WebSockets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project1.DTOs;
using Project1.Services;

namespace Project1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public ActionResult AuthCode()
        {
            string authCodeURI = _authService.GetAuthCodeURI();
            return Ok(authCodeURI);
        }
        [HttpPost]
        [Route("{authorization_code}")]
        public async Task<ActionResult> SignIn(string authorization_code)
        {
            var res = await _authService.GetBearerToken(authorization_code);

            if (!res.IsSuccess)
                return BadRequest(res.ErrorMessage);

            return Ok(res.Value);
        }
    }
}
