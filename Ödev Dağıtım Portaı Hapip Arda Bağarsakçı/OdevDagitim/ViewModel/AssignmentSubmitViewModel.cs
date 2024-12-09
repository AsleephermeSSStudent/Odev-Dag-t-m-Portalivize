namespace OdevDagitim.ViewModel
{
    public class AssignmentSubmitViewModel
    {
        public int AssignmentId { get; set; }
        public string? AssignmentTitle { get; set; }
        public DateTime DueDate { get; set; }
        public string? Description { get; set; }
        public IFormFile File { get; set; }
    }
} 