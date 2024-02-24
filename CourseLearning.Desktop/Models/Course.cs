namespace CourseLearning.Desktop.Models;

public class Course
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public User User { get; set; }
}