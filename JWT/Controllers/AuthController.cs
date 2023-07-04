namespace JWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuth _authServices;

        public AuthController(IAuth authServices)
        {
            _authServices = authServices;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync(RegisterRequest regestModerl)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                RegisterResponse register = await _authServices.RegisterAsync(regestModerl);

                if(register == null || !register.IsAuthenticated)
                    return BadRequest(register?.Message);

                return Ok(register);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + " " + ex.StackTrace);
            }
        }

        [Authorize]
        [HttpPost("GetToken")]
        public async Task<IActionResult> GetTokenAsync(TokenRequest regestModerl)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                RegisterResponse register = await _authServices.GetTokenAsync(regestModerl);

                if (register == null || !register.IsAuthenticated)
                    return BadRequest(register?.Message);

                return Ok(register);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + " " + ex.StackTrace);
            }
        }
    }
}
