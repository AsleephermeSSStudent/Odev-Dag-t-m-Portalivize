namespace OdevDagitim.Models
{
    public class User : BaseEntity
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public int? ClassId { get; set; }
        public Class Class { get; set; }
        public ICollection<Assignment> TeacherAssignments { get; set; }
        public ICollection<AssignmentSubmission> Submissions { get; set; }
    }
}
