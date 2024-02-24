using System.Diagnostics;

namespace CourseLearning.Core.Models;

[DebuggerDisplay($"Page {nameof(PageNumber)}}}: \"{nameof(Header)}\", \"{nameof(Text)}\";")]
public class Page
{
    public int PageNumber { get; set; }
    public string Header { get; set; }
    public string Text { get; set; }
    public Test StandartTest { get; set; }
    public string Question { get; set; }
    public string CorrectAnswer { get; set; }
}

public class Test
{
    public string Question { get; set; }
    public string[] AnswerOptions { get; set; }
    public int CorrectAnswer { get; set; }
}