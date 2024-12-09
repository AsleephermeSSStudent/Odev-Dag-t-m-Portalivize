namespace OdevDagitim.ViewModel
{
    public class StudentAssignmentViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string TeacherName { get; set; }
        public bool IsSubmitted { get; set; }
        public bool IsLate { get; set; }
        public DateTime? SubmissionDate { get; set; }
    }
} 