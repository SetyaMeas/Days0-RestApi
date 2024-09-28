namespace RestApi.Src.Config
{
    public class Secret
    {
        private readonly IConfiguration _config;

        public Secret(IConfiguration config)
        {
            _config = config;
        }

        public string GetDbString()
        {
            return _config["DbConfig:DbString"] ?? string.Empty;
        }
        public string GetJwtSecretToken() {
            return _config["JwtConfig:SecretToken"] ?? string.Empty;
        }
    }
}
