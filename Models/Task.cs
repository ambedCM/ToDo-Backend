namespace Todo.Models;
public class TodoTask {
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = "Pending";
    public int UserId { get; set; }
    public User User { get; set; }
}