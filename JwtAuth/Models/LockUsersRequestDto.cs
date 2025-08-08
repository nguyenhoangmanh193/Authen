namespace JwtAuth.Models
{
    public class LockUsersRequestDto
    {
        public List<Guid> UserIds { get; set; } = new();
    }
}
