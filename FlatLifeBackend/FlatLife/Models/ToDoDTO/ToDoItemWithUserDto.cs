namespace FlatLife.Models.UserDTO
{
    public class ToDoItemWithUserDto
    {
        public int Id { get; set; }
        public string Task { get; set; }
        public bool IsChecked { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int FlatID { get; set; }
        public string CreatedByUserName { get; set; }
    }
}
