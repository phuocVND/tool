using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using System;
using System.Threading;
using System.IO;

class Program
{
    static void Main()
    {
        try
        {
            // Khởi tạo ChromeOptions
            ChromeOptions options = new ChromeOptions();

            // Thêm các argument tùy chọn
            // options.AddArgument("--headless"); // Chạy ở chế độ ẩn (bỏ comment nếu cần)
            options.AddArgument("--disable-webrtc");
            options.AddArgument("--disable-features=WebRtcHideLocalIpsWithMdns");
            options.AddArgument("--inprivate"); // Chế độ ẩn danh

            // Đường dẫn đến file extension .zip
            string extensionPath = @"../ProxyAuthExtension.zip";
            if (!File.Exists(extensionPath))
            {
                Console.WriteLine($"Error: Extension file not found at: {extensionPath}");
                Console.WriteLine("Please ensure the ProxyAuthExtension.zip file exists and is correctly formatted.");
                return;
            }

            // Thêm extension vào Chrome
            try
            {
                options.AddExtension(extensionPath);
                Console.WriteLine("Extension added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding extension: {ex.Message}");
                return;
            }

            // Khởi tạo ChromeDriver
            using (IWebDriver driver = new ChromeDriver(options))
            {
                try
                {
                    // Mở một trang web để kiểm tra proxy
                    driver.Navigate().GoToUrl("https://api.ipify.org");

                    // Chờ vài giây để kiểm tra kết quả
                    Thread.Sleep(5000);

                    // In tiêu đề trang để xác nhận
                    Console.WriteLine("Page title: " + driver.Title);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during browser operation: {ex.Message}");
                }
                finally
                {
                    // Đóng trình duyệt
                    driver.Quit();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General error: {ex.Message}");
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}