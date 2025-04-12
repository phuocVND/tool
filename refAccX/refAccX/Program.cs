using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using System;
using System.Threading;
using System.IO;
using AutoIt;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        string mailPass = "../../../mailPass.txt";
        string proxyfile = "../../../proxyfile.txt";
        string[] linesMailPass = File.ReadAllLines(mailPass);
        string[] linesProxyfile = File.ReadAllLines(proxyfile);
        string extensionFolderPath = "C:/Users/lit/Desktop/tool/Betacaptcha2";
        int index = 6;

        string[] mailPassParts = linesMailPass[index].Split('|');
        string[] proxyfileParts = linesProxyfile[index].Split(':');

        string user = "kevvin2182849";
        string mail = mailPassParts[0];
        string password = mailPassParts[1];
        string proxyUser = proxyfileParts[2];

        string proxyPass = proxyfileParts[3];
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

        ChromeOptions options = new ChromeOptions();

        //options.AddArgument("--headless");
        options.AddArgument("--disable-webrtc");
        options.AddArgument("--disable-features=WebRtcHideLocalIpsWithMdns");
        options.AddArgument("--incognito"); // Chế độ ẩn danh trong Chrome


        options.AddArgument($"--load-extension={extensionFolderPath}");

        IWebDriver driver = new ChromeDriver(options);
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

            Thread.Sleep(1000); // Chờ trang tải
            Console.WriteLine("Proxy đang hoạt động.");

            var windowHandles = driver.WindowHandles;
            Console.WriteLine($"Số lượng cửa sổ đang mở: {windowHandles.Count}");

            // Nếu có đúng 2 cửa sổ, đóng cửa sổ thứ hai
            if (windowHandles.Count == 2)
            {
                string originalWindow = driver.CurrentWindowHandle; // Lưu cửa sổ chính
                foreach (var handle in windowHandles)
                {
                    if (handle != originalWindow)
                    {
                        driver.SwitchTo().Window(handle); // Chuyển sang cửa sổ thứ hai
                        driver.Close(); // Đóng cửa sổ thứ hai
                        break;
                    }
                }
                driver.SwitchTo().Window(originalWindow); // Quay lại cửa sổ chính
            }

            driver.Navigate().GoToUrl("https://x.com");
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            // Click vào nút "Đăng ký"
            IWebElement signUpButton = driver.FindElement(By.XPath("//*[@id='react-root']/div/div/div[2]/main/div/div/div[1]/div/div/div[3]/a/div"));
            signUpButton.Click();
            Thread.Sleep(4000); // Chờ trang tải

            // Nhập tên người dùng
            IWebElement nameInput = driver.FindElement(By.XPath("//input[@name='name' and contains(@class, 'r-30o5oe')]"));
            nameInput.SendKeys(user);

            // Nhập email
            IWebElement emailInput = driver.FindElement(By.XPath("//input[@name='email' and @type='email']"));
            emailInput.SendKeys(mail);

            // Chọn tháng, ngày, năm sinh
            string[] birthValues = GenerateRandomBirthDate();
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
            driver.SwitchTo().Frame(iframeElement);

            iframeElement = driver.FindElement(By.Id("game-core-frame"));
            driver.SwitchTo().Frame(iframeElement);

            IWebElement authenticate = driver.FindElement(By.XPath("//*[@id='root']/div/div[1]/button"));
            authenticate.Click();

            Thread.Sleep(20000);
            //IWebElement codeVeri = driver.FindElement(By.XPath("/html/body/div/div/div/div[1]/div[2]/div/div/div/div/div/div[2]/div[2]/div/div/div[2]/div[2]/div[1]/div/div[2]/label/div/div[2]/div/input"));
            var elements = driver.FindElements(By.XPath("/html/body/div/div/div/div[1]/div[2]/div/div/div/div/div/div[2]/div[2]/div/div/div[2]/div[2]/div[1]/div/div[2]/label/div/div[2]/div/input"));
            if (elements.Count > 0)
            {
                IWebElement codeVeri = elements[0];
                ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
                var windows = driver.WindowHandles;
                driver.SwitchTo().Window(windows[1]);
                string code = getCode(driver, mail, password);
                driver.SwitchTo().Window(windows[0]);
                codeVeri.SendKeys(code);
                Console.WriteLine("Đã nhập code thành công.");
            }

            //((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
            //var windows = driver.WindowHandles;
            //driver.SwitchTo().Window(windows[1]);
            //string code = getCode(driver, mail, password);
            //driver.SwitchTo().Window(windows[0]);

            //codeVeri.SendKeys(code);


            nextButton = driver.FindElement(By.XPath("/html/body/div/div/div/div[1]/div[2]/div/div/div/div/div/div[2]/div[2]/div/div/div[2]/div[2]/div[2]/div/div/div/button/div"));
            nextButton.Click();

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
    static string[] GenerateRandomBirthDate()
    {
        DateTime currentDate = DateTime.Now;
        Random random = new Random();

        // Tính năm sinh ngẫu nhiên cho độ tuổi 18-40
        int age = random.Next(18, 41);
        int birthYear = currentDate.Year - age;

        // Chọn tháng ngẫu nhiên
        int birthMonth = random.Next(1, 13);

        // Chọn ngày ngẫu nhiên, đảm bảo hợp lệ với tháng và năm
        int maxDays = DateTime.DaysInMonth(birthYear, birthMonth);
        int birthDay = random.Next(1, maxDays + 1);

        // Trả về mảng [tháng, ngày, năm]
        return new string[] { birthMonth.ToString(), birthDay.ToString(), birthYear.ToString() };
    }
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

            string codeText = "";


            Thread.Sleep(2000); // Chờ trang tải

            try
            {
                IWebElement nextButton = driver.FindElement(By.XPath("/html/body/div[1]/div/div[2]/main/div[2]/div[2]/ul/li[1]/a/div/div/div[2]/div[1]/div[2]"));
                nextButton.Click();
                Thread.Sleep(1000); // Chờ trang tải

                IWebElement iframeElement = driver.FindElement(By.XPath("/html/body/div[1]/div/div[2]/main/div[2]/div[2]/div[2]/div/iframe"));
                driver.SwitchTo().Frame(iframeElement);

                IWebElement code = driver.FindElement(By.XPath("/html/body/table/tbody/tr[1]/td/table/tbody/tr[1]/td/table[1]/tbody/tr/td[2]/table/tbody/tr[10]/td"));
                Console.WriteLine($"{code.Text}");

                codeText = code.Text;
            }
            catch
            {
                driver.Navigate().Refresh();
                Thread.Sleep(2000); // Chờ trang tải
                IWebElement nextButton = driver.FindElement(By.XPath("/html/body/div[1]/div/div[2]/main/div[2]/div[2]/ul/li[1]/a/div/div/div[2]/div[1]/div[2]"));
                nextButton.Click();
                Thread.Sleep(1000); // Chờ trang tải

                IWebElement iframeElement = driver.FindElement(By.XPath("/html/body/div[1]/div/div[2]/main/div[2]/div[2]/div[2]/div/iframe"));
                driver.SwitchTo().Frame(iframeElement);

                IWebElement code = driver.FindElement(By.XPath("/html/body/table/tbody/tr[1]/td/table/tbody/tr[1]/td/table[1]/tbody/tr/td[2]/table/tbody/tr[10]/td"));
                Console.WriteLine($"{code.Text}");

                codeText = code.Text;
            }

            return codeText;
        }
        catch
        {
            driver.Navigate().Refresh();
            Console.WriteLine("Đã xảy ra lỗi");
            return "";
        }
        finally
        {
            driver.Close();
        }
    }
}