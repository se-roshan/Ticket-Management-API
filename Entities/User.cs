namespace WebAPI_Code_First.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? ContactNo { get; set; }
        public string? Password { get; set; }
        public DateTime LastPasswordChange { get; set; }
        public int LastPasswordChangeCount { get; set; }
        public string? Gender { get; set; } 
        public string? DOB { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDateTime { get; set; }
    }


}
