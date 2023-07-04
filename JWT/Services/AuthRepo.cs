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
                if (_userManager.FindByEmailAsync(request.Email).Result is not null)
                        return new() { Message = "There is alredy user have this mail" };

                if(_userManager.FindByNameAsync(request.UserName).Result is not null )
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
                        errors += $"{error.Description} \n";
                    });

                    errors.Remove(errors.Length - 2);

                    return new() { Message = errors };
                }

                // add roles 
                await _userManager.AddToRoleAsync(user, "User");

                JwtSecurityToken jwtToken = await GenerateUserToken(user);


                return new()
                {
                    Email = user.Email,
                    ExpiresOn = jwtToken.ValidTo,
                    IsAuthenticated = true,
                    Roles = new() { "User"},
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    UserName = user.UserName,
                    Message = "Register Done Successfully"
                };
            }
            catch (Exception ex)
			{
                return new() { Message = ex.Message + " " + ex.StackTrace };
			}
        }

        public async Task<RegisterResponse> GetTokenAsync(TokenRequest request)
        {
            try
            {
                RegisterResponse response = new();

                ApplicationUser user = await _userManager.FindByNameAsync(request.UserName);

                if(user != null || await _userManager.CheckPasswordAsync(user, request.Password))
                {
                    response.Message = "Invalid User Name or Password";
                    return response;
                }

                JwtSecurityToken jwtToken = await GenerateUserToken(user);
                IList<string> userRoles = await _userManager.GetRolesAsync(user);

                response.Email = user.Email;
                response.ExpiresOn = jwtToken.ValidTo;
                response.IsAuthenticated = true;
                response.Roles = userRoles.ToList();
                response.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
                response.UserName = user.UserName;
                response.Message = "Register Done Successfully";

                return response;

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
