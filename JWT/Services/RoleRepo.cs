namespace JWT.Services
{
    public class RoleRepo : IRole
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleRepo(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<string> AddRoleAsync(AddRoleRequest roleRequest)
        {
            try
            {
                ApplicationUser user = await _userManager.FindByIdAsync(roleRequest.UserID);

                if (user == null)
                    return "Invalid User ID";

                foreach (string role in roleRequest.Roles)
                {
                    bool res = await _roleManager.RoleExistsAsync(role);

                    if (res)
                        return "Invalid Role Name";
                };

                roleRequest.Roles.ForEach(async role =>
                {
                    bool validIsAssaient = await _userManager.IsInRoleAsync(user, role);

                    if (validIsAssaient)
                        roleRequest.Roles.Remove(role);
                    else
                        await _userManager.AddToRoleAsync(user, role);
                });

                return "Roles Added Successfully";
            }
            catch (Exception ex)
            {
                return ex.Message + " " + ex.StackTrace;
            }
        }
    }
}
