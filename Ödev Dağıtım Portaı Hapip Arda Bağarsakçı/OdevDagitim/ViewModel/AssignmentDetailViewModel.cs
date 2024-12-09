namespace OdevDagitim.ViewModel
{
    public class AssignmentDetailViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string TeacherName { get; set; }
        public string ClassName { get; set; }
        public string FilePath { get; set; }
        public List<SubmissionDetailViewModel> Submissions { get; set; }
        public SubmissionDetailViewModel UserSubmission { get; set; }
    }

    public class SubmissionDetailViewModel
    {
        public int Id { get; set; }
        public string StudentName { get; set; }
        public DateTime SubmissionDate { get; set; }
        public bool IsLate { get; set; }
        public string Description { get; set; }
        public string SubmissionPath { get; set; }
    }
} 