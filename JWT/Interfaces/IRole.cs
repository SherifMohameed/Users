namespace JWT.Interfaces
{
    public interface IRole
    {
        Task<string> AddRoleAsync(AddRoleRequest roleRequest);
    }
}
