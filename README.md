# Web Scraper

## Overview

This C# web scraper project uses Selenium and HtmlAgilityPack to collect proxy server data from [proxyservers.pro](https://proxyservers.pro/proxy/list/order/updated/order_dir/desc) and saves it in various formats (HTML, JSON, and Database).

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) installed
- [ChromeDriver](https://sites.google.com/chromium.org/driver/) installed and added to your system's PATH.

## Getting Started

1. **Clone this repository:**

   ```bash
   git clone https://github.com/your-username/web-scraper.git
   ```

2. **Navigate to the project directory:**

   ```bash
   cd web-scraper
   ```

3. **Download dependencies:**

   ```bash
   dotnet restore
   ```

4. **Apply database migrations:**

   ```bash
   dotnet ef database update
   ```

5. **Build the project:**

   ```bash
   dotnet build
   ```

6. **Run the project:**

   ```bash
   dotnet run
   ```

## Configuration

- The base URL for scraping is set in the `baseUrl` variable in the `Program.cs` file.
- The project uses SQLite as the default database. You can change the database configuration in the `DataContext.cs` file.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
