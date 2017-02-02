namespace NancyMusicStore.Models
{
    public class LogOnModel
    {
        public string SysUserName { get; set; }
        public string SysUserPassword { get; set; }
        public string ReturnUrl { get; set; }        
    }

    public class RegisterModel
    {        
        public string SysUserId { get; set; }

        public string SysUserName { get; set; }
        
        public string SysUserEmail { get; set; }

        public string SysUserPassword { get; set; }
        
        public string ConfirmPassword { get; set; }
    }

    public class SysUser
    {
        public string SysUserId { get; set; }
        public string SysUserName { get; set; }
        public string SysUserPassword { get; set; }
        public string SysUserEmail { get; set; }       
    }
}
