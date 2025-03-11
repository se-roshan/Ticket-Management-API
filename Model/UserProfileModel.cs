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
}
