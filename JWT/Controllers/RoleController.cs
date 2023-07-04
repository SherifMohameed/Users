namespace JWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRole _role;

        public RoleController(IRole role)
        {
            _role = role;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddRoles")]
        public async Task<IActionResult> AddRolesAsync(AddRoleRequest roleRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                string response = await _role.AddRoleAsync(roleRequest);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + " " + ex.StackTrace);

            }
        }
    }
}
