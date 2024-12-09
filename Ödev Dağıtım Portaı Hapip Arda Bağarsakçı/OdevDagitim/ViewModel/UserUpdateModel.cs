namespace OdevDagitim.ViewModel
{
    public class UserUpdateModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } // "Free" veya "Admin"
        public double StorageLimit { get; set; } = 5 * 1024; // GB olarak depolama limiti
    }
}
