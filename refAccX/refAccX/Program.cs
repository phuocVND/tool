using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using System;
using System.Threading;
using System.IO;
using OpenQA.Selenium.Chrome;
using AutoIt;

class Program
{
    static void Main()
    {


        string mailPass = "../../../mailPass.txt";
        string proxyfile = "../../../proxyfile.txt";
        string[] linesMailPass = File.ReadAllLines(mailPass);
        string[] linesProxyfile = File.ReadAllLines(proxyfile);

        int index = 9;

        string[] mailPassParts = linesMailPass[index].Split('|');
        string[] proxyfileParts = linesProxyfile[index].Split(':');

        string user = "kevvin2182849";
        string mail = mailPassParts[0];
        string password = mailPassParts[1];
        string proxyUser = proxyfileParts[2];
        string proxyPass = proxyfileParts[3];
        //string proxyAddress = proxyUser + ':' + proxyPass + '@' + proxyfileParts[0] + ':' + proxyfileParts[1];
        string proxyAddress = proxyfileParts[0] + ':' + proxyfileParts[1];

        // In ra các thông tin đã cấu hình
        Console.WriteLine("Thông tin cấu hình:");
        Console.WriteLine($"Email: {mail}");
        Console.WriteLine($"Mật khẩu: {password}");
        Console.WriteLine($"Proxy: {proxyAddress}");
        Console.WriteLine($"Proxy: {proxyUser}");
        Console.WriteLine($"Proxy: {proxyPass}");

        var proxy = new Proxy
        {

            HttpProxy = $"{proxyAddress}",
            SslProxy = $"{proxyAddress}"
        };



        EdgeOptions options = new EdgeOptions();
        options.Proxy = proxy;
        //options.AddArgument("--headless");
        options.AddArgument($"--proxy-user={proxyUser}");
        options.AddArgument($"--proxy-password={proxyPass}");
        //options.AddArgument($"--proxy-server={proxyUser}:{proxyPass}@{proxyAddress}");
        options.AddArgument("--disable-webrtc");
        options.AddArgument("--disable-features=WebRtcHideLocalIpsWithMdns");
        //options.AddArgument("--inprivate"); // Chế độ ẩn danh

        string extensionFolderPath = "C:/Users/vongu/OneDrive/Desktop/learnToolC#/projectSelenium/BetaCaptcha";
        options.AddArgument($"--load-extension={extensionFolderPath}");
        
        IWebDriver driver = new EdgeDriver(options);
        Actions actions = new Actions(driver);

        WebDriverWait wait;

        try
        {
            driver.Navigate().GoToUrl("https://api.ipify.org");
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            IWebElement ipElement = wait.Until(d => d.FindElement(By.TagName("body")));

            AutoItX.WinWaitActive("Proxy Authentication", "", 1);
            AutoItX.Send(proxyUser);
            AutoItX.Send("{TAB}");
            AutoItX.Send(proxyPass);
            AutoItX.Send("{ENTER}");

            //string currentIp = ipElement.Text;
            //Console.WriteLine($"Địa chỉ IP hiện tại: {currentIp}");

            Thread.Sleep(1000); // Chờ trang tải
            //if (true)
            //{
                Console.WriteLine("Proxy đang hoạt động.");

                driver.Navigate().GoToUrl("https://x.com");
                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                // Click vào nút "Đăng ký"
                IWebElement signUpButton = driver.FindElement(By.XPath("//*[@id='react-root']/div/div/div[2]/main/div/div/div[1]/div/div/div[3]/a/div"));
                signUpButton.Click();
                Thread.Sleep(3000); // Chờ trang tải

                // Nhập tên người dùng
                IWebElement nameInput = driver.FindElement(By.XPath("//input[@name='name' and contains(@class, 'r-30o5oe')]"));
                nameInput.SendKeys(user);

                // Nhập email
                IWebElement emailInput = driver.FindElement(By.XPath("//input[@name='email' and @type='email']"));
                emailInput.SendKeys(mail);

                // Chọn tháng, ngày, năm sinh
                string[] birthValues = { "12", "4", "2000" }; // Tháng, ngày, năm
                var selectElements = driver.FindElements(By.XPath("//select[contains(@class, 'r-30o5oe')]"));

                for (int i = 0; i < selectElements.Count; i++)
                {
                    SelectElement select = new SelectElement(selectElements[i]);
                    select.SelectByValue(birthValues[i]); // Chọn giá trị tương ứng
                }

                Thread.Sleep(1000); // Chờ thao tác hoàn tất

                // Click vào nút "Tiếp theo"
                IWebElement nextButton = driver.FindElement(By.XPath("//div[@class='css-175oi2r r-b9tw7p']/button"));
                actions.MoveToElement(nextButton).Perform();
                nextButton.Click();

                Thread.Sleep(3000); // Chờ trang tải

                IWebElement iframeElement = driver.FindElement(By.Id("arkoseFrame"));
                driver.SwitchTo().Frame(iframeElement);

                iframeElement = driver.FindElement(By.CssSelector("iframe[src*='client-api.arkoselabs.com'][data-e2e='enforcement-frame']"));

                // Chuyển vào iframe
                driver.SwitchTo().Frame(iframeElement);

                iframeElement = driver.FindElement(By.Id("game-core-frame"));
                driver.SwitchTo().Frame(iframeElement);

                IWebElement authenticate = driver.FindElement(By.XPath("//*[@id='root']/div/div[1]/button"));
                authenticate.Click();
            //}
            //else
            //{
            //    Console.WriteLine("Proxy không hoạt động.");
            //}
        }
        catch
        {
            Console.WriteLine("Đã xảy ra lỗi");
        }
        finally
        {
            // Đóng trình duyệt
            //driver.Quit();
        }
    }
}
