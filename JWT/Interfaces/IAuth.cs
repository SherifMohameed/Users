namespace JWT.Interfaces
{
    public interface IAuth
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
        Task<RegisterResponse> GetTokenAsync(TokenRequest request);

    }
}
