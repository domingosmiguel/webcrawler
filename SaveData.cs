using WebScraper.Data;

namespace WebScraper
{
  static class SaveData
  {
    public static void ToJSON(string scrapId, string content)
    {
      string fileName = string.Format(@"{0}.json", scrapId);
      string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
      string filePath = Path.Combine(projectDirectory, "data", fileName);

      _ = Directory.CreateDirectory(Path.GetDirectoryName(filePath));

      try
      {
        File.WriteAllText(filePath, content);
      }
      catch (IOException ex)
      {
        Console.WriteLine("Error saving file: {0}", ex.Message);
      }
    }

    public static void ToHTML(string scrapId, int pageNumber, string content)
    {
      string fileName = string.Format(@"page_{0}.html", pageNumber);
      string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
      string filePath = Path.Combine(projectDirectory, "data", scrapId, fileName);

      _ = Directory.CreateDirectory(Path.GetDirectoryName(filePath));

      try
      {
        using StreamWriter writer = new(filePath);
        writer.Write(content.ToString());
      }
      catch (IOException ex)
      {
        Console.WriteLine("Error saving file: {0}", ex.Message);
      }
    }

    public static void ToDatabase(ScraperData data)
    {
      using DataContext dataContext = new();
      dataContext.Scraps.Add(data);

      dataContext.SaveChanges();
    }
  }
}