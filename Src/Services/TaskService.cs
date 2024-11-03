using Dapper;
using Microsoft.Data.SqlClient;
using RestApi.Src.Config;
using RestApi.Src.Dto;
using RestApi.Src.Validations.Cmd;

namespace RestApi.Src.Services
{
    public class TaskService
    {
        private readonly Secret secret;
        private readonly string DbString;

        public TaskService(IConfiguration config)
        {
            secret = new(config);
            DbString = secret.GetDbString();
        }

        public async Task<int> CreateTaskAsync(CreateTaskCmd req, int userId)
        {
            string sql = "exec sp_create_task @UserId, @Task, @StartedDate;";
            DateTime StartedDate = DateTime.UtcNow;
            using var connection = new SqlConnection(DbString);

            int taskId = await connection.ExecuteScalarAsync<int>(
                sql,
                new
                {
                    Task = req.Task,
                    UserId = userId,
                    StartedDate,
                }
            );
            return taskId;
        }

        public async Task<TaskDto?> GetTaskDetailAsync(int taskId, int userId)
        {
            string sql = "exec sp_get_task_detail @TaskId, @UserId;";
            using var connection = new SqlConnection(DbString);
            var task = await connection.QueryFirstOrDefaultAsync<TaskDto>(
                sql,
                new { TaskId = taskId, UserId = userId }
            );

            if (task is null)
            {
                return null;
            }

            DateTime currentDate = DateTime.UtcNow;
            var calculatedDate = currentDate.Subtract(task.StartedDate);

            task.CurrentDate = currentDate;
            task.TotalDays = calculatedDate.Days;
            task.TotalHours = calculatedDate.Hours;
            task.TotalMinutes = calculatedDate.Minutes;
            task.TotalSeconds = calculatedDate.Seconds;

            return task;
        }

        public async Task<bool> DeleteTaskAsync(int taskId, int userId)
        {
            string sql = "exec sp_delete_task @UserId, @TaskId;";
            using var connection = new SqlConnection(DbString);
            var task = await connection.ExecuteReaderAsync(
                sql,
                new { TaskId = taskId, UserId = userId }
            );
            return task.RecordsAffected == 1;
        }

        public async Task<List<AllTaskDto>> GetAllAsync(int userId)
        {
            string sql = "exec sp_get_all_byUserId @userId;";
            using var con = new SqlConnection(DbString);
            var tasks = (await con.QueryAsync<AllTaskDto>(sql, new { userId })).ToList();
            return tasks;
        }

        public async Task<int> CountTaskByUserIdAsync(int userId)
        {
            string sql = "exec sp_Count_Task_By_UserId @UserId;";
            using var con = new SqlConnection(DbString);
            int totalTasks = await con.ExecuteScalarAsync<int>(sql, new { UserId = userId });
            return totalTasks;
        }
    }
}
