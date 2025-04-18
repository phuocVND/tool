using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.IO;
using System.Text;
using System.Threading;
using OpenQA.Selenium.Interactions;

class Program
{
    private static readonly Random random = new Random();

    // Hàm tạo độ trễ ngẫu nhiên từ 0.2 đến 0.5 giây
    private static void RandomDelay()
    {
        Thread.Sleep(random.Next(200, 501));
    }

    // Hàm click tự nhiên
    private static void NaturalClick(IWebDriver driver, IWebElement element)
    {
        new Actions(driver)
            .MoveToElement(element)
            .Pause(TimeSpan.FromMilliseconds(random.Next(100, 300)))
            .Click()
            .Perform();
        RandomDelay();
    }

    // Hàm nhập văn bản tự nhiên
    private static void NaturalInput(IWebElement element, string text)
    {
        foreach (char c in text)
        {
            element.SendKeys(c.ToString());
            Thread.Sleep(random.Next(50, 150));
        }
        RandomDelay();
    }

    // Hàm cuộn trang ngẫu nhiên
    private static void RandomScroll(IWebDriver driver)
    {
        if (random.Next(0, 2) == 0) // 50% cơ hội cuộn
        {
            ((IJavaScriptExecutor)driver).ExecuteScript($"window.scrollBy(0, {random.Next(100, 300)});");
            Thread.Sleep(random.Next(200, 500));
        }
    }

    static void createMailmt(IWebDriver driver, string mail, string password)
    {
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        try
        {
            driver.Navigate().GoToUrl("https://mail.tm");
            RandomScroll(driver);

            // Click vào nút "Mở"
            IWebElement openButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("/html/body/div[1]/div/div[2]/div/div/div[2]/button[3]")));
            NaturalClick(driver, openButton);
            RandomScroll(driver);

            // Click vào nút "Đăng ký"
            IWebElement signUpButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("/html/body/div[4]/div/div[2]/button[1]")));
            NaturalClick(driver, signUpButton);
            RandomScroll(driver);

            // Nhập email
            IWebElement emailInput = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("v-1-15")));
            NaturalInput(emailInput, mail);

            // Nhập password
            IWebElement passwordInput = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("v-1-17")));
            NaturalInput(passwordInput, password);

            // Lấy domain
            IWebElement domain = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/div[8]/div/div/div[1]/form/div[1]/div[2]/div/span[2]/button")));
            string fullEmail = $"{mail}@{domain.Text}";
            RandomDelay();
            
            IWebElement create = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("/html/body/div[8]/div/div/div[2]/span[1]/button")));
            NaturalClick(driver, create);
            RandomScroll(driver);

            // Lặp kiểm tra emailFull và chỉ click "Create" nếu cần
            bool doneCreate = false;
            int maxAttempts = 10;
            int attempt = 0;

            do
            {
                try
                {
                    // Kiểm tra emailFull
                    try
                    {
                        IWebElement emailFull = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Dont_use_WEB_use_API_OK")));
                        string emailValue = emailFull.GetAttribute("value");
                        if (emailValue == fullEmail)
                        {
                            // Console.WriteLine($"Tên tài khoản: {fullEmail}");
                            // Console.WriteLine($"Mật khẩu: {password}");
                            SaveCredentialsToFile($"{fullEmail}|{password}");
                            doneCreate = true;
                            Console.WriteLine("Done");
                        }
                        else
                        {
                            // Console.WriteLine($"Chưa khớp, emailFull: {emailValue}");
                            // Click nút "Create" nếu email không khớp
                            IWebElement createButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("/html/body/div[8]/div/div/div[2]/span[1]/button")));
                            NaturalClick(driver, createButton);
                            // Console.WriteLine($"Đã click nút Create (lần {attempt + 1})");
                            Thread.Sleep(random.Next(1500, 2500));
                        }
                    }
                    catch (WebDriverTimeoutException)
                    {
                        // Console.WriteLine("Chưa tìm thấy emailFull, thử lại...");
                        // Click nút "Create" nếu emailFull chưa xuất hiện
                        IWebElement createButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("/html/body/div[8]/div/div/div[2]/span[1]/button")));
                        NaturalClick(driver, createButton);
                        // Console.WriteLine($"Đã click nút Create (lần {attempt + 1})");
                        Thread.Sleep(random.Next(1500, 2500));
                    }

                    attempt++;
                    if (attempt >= maxAttempts)
                    {
                        Console.WriteLine("Đã vượt quá số lần thử tối đa!");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    // Console.WriteLine($"Lỗi khi xử lý (lần {attempt + 1}): {ex.Message}");
                    Thread.Sleep(random.Next(1500, 2500));
                    attempt++;
                    if (attempt >= maxAttempts)
                    {
                        Console.WriteLine("Đã vượt quá số lần thử tối đa!");
                        break;
                    }
                }
            } while (!doneCreate);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Đã xảy ra lỗi: {ex.Message}");
        }
        finally
        {
            driver.Quit();
        }
    }

static string GenerateRandomUsername()
{
    const string letters = "abcdefghijklmnopqrstuvwxyz"; // Chỉ chữ cái cho ký tự đầu
    const string chars = "abcdefghijklmnopqrstuvwxyz0123456789"; // Chữ cái và số cho các ký tự còn lại
    Random random = new Random();
    int length = random.Next(12, 20); // Độ dài ngẫu nhiên từ 12 đến 19
    StringBuilder username = new StringBuilder(length);

    // Ký tự đầu tiên là chữ cái
    username.Append(letters[random.Next(letters.Length)]);

    // Các ký tự còn lại có thể là chữ cái hoặc số
    for (int i = 1; i < length; i++)
    {
        username.Append(chars[random.Next(chars.Length)]);
    }

    return username.ToString();
}

    static string GenerateRandomPassword()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*_+";
        int length = random.Next(8, 14);
        StringBuilder password = new StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            password.Append(chars[random.Next(chars.Length)]);
        }
        return password.ToString();
    }

    static void SaveCredentialsToFile(string credentials)
    {
        try
        {
            File.AppendAllText("mailPass.txt", credentials + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi lưu thông tin: {ex.Message}");
        }
    }

    static void Main()
    {
        // Danh sách user-agent ngẫu nhiên
        string[] userAgents = new[]
        {
            "Mozilla/5.0 (iPhone; CPU iPhone OS 16_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/16.0 Mobile/15E148 Safari/604.1",
            "Mozilla/5.0 (Linux; Android 12; SM-G998B) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.0.0 Mobile Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.0.0 Safari/537.36",
        };

        // Giới hạn số lần lặp (tạm đặt 10 để tránh chạy quá lâu)
        int maxIterations = 100;
        for (int i = 0; i < maxIterations; i++)
        {
            ChromeOptions options = new ChromeOptions();
            // options.AddArgument("--headless"); // Tạm bỏ để debug
            options.AddArgument("--disable-webrtc");
            options.AddArgument("--disable-features=WebRtcHideLocalIpsWithMdns");
            options.AddArgument("--incognito");
            options.AddArgument("--window-size=414,896");
            options.AddArgument($"--user-agent={userAgents[random.Next(userAgents.Length)]}");

            // Tắt log ChromeDriver
            var service = ChromeDriverService.CreateDefaultService();
            service.SuppressInitialDiagnosticInformation = true;
            service.HideCommandPromptWindow = true;

            using (IWebDriver driver = new ChromeDriver(service, options))
            {
                string mail = GenerateRandomUsername();
                string password = GenerateRandomPassword();
                createMailmt(driver, mail, password);
            }

            // Đợi ngẫu nhiên giữa các lần lặp
            Thread.Sleep(random.Next(1000, 3000));
        }
    }
}