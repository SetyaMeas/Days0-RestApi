using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestApi.Src.Dto;
using RestApi.Src.Services;
using RestApi.Src.Validations.Cmd;

namespace RestApi.Src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly TaskService taskService;
        private readonly IMediator _mediator;

        public TaskController(IConfiguration config, IMediator mediator)
        {
            taskService = new(config);
            _mediator = mediator;
        }

        private static JwtClaimDto GetUserClaim(HttpContext context)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = context.User.FindFirstValue(ClaimTypes.Email);
            if (userId is null || email is null)
            {
                throw new UnauthorizedAccessException(string.Empty);
            }

            return new JwtClaimDto { UserId = int.Parse(userId), Email = email };
        }

        [Authorize]
        [HttpPost]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskCmd req)
        {
            await _mediator.Send(req);

            JwtClaimDto user = GetUserClaim(HttpContext);
            var totalTask = await taskService.CountTaskByUserIdAsync(user.UserId);
            if (totalTask >= 15)
            {
                return StatusCode(StatusCodes.Status429TooManyRequests, "Task limit is 15");
            }

            var task = await taskService.CreateTaskAsync(req, user.UserId);
            return Ok(task);
        }

        [Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllTask()
        {
            var user = GetUserClaim(HttpContext);
            var tasks = await taskService.GetAllAsync(user.UserId);
            return Ok(tasks);
        }

        [Authorize]
        [HttpGet("detail/{taskId}")]
        public async Task<IActionResult> GetTaskDetail(int taskId)
        {
            var user = GetUserClaim(HttpContext);
            var task = await taskService.GetTaskDetailAsync(taskId, user.UserId);
            if (task is null)
            {
                return NotFound(new { message = "task not found" });
            }
            return Ok(task);
        }

        [Authorize]
        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask(int taskId)
        {
            var user = GetUserClaim(HttpContext);
            var deletedTask = await taskService.DeleteTaskAsync(taskId, user.UserId);
            if (deletedTask)
            {
                return Ok();
            }
            return NotFound(new { message = "task not found" });
        }

        // TODO: let's get to front-end: login, logout, create task, show task, delete
        // TODO: after that, create forgot passsword, change password and username
    }
}
