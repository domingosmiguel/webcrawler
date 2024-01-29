using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System.Text.Json;

namespace WebScraper
{
  public class ScraperData
  {
    public int Id { get; }
    public string? DataInicio { get; set; }
    public string? DataFim { get; set; }
    public int Páginas { get; set; }
    public int Linhas { get; set; }
    public string? Json { get; set; }
  }

  public class ProxyServer
  {
    public string? IPAddress { get; set; }
    public string? Port { get; set; }
    public string? Country { get; set; }
    public string? Protocol { get; set; }
  }

  public class Counter
  {
    private int count;
    public Counter()
    {
      count = 0;
    }
    public void Increment()
    {
      count++;
    }
    public void Reset()
    {
      count = 0;
    }
    public int GetCount()
    {
      return count;
    }
  }

  class WorkerParams
  {
    public string ScrapId { get; set; }
    public int PagesCount { get; set; }
    public string HtmlContent { get; set; }
    public IEnumerable<HtmlNode> ProxyHTMLElements { get; set; }
    public List<ProxyServer> ProxyServers { get; set; }
  }

  static class Program
  {
    private static Semaphore _pool;
    static async Task Main()
    {
      const string baseUrl = "https://proxyservers.pro/proxy/list/order/updated/order_dir/desc";
      string urlToScrap = baseUrl;

      List<ProxyServer> ProxyServers = [];

      Counter pagesCount = new();

      string scrapId = Guid.NewGuid().ToString();
      string beginDate = DateTime.Now.ToString();

      _pool = new Semaphore(initialCount: 0, maximumCount: 3);

      ChromeOptions chromeOptions = new();
      chromeOptions.AddArguments("headless");

      using ChromeDriver driver = new(chromeOptions);
      do
      {
        driver.Navigate().GoToUrl(urlToScrap);
        string htmlContent = driver.PageSource;
        HtmlDocument document = new();
        document.LoadHtml(htmlContent);

        pagesCount.Increment();

        IEnumerable<HtmlNode> proxyHTMLElements = document.DocumentNode.QuerySelector("tbody").GetChildElements();

        WorkerParams workerParams = new()
        {
          ScrapId = scrapId,
          PagesCount = pagesCount.GetCount(),
          HtmlContent = htmlContent,
          ProxyHTMLElements = proxyHTMLElements,
          ProxyServers = ProxyServers
        };

        Thread t = new(new ParameterizedThreadStart(Worker));
        t.Start(workerParams);

        HtmlNode nextPageButtonNode = document.DocumentNode.QuerySelector("li.page-item.active").NextSiblingElement();

        if (nextPageButtonNode == null)
        {
          break;
        }

        string pageNumber = nextPageButtonNode.InnerText.Trim();
        urlToScrap = string.Format(@"{0}/page/{1}", baseUrl, pageNumber);
      }
      while (pagesCount.GetCount() < 60);

      _pool.Release(3);

      string endDate = DateTime.Now.ToString();

      string jsonData = JsonSerializer.Serialize(ProxyServers);

      ScraperData scraperData = new() { DataInicio = beginDate, DataFim = endDate, Páginas = pagesCount.GetCount(), Linhas = ProxyServers.Count, Json = jsonData };
      SaveData.ToDatabase(scraperData);

      SaveData.ToJSON(scrapId, jsonData);
    }

    private static void Worker(object obj)
    {
      WorkerParams workerParams = (WorkerParams)obj;
      string scrapId = workerParams.ScrapId;
      int pagesCount = workerParams.PagesCount;
      string htmlContent = workerParams.HtmlContent;
      IEnumerable<HtmlNode> proxyHTMLElements = workerParams.ProxyHTMLElements;
      List<ProxyServer> ProxyServers = workerParams.ProxyServers;

      _pool.WaitOne();

      SaveData.ToHTML(scrapId, pagesCount, htmlContent);
      ScrapPage(proxyHTMLElements, ProxyServers);

      _pool.Release();
    }

    static void ScrapPage(IEnumerable<HtmlNode> proxyHTMLElements, List<ProxyServer> ProxyServers)
    {
      foreach (HtmlNode proxyHTMLElement in proxyHTMLElements)
      {
        List<HtmlNode> proxyHTMLData = [.. proxyHTMLElement.QuerySelectorAll("td")];

        string ipAddress = proxyHTMLData[1].QuerySelector("a").Attributes["href"].Value.Trim().Replace("/proxy/", "");
        string port = proxyHTMLData[2].QuerySelector("span.port").InnerText.Trim();
        string country = proxyHTMLData[3].InnerText.Trim();
        string protocol = proxyHTMLData[6].InnerText.Trim();

        ProxyServer proxyData = new() { IPAddress = ipAddress, Port = port, Country = country, Protocol = protocol };

        ProxyServers.Add(proxyData);
      }
    }
  }
}

