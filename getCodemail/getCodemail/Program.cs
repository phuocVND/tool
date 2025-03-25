using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using System;
using System.Threading;
using System.IO;
using OpenQA.Selenium.Chrome;


class Program
{
    static string getCode(IWebDriver driver, string mail, string password)
    {
        try
        {
            driver.Navigate().GoToUrl("https://mail.tm");
            //wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            Thread.Sleep(3000); // Chờ trang tải

            // Click vào nút "Mở"
            IWebElement openButton = driver.FindElement(By.XPath("/html/body/div[1]/div/div[2]/div/div/div[2]/button[3]"));
            openButton.Click();

            //// Click vào nút "Đăng ký"
            //IWebElement signUpButton = driver.FindElement(By.XPath("/html/body/div[4]/div/div[2]/button[1]"));
            //signUpButton.Click();

            // Click vào nút "Login"
            IWebElement loginButton = driver.FindElement(By.XPath("/html/body/div[4]/div/div[2]/button[2]"));
            loginButton.Click();

            Thread.Sleep(1000); // Chờ trang tải

            // Nhập email
            IWebElement emailInput = driver.FindElement(By.XPath("/html/body/div[8]/div/div/div[1]/form/div[1]/div[2]/div/input"));
            emailInput.SendKeys(mail);

            // Nhập tên người dùng
            IWebElement passwordInput = driver.FindElement(By.XPath("/html/body/div[8]/div/div/div[1]/form/div[2]/div[2]/div/input"));
            passwordInput.SendKeys(password);
            Thread.Sleep(1000); // Chờ trang tải
            IWebElement logButton = driver.FindElement(By.XPath("/html/body/div[8]/div/div/div[2]/span[1]/button"));
            logButton.Click();
            Thread.Sleep(2000); // Chờ trang tải
            IWebElement nextButton = driver.FindElement(By.XPath("/html/body/div[1]/div/div[2]/main/div[2]/div[2]/ul/li[2]/a/div/div/div[2]/div[1]/div[2]"));
            nextButton.Click();
            Thread.Sleep(1000); // Chờ trang tải
            IWebElement iframeElement = driver.FindElement(By.XPath("/html/body/div[1]/div/div[2]/main/div[2]/div[2]/div[2]/div/iframe"));
            driver.SwitchTo().Frame(iframeElement);

            IWebElement code = driver.FindElement(By.XPath("/html/body/table/tbody/tr[1]/td/table/tbody/tr[1]/td/table[1]/tbody/tr/td[2]/table/tbody/tr[10]/td"));
            //Console.WriteLine($"{code.Text}");
            string codeText = code.Text;
            return codeText;
        }
        catch
        {
            Console.WriteLine("Đã xảy ra lỗi");
            return "";
        }
        finally
        {
            driver.Close();
        }
    }
    static void Main()
    {
        string mailPass = "../../../mailPass.txt";
        string proxyfile = "../../../proxyfile.txt";
        string[] linesMailPass = File.ReadAllLines(mailPass);
        string[] linesProxyfile = File.ReadAllLines(proxyfile);

        int index = 1;

        string[] mailPassParts = linesMailPass[index].Split('|');
        string[] proxyfileParts = linesProxyfile[index].Split(':');

        string mail = mailPassParts[0];
        string password = mailPassParts[1];
        string proxyUser = proxyfileParts[2];
        string proxyPass = proxyfileParts[3];
        string proxyAddress = proxyfileParts[0] + ':' + proxyfileParts[1];

        EdgeOptions options = new EdgeOptions();

        //options.AddArgument("--headless");
        options.AddArgument("--disable-webrtc");
        options.AddArgument("--disable-features=WebRtcHideLocalIpsWithMdns");
        options.AddArgument("--inprivate"); // Chế độ ẩn danh


        IWebDriver driver = new EdgeDriver(options);
        Actions actions = new Actions(driver);

        WebDriverWait wait;

        ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
        var windows = driver.WindowHandles;
        driver.Navigate().GoToUrl("https://x.com");
        driver.SwitchTo().Window(windows[1]);
        string code = getCode(driver, mail, password);
        driver.SwitchTo().Window(windows[0]);
        Console.WriteLine(code);
    }
}
