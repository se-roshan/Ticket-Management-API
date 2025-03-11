namespace WebAPI_Code_First.Entities
{
    public class Role
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
