namespace ShopMaster.Areas.FrontEnd.ViewModels
{
    public class EditProfileViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateOnly? Birthday { get; set; }
        public string? Avatar { get; set; } // 用於顯示當前的頭像
        public IFormFile? AvatarFile { get; set; } // 用於上傳新的頭像
    }
}
