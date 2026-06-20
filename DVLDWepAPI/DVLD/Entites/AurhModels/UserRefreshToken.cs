namespace Entites.AurhModels
{
    public class UserRefreshToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; } = null!;
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }
    }

    public record TokenRequestDto(string AccessToken, string RefreshToken);

}