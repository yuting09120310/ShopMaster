namespace ShopMaster.Areas.BackEnd.ViewModels
{
    public class AdminGroupPermissionViewModel
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public List<MenuPermission> MenuPermissions { get; set; }
    }

    public class MenuPermission
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public string MenuGroupName { get; set; } // ➡️ 新增：大類名稱
        public bool CanCreate { get; set; }
        public bool CanRead { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }
}
