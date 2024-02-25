using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using CourseLearning.Core.Models;

namespace CourseLearning.Desktop.Services;
public class CourseService
{
    public IEnumerable<Page> OpenFile(FileInfo file)
    {
        if (!file.Exists)
            return Enumerable.Empty<Page>();

        JsonSerializerOptions jso = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
            
        };

        try
        {
            var pages = JsonSerializer.Deserialize<List<Page>>(
                File.ReadAllText(file.FullName),
                options: jso);
            if (pages is null)
                return Enumerable.Empty<Page>();

            return pages;
        }
        catch (Exception)
        {
            return Enumerable.Empty<Page>();
        }
    }
}
