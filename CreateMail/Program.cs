using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using System;
using System.Threading;
using System.IO;
using System.Text;
using OpenQA.Selenium.Chrome;


class Program
{
    static void createMailmt(IWebDriver driver, string mail, string password)
    {
        driver.Navigate().GoToUrl("https://mail.tm");
        Thread.Sleep(3000); // Chờ trang tải
        try
        {
            // Click vào nút "Mở"
            IWebElement openButton = driver.FindElement(By.XPath("/html/body/div[1]/div/div[2]/div/div/div[2]/button[3]"));
            openButton.Click();
            Thread.Sleep(1000); // Chờ trang tải
            // Click vào nút "Đăng ký"
            IWebElement signUpButton = driver.FindElement(By.XPath("/html/body/div[4]/div/div[2]/button[1]"));
            signUpButton.Click();
            // Nhập email
            Thread.Sleep(1000); // Chờ trang tải
            IWebElement emailInput = driver.FindElement(By.XPath("/html/body/div[8]/div/div/div[1]/form/div[1]/div[2]/div/input"));
            emailInput.SendKeys(mail);
            Thread.Sleep(1000); // Chờ trang tải
            IWebElement passwordInput = driver.FindElement(By.XPath("/html/body/div[8]/div/div/div[1]/form/div[2]/div[2]/div/input"));
            passwordInput.SendKeys(password);
            Thread.Sleep(1000); // Chờ trang tải
            // Click vào nút "Create"
            IWebElement createButton = driver.FindElement(By.XPath("/html/body/div[8]/div/div/div[2]/span[1]/button"));
            createButton.Click();

            IWebElement domain = driver.FindElement(By.XPath("/html/body/div[8]/div/div/div[1]/form/div[1]/div[2]/div/span[2]/button"));
            

            Console.WriteLine($"Tên tài khoản: {mail}@{domain.Text}");
            Console.WriteLine($"Mật khẩu: {password}");
            SaveCredentialsToFile($"{mail}@{domain.Text}|{password}");
        }
        catch
        {
            Console.WriteLine("Đã xảy ra lỗi");
        }
        finally
        {
            driver.Quit();
        }
    }
    static void Main()
    {
        string proxyfile = "proxyfile.txt";
        string[] linesProxyfile = File.ReadAllLines(proxyfile);



        for (int i = 0; i < 10; i++){
            string[] proxyfileParts = linesProxyfile[i].Split(':');
            string proxyUser = proxyfileParts[2];
            string proxyPass = proxyfileParts[3];
            string proxyAddress = proxyfileParts[0] + ':' + proxyfileParts[1];

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--disable-webrtc");
            options.AddArgument("--disable-features=WebRtcHideLocalIpsWithMdns");
            options.AddArgument("--inprivate"); // Chế độ ẩn danh

            IWebDriver driver = new ChromeDriver(options);
            Actions actions = new Actions(driver);

            string mail = GenerateRandomUsername();
            string password = GenerateRandomPassword();
            createMailmt(driver, mail, password);
        }
    }
        static void SaveCredentialsToFile(string credentials)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter("mailPass.txt", true))
            {
                writer.WriteLine(credentials);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi lưu thông tin: {ex.Message}");
        }
    }
    static string GenerateRandomUsername()
    {
        // Ký tự có thể sử dụng cho tên tài khoản
        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        
        // Chọn độ dài ngẫu nhiên từ 8 đến 12 ký tự
        Random random = new Random();
        int length = random.Next(12, 20);

        // Tạo tên tài khoản ngẫu nhiên
        StringBuilder username = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            username.Append(chars[random.Next(chars.Length)]);
        }

        return username.ToString();
    }


    // Hàm tạo mật khẩu ngẫu nhiên
    static string GenerateRandomPassword()
    {
        // Ký tự có thể sử dụng trong mật khẩu
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*_+";
        
        // Chọn độ dài ngẫu nhiên từ 8 đến 12 ký tự
        Random random = new Random();
        int length = random.Next(8, 14);

        // Tạo mật khẩu ngẫu nhiên
        StringBuilder password = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            password.Append(chars[random.Next(chars.Length)]);
        }
        return password.ToString();
    }
}
