namespace RestApi.Src.Dto
{
    public class LoginDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Passwd { get; set; } = string.Empty;
    }
}
