using System.Text;

namespace JWT.Services
{
    public class AuthRepo : IAuth
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthRepo(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
			try
			{
                if (_userManager.FindByEmailAsync(request.Email) is not null)
                    return new() { Message = "There is alredy user have this mail" };

                if(_userManager.FindByNameAsync(request.UserName) is not null )
                    return new() { Message = "There is alredy user have this user name" };

                if(request.Password != request.ConfirmPassword)
                    return new() { Message = "Wrong Password" };

                ApplicationUser user = AutoMapperConfiguration.Configure().Map<ApplicationUser>(request);
                
                IdentityResult result = await _userManager.CreateAsync(user, request.Password);   

                if(!result.Succeeded) 
                {
                    string errors = string.Empty;

                    result.Errors.ToList().ForEach(error =>
                    {
                        errors += $"{error}, ";
                    });

                    errors.Remove(errors.Length - 2);

                    return new() { Message = errors };
                }

                // add roles 

                return new();
            }
            catch (Exception ex)
			{
                return new() { Message = ex.Message + " " + ex.StackTrace };
			}
        }


        private async Task<JwtSecurityToken> GenerateUserToken(ApplicationUser user)
        {
            JWTConfiguration jWT = JWTConfiguration.GetInstance();


            IList<Claim> userClams = await _userManager.GetClaimsAsync(user);
            IList<string> roles = await _userManager.GetRolesAsync(user);
            List<Claim> roleClaims = new ();

            roles.ToList().ForEach(role =>
            {
                roleClaims.Add(new("roles", role));
            });

            IEnumerable<Claim> clams = new []
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName)
            }
            .Union(userClams)
            .Union(roleClaims);
            SymmetricSecurityKey SymmetricSecurityCode = new (Encoding.UTF8.GetBytes(jWT.Key));
            SigningCredentials signingCredentials = new(SymmetricSecurityCode, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken jwtSecurityToken = new(
                issuer: jWT.Issuer,
                audience: jWT.Audience,
                claims: clams,
                expires: DateTime.Now.AddDays(jWT.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
    }
}
