namespace WebAPI_Code_First.Entities
{
    public class FileUpload
    {
        public int Id { get; set; }

        public int UserId { get; set; } // Foreign Key to Users table
        public string FileName { get; set; }
        public bool IsCurrentProfileImage { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
    }
}
