namespace RestApi.Src.Dto
{
    public class TaskDto
    {
        public int TaskId { get; set; }
        public string Task { get; set; } = string.Empty;
        public int UserId { get; set; }
        public DateTime StartedDate { get; set; } 
        public DateTime CurrentDate { get; set; } 
        public int TotalDays { get; set; } = 0;
        public int TotalHours { get; set; } = 0;
        public int TotalMinutes { get; set; } = 0;
        public int TotalSeconds { get; set; } = 0;
    }
}

