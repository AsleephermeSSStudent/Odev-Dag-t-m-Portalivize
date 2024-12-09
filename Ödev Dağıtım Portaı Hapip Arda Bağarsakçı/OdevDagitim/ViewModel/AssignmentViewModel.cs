namespace OdevDagitim.ViewModel
{
    public class AssignmentViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public int ClassId { get; set; }
        public int TeacherId { get; set; }
        
        // Navigation properties
        public string? ClassName { get; set; }  // Sınıf adı
        public string? TeacherName { get; set; } // Öğretmen adı
    }
} 