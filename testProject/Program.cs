using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;

class Program
{
    static void Main()
    {
        // Thông tin proxy
        string proxyHost = "157.66.252.83";
        string proxyPort = "40471";
        string proxyUser = "emgai38";
        string proxyPass = "emgai38";

        string proxyExtensionPath = null;
        try
        {
            // Gọi hàm SetupProxy để tạo extension proxy
            proxyExtensionPath = SetupProxy(proxyHost, proxyPort, proxyUser, proxyPass);

            // Cấu hình ChromeOptions
            ChromeOptions options = new ChromeOptions();
            // Tải extension dưới dạng thư mục
            options.AddArgument($"--load-extension={proxyExtensionPath}");
            // Vô hiệu hóa các cảnh báo extension
            options.AddArgument("--disable-extensions-except=" + proxyExtensionPath);
            options.AddArgument("--ignore-certificate-errors");
            // Tùy chọn để giảm lỗi trên macOS
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");

            // Khởi tạo ChromeDriver
            using (IWebDriver driver = new ChromeDriver(options))
            {
                System.Threading.Thread.Sleep(1000);
                // Truy cập trang web để kiểm tra proxy
                driver.Navigate().GoToUrl("https://checkip.com.vn/");

                // Đợi để xem kết quả
                System.Threading.Thread.Sleep(5000);

                // Đóng trình duyệt
                driver.Quit();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi: {ex.Message}");
        }
        finally
        {
            // Xóa thư mục tạm
            if (Directory.Exists(proxyExtensionPath))
            {
                Directory.Delete(proxyExtensionPath, true);
            }
        }
    }

    static string SetupProxy(string proxyHost, string proxyPort, string proxyUser, string proxyPass)
    {
        // Đường dẫn thư mục tạm cho extension
        string proxyExtensionPath = Path.Combine(Directory.GetCurrentDirectory(), "proxy_auth_extension");

        try
        {
            // Tạo thư mục cho extension nếu chưa tồn tại
            if (!Directory.Exists(proxyExtensionPath))
            {
                Directory.CreateDirectory(proxyExtensionPath);
            }

            // Tạo manifest.json (Manifest V3)
            string manifestJson = @"{
                ""version"": ""1.0.0"",
                ""manifest_version"": 3,
                ""name"": ""Proxy Auto Auth"",
                ""permissions"": [
                    ""proxy"",
                    ""storage"",
                    ""webRequest"",
                    ""webRequestAuthProvider""
                ],
                ""host_permissions"": [
                    ""<all_urls>""
                ],
                ""background"": {
                    ""service_worker"": ""background.js""
                },
                ""minimum_chrome_version"": ""88.0.0""
            }";

            // Tạo background.js
            string backgroundJs = $@"var config = {{
                mode: ""fixed_servers"",
                rules: {{
                    singleProxy: {{
                        scheme: ""http"",
                        host: ""{proxyHost}"",
                        port: parseInt({proxyPort})
                    }},
                    bypassList: [""localhost""]
                }}
            }};

            chrome.proxy.settings.set({{value: config, scope: ""regular""}}, function() {{}});

            chrome.webRequest.onAuthRequired.addListener(
                function(details, callbackFn) {{
                    callbackFn({{
                        authCredentials: {{
                            username: ""{proxyUser}"",
                            password: ""{proxyPass}""
                        }}
                    }});
                }},
                {{urls: [""<all_urls>""]}},
                ['asyncBlocking']
            );";

            // Lưu manifest.json và background.js
            File.WriteAllText(Path.Combine(proxyExtensionPath, "manifest.json"), manifestJson);
            File.WriteAllText(Path.Combine(proxyExtensionPath, "background.js"), backgroundJs);

            return proxyExtensionPath;
        }
        catch (Exception ex)
        {
            // Xóa thư mục tạm nếu có lỗi
            if (Directory.Exists(proxyExtensionPath))
            {
                Directory.Delete(proxyExtensionPath, true);
            }
            throw new Exception($"Lỗi khi thiết lập proxy: {ex.Message}");
        }
    }
}