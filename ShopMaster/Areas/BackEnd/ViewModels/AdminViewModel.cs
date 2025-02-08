namespace ShopMaster.Areas.BackEnd.ViewModels
{
    public class AdminViewModel
    {
        public long? Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Password { get; set; }
        public int GroupId { get; set; }
    }
}
