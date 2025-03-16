namespace WebAPI_Code_First.Model
{
    public class UserProfileModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContactNo { get; set; }
        public String EmailId { get; set; }
        public bool IsActive { get; set; }
        
        public List<UserProfilePics>? UserProfilePics { get; set; }
    }

    public class UserProfilePictureModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public DateTime? LastPasswordChange { get; set; }
        public string Gender { get; set; }
        public DateTime? DOB { get; set; }
        public string ProfileImage { get; set; } // ✅ Full URL to profile image
        public bool IsActive { get; set; }
    }


}
