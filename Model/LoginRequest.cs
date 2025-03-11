namespace WebAPI_Code_First.Model
{
    public class LoginRequest
    {
        public string EmailOrContactNo { get; set; }
        public string Password { get; set; }
    }
    public class UserListModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDateTime { get; set; }
    }
    public class PasswordModel
    {
        public int Id { get; set; }
        //public string? OldPassword { get; set; }
        public string NewPassword { get; set; }
        public int? LastPasswordChangeCount { get; set; }
        public DateTime? LastPasswordChange { get; set; } 
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDateTime { get; set; }
    }

    public class UserProfilePics
    {
        public int Id { get; set; }

        public int UserId { get; set; } 

        public string FilePath { get; set; }
        public string FileName { get; set; }
        public bool IsCurrentProfileImage { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
    }

    public class RoleModel 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDateTime { get; set; }
    }

}
