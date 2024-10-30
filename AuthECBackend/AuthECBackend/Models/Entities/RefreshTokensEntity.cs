namespace AuthECBackend.Models.Entities
{
    public class RefreshTokensEntity
    {
        public int? Id { get; set; }
        public required string RefreshToken { get; set; }
        public required bool IsUsed { get; set; }
    }
}
