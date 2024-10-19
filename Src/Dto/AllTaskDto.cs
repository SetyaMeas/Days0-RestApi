namespace RestApi.Src.Dto
{
    public class AllTaskDto
    {
        public int UserId { get; set; }
        public int TaskId { get; set; }
        public string Task { get; set; } = string.Empty;
        public DateTime StartedDate { get; set; }
    }
}
