namespace OdevDagitim.ViewModel
{
    public class UserModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } // "Free" veya "Admin"
        public double StorageLimit { get; set; } // GB olarak depolama limiti
        public double UsedStorage { get; set; } // Kullanılmış depolama miktarı
        public DateTime CreatedAt { get; set; }
    }
}
