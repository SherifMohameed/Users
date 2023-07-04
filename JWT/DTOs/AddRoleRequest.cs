namespace JWT.DTOs
{
    public class AddRoleRequest
    {
        public string UserID { get; set; }
        public List<string> Roles { get; set; }
    }
}
