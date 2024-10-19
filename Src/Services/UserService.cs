using Dapper;
using Microsoft.Data.SqlClient;
using RestApi.Src.Config;
using RestApi.Src.Dto;
using RestApi.Src.Validations;
using RestApi.Src.Validations.Cmd;

namespace RestApi.Src.Services
{
    public class UserService
    {
        private readonly Secret secret;
        private readonly string dbString;

        public UserService(IConfiguration _config)
        {
            secret = new(_config);
            dbString = secret.GetDbString();
        }

        public async Task<int> CreateUserAsync(RegisterCmd req)
        {
            string sql = "exec sp_insert_user @Username, @Email, @Passwd;";
            using var connection = new SqlConnection(dbString);
            int userId = await connection.ExecuteScalarAsync<int>(sql, req);

            return userId;
        }

        public async Task<LoginDto> GetUserForLogin(LoginCmd req)
        {
            string sql = "exec sp_get_user_for_login @Email;";
            using var connection = new SqlConnection(dbString);
            var result = await connection.QuerySingleOrDefaultAsync<LoginDto>(
                sql,
                new { Email = req.Email }
            );

            if (result is null)
            {
                throw new UnauthorizedAccessException(string.Empty);
            }
            return result;
        }
    }
}
