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
        driver.Navigate().GoToUrl("https://www.google.com/intl/en/account/about/");
        Thread.Sleep(3000); // Chờ trang tải
        try
        {
            // Click vào nút "Mở"
            IWebElement openButton = driver.FindElement(By.XPath("/html/body/header/div[1]/div[5]/ul/li[1]/a"));
            openButton.Click();
            // Thread.Sleep(1000); // Chờ trang tải
            // // Click vào nút "Đăng ký"
            // IWebElement signUpButton = driver.FindElement(By.XPath("/html/body/div[4]/div/div[2]/button[1]"));
            // signUpButton.Click();
            // // Nhập email
            // Thread.Sleep(1000); // Chờ trang tải
            string username = "setkiqotgmdofmg";
            IWebElement userInput = driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div[2]/c-wiz/div/div[2]/div/div/div/form/span/section/div/div/div/div[1]/div[1]/div/div[1]/div/div[1]/input"));
            userInput.SendKeys(username);
            string lastname = "sdàasagsasgas";
            IWebElement lastNameInput = driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div[2]/c-wiz/div/div[2]/div/div/div/form/span/section/div/div/div/div[1]/div[2]/div/div[1]/div/div[1]/input"));
            lastNameInput.SendKeys(lastname);

            Thread.Sleep(1000);
            // Click vào nút "next"
            IWebElement nextButton = driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div[2]/c-wiz/div/div[3]/div/div/div/div/button"));
            nextButton.Click();

            Thread.Sleep(1000);

            string[] birthValues = { "12", "1", "2000" }; // Tháng, ngày, năm
            //var selectElements = driver.FindElements(By.XPath("//*[@id=\"month\"]"));
            //SelectElement select = new SelectElement(selectElements[0]);
            //select.SelectByValue(birthValues[1]);
            ////for (int i = 0; i < selectElements.Count; i++)
            ////{
            ////    SelectElement select = new SelectElement(selectElements[i]);
            ////    select.SelectByValue(birthValues[i]); // Chọn giá trị tương ứng
            ////}

            IWebElement dayInput = driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div[2]/c-wiz/div/div[2]/div/div/div/form/span/section/div/div/div[1]/div[2]/div/div/div[1]/div/div[1]/input"));
            dayInput.SendKeys(birthValues[0]);

            IWebElement yearInput = driver.FindElement(By.XPath("//*[@id=year]"));
            yearInput.SendKeys(birthValues[2]);

            // Thread.Sleep(1000); // Chờ trang tải
            // IWebElement passwordInput = driver.FindElement(By.XPath("//*[@id="day"]"));
            // passwordInput.SendKeys(password);
            // Thread.Sleep(1000); // Chờ trang tải
            // // Click vào nút "Create"
            // IWebElement createButton = driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div[2]/c-wiz/div/div[3]/div/div/div/div/button"));
            // createButton.Click();

            // Console.WriteLine($"Tên tài khoản: {mail}@indigobook.com");
            // Console.WriteLine($"Mật khẩu: {password}");
            // SaveCredentialsToFile($"{mail}@indigobook.com|{password}");
        }
        catch
        {
            Console.WriteLine("Đã xảy ra lỗi");
        }
        finally
        {
            // driver.Quit();
        }
    }
    static void Main()
    {
        string proxyfile = "C:\\Users\\vongu\\OneDrive\\Desktop\\tool\\createGmail\\proxyfile.txt";
        string[] linesProxyfile = File.ReadAllLines(proxyfile);

        for (int i = 0; i < 1; i++){
            string[] proxyfileParts = linesProxyfile[i].Split(':');
            string proxyUser = proxyfileParts[2];
            string proxyPass = proxyfileParts[3];
            string proxyAddress = proxyfileParts[0] + ':' + proxyfileParts[1];

            ChromeOptions options = new ChromeOptions();
            // options.AddArgument("--headless");
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
