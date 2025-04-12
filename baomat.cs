using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

class SeleniumPrivacy
{
    static void Main(string[] args)
    {
        // Thiết lập các tùy chọn trình duyệt
        var options = new ChromeOptions();

        // 1. Thay đổi User-Agent
        options.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

        // 2. Sử dụng Proxy
        string proxy = "123.45.67.89:8080"; // Thay thế bằng proxy của bạn
        options.AddArgument($"--proxy-server={proxy}");

        // 3. Chặn Cookies
        options.AddArgument("--disable-cookies");

        // 4. Vô hiệu hóa JavaScript
        options.AddArgument("--disable-javascript");

        // 5. Thay đổi Vị trí Địa lý
        options.AddArgument("--use-fake-ui-for-media-stream");
        options.AddArgument("--use-fake-device-for-media-stream");
        options.AddArgument("--lang=en-US"); // Ngôn ngữ
        options.AddArgument("--geolocation=37.7749,-122.4194"); // Vị trí giả mạo (San Francisco)

        // 6. Sử dụng Trình duyệt Ẩn danh
        options.AddArgument("--incognito");

        // 7. Ẩn thông tin tự động hóa (tránh bị phát hiện là Selenium)
        options.AddArgument("--disable-blink-features=AutomationControlled");

        // Khởi tạo trình duyệt với các tùy chọn
        IWebDriver driver = new ChromeDriver(options);

        // Mở trang web
        driver.Navigate().GoToUrl("https://example.com");

        // Đóng trình duyệt sau khi hoàn thành
        driver.Quit();
    }
}
