using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using System;
using System.Threading;
using System.IO;
using AutoIt;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Linq;

class Program
{
    // Hàm tạo độ trễ ngẫu nhiên để mô phỏng hành vi con người
    static void RandomDelay(int minMs, int maxMs)
    {
        Random random = new Random();
        Thread.Sleep(random.Next(minMs, maxMs));
    }
    static string GetRandomLastName()
    {
        List<string> lastNames = new List<string>
        {
            "Smith", "Johnson", "Williams", "Brown", "Jones", "Miller", "Davis", "García", "Rodriguez", "Martínez",
            "Hernández", "Lopez", "González", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin",
            "Lee", "Perez", "Thompson", "White", "Harris", "Sanchez", "Clark", "Ramírez", "Lewis", "Roberts",
            "Walker", "Young", "King", "Scott", "Green", "Adams", "Baker", "Nelson", "Hill", "Carter", "Mitchell",
            "Perez", "Robinson", "Gomez", "Hall", "Rivera", "Wright", "Lopez", "Gonzalez", "Wilson", "Martinez",
            "Brown", "Davis", "Miller", "Taylor", "Moore", "Jackson", "Martin", "Lee", "Allen", "Sanchez",
            "Hernandez", "Morris", "Young", "King", "Scott", "Green", "Adams", "Baker", "Nelson", "Carter",
            "Mitchell", "Wright", "Clark", "Cooper", "Morgan", "Rogers", "Reed", "Cook", "Hughes", "Ward",
            "Flores", "Rivera", "Campbell", "Bailey", "Gonzalez", "James", "Roberts", "Jenkins", "Simmons",
            "Foster", "Perry", "Russell", "Powell", "Long", "Patterson", "Hughes", "Perry", "Sanders"
        };

        // Sử dụng DateTime.Now.Ticks làm seed cho Random
        Random random = new Random((int)DateTime.Now.Ticks);
        int index = random.Next(lastNames.Count);  // Lấy chỉ số ngẫu nhiên trong danh sách
        return lastNames[index];  // Trả về tên họ ngẫu nhiên
    }
    // Hàm tạo username ngẫu nhiên
    static string GenerateRandomUsername()
    {
        Random random = new Random();
        StringBuilder username = new StringBuilder();
        string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string numbers = "0123456789";
        string allowedChars = letters + numbers + "_";

        // Độ dài username từ 6 đến 15 ký tự
        int length = random.Next(6, 16);

        // Bắt đầu bằng chữ cái để hợp lệ
        username.Append(letters[random.Next(letters.Length)]);

        // Thêm các ký tự còn lại
        for (int i = 1; i < length - 2; i++)
        {
            username.Append(allowedChars[random.Next(allowedChars.Length)]);
        }

        // Kết thúc bằng số để trông tự nhiên
        username.Append(numbers[random.Next(numbers.Length)]);

        // Thêm số ngẫu nhiên 2-4 chữ số để tăng tính độc đáo
        username.Append(random.Next(10, 10000));

        return username.ToString();
    }

    // Hàm tạo ngày sinh ngẫu nhiên
    static string[] GenerateRandomBirthDate()
    {
        DateTime currentDate = DateTime.Now;
        Random random = new Random();
        int age = random.Next(18, 41); // Tuổi từ 18 đến 40
        int birthYear = currentDate.Year - age;
        int birthMonth = random.Next(1, 13);
        int maxDays = DateTime.DaysInMonth(birthYear, birthMonth);
        int birthDay = random.Next(1, maxDays + 1);
        return new string[] { birthMonth.ToString(), birthDay.ToString(), birthYear.ToString() };
    }

    // Hàm lấy User-Agent ngẫu nhiên (chỉ Chrome desktop)
    static string GetRandomUserAgent(string userAgentFilePath = "../../../userAgent.txt")
    {
        try
        {
            // Đọc tất cả dòng từ file userAgent.txt
            string[] userAgents = File.ReadAllLines(userAgentFilePath);

            // Kiểm tra nếu file rỗng
            if (userAgents.Length == 0)
            {
                Console.WriteLine("File userAgent.txt rỗng. Trả về User-Agent mặc định.");
                return "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";
            }

            // Lọc chỉ các User-Agent Chrome desktop
            var chromeUserAgents = userAgents
                .Where(ua => !string.IsNullOrWhiteSpace(ua) &&
                             ua.Contains("Chrome") &&
                             (ua.Contains("Windows NT") || ua.Contains("Macintosh") || ua.Contains("Linux x86_64")) &&
                             !ua.Contains("Mobile") && !ua.Contains("iPhone") && !ua.Contains("iPad") && !ua.Contains("Android"))
                .ToList();

            // Kiểm tra nếu không có User-Agent Chrome desktop
            if (chromeUserAgents.Count == 0)
            {
                Console.WriteLine("Không tìm thấy User-Agent Chrome desktop. Trả về User-Agent mặc định.");
                return "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";
            }

            // Chọn ngẫu nhiên một User-Agent
            Random random = new Random();
            string selectedUserAgent = chromeUserAgents[random.Next(chromeUserAgents.Count)].Trim();
            return selectedUserAgent;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi đọc userAgent.txt: {ex.Message}. Trả về User-Agent mặc định.");
            return "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";
        }
    }

    // Hàm đọc thông tin tài khoản từ file
    static (string user, string mail, string password, string proxyUser, string proxyPass, string proxyAddress) LoadAccountInfo(int index)
    {
        // Đọc file chứa thông tin email và mật khẩu
        string mailPass = "../../../mailPass.txt";
        string[] linesMailPass = File.ReadAllLines(mailPass);

        // Đọc file chứa thông tin proxy
        string proxyfile = "../../../proxyfile.txt";
        string[] linesProxyfile = File.ReadAllLines(proxyfile);

        // Tách thông tin tài khoản
        string[] mailPassParts = linesMailPass[index].Split('|');
        string[] proxyfileParts = linesProxyfile[index].Split(':');

        // Tạo username ngẫu nhiên
        string user = GenerateRandomUsername();
        string mail = mailPassParts[0];
        string password = mailPassParts[1];
        string proxyUser = proxyfileParts[2];
        string proxyPass = proxyfileParts[3];
        string proxyAddress = proxyfileParts[0] + ':' + proxyfileParts[1];

        // In thông tin để kiểm tra
        Console.WriteLine("Thông tin tài khoản:");
        Console.WriteLine($"Username: {user}");
        Console.WriteLine($"Email: {mail}");
        Console.WriteLine($"Mật khẩu: {password}");
        Console.WriteLine($"Proxy: {proxyAddress}");
        Console.WriteLine($"Proxy User: {proxyUser}");
        Console.WriteLine($"Proxy Pass: {proxyPass}");

        return (user, mail, password, proxyUser, proxyPass, proxyAddress);
    }

    // Hàm cấu hình ChromeDriver với các tùy chọn tránh phát hiện
    static (IWebDriver driver, Actions actions) ConfigureBrowser(string userAgent, string proxyAddress, string extensionFolderPath)
    {
        // Cấu hình proxy
        var proxy = new Proxy
        {
            HttpProxy = proxyAddress,
            SslProxy = proxyAddress
        };

        // Cấu hình ChromeOptions
        ChromeOptions options = new ChromeOptions();



        options.AddArgument($"--user-agent={userAgent}"); // Đặt User-Agent
        options.AddArgument("--disable-webrtc"); // Tắt WebRTC để tránh rò rỉ IP
        options.AddArgument("--disable-features=WebRtcHideLocalIpsWithMdns");
        options.AddArgument("--disable-blink-features=AutomationControlled"); // Ẩn dấu hiệu Selenium
        options.AddArgument("--lang=en-US"); // Đặt ngôn ngữ tiếng Anh
        options.AddArgument("--no-sandbox"); // Tăng ổn định
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--disable-notifications"); // Tắt thông báo bật lên
        options.AddArgument($"--load-extension={extensionFolderPath}"); // Tải extension CAPTCHA
        //options.AddArgument("--headless");
        options.AddArgument("--incognito"); // Chế độ ẩn danh
        options.Proxy = proxy;

        // Khởi tạo ChromeDriver
        IWebDriver driver = new ChromeDriver(options);
        Actions actions = new Actions(driver);

        // Ngẫu nhiên hóa kích thước cửa sổ để tránh dấu vân tay trình duyệt
        Random random = new Random();
        int[] widths = { 1920, 1366, 1440, 1600 };
        int[] heights = { 1080, 768, 900, 1050 };
        driver.Manage().Window.Size = new Size(widths[random.Next(widths.Length)], heights[random.Next(heights.Length)]);

        // Ghi đè navigator.webdriver để che giấu Selenium
        ((IJavaScriptExecutor)driver).ExecuteScript("Object.defineProperty(navigator, 'webdriver', { get: () => undefined });");

        return (driver, actions);
    }

    // Hàm xác thực proxy
    static void AuthenticateProxy(IWebDriver driver, string proxyUser, string proxyPass)
    {
        // Truy cập trang kiểm tra IP
        driver.Navigate().GoToUrl("https://api.ipify.org");
        RandomDelay(500, 2000);

        // Đóng cửa sổ thừa nếu có
        var windowHandles = driver.WindowHandles;
        Console.WriteLine($"Số lượng cửa sổ đang mở: {windowHandles.Count}");
        if (windowHandles.Count > 2)
        {
            string originalWindow = driver.CurrentWindowHandle;
            foreach (var handle in windowHandles)
            {
                if (handle != originalWindow)
                {
                    driver.SwitchTo().Window(handle);
                    driver.Close();
                    break;
                }
            }
            driver.SwitchTo().Window(originalWindow);
        }

        // Đợi trang tải và xác thực proxy
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        wait.Until(d => d.FindElement(By.TagName("body")));

        AutoItX.WinWaitActive("Proxy Authentication", "", 1);
        AutoItX.Send(proxyUser);
        AutoItX.Send("{TAB}");
        AutoItX.Send(proxyPass);
        AutoItX.Send("{ENTER}");

        RandomDelay(1000, 3000);
        Console.WriteLine("Proxy đang hoạt động.");
    }

    // Hàm lấy mã xác minh từ email
    static string GetVerificationCode(IWebDriver driver, string mail, string password)
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
                Thread.Sleep(3000);
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



    // Hàm đăng ký tài khoản trên x.com
    static int RegisterAccount(IWebDriver driver, Actions actions, string user, string mail, string[] birthValues, string password, string lastname)
    {
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        Random random = new Random();

        // Truy cập x.com
        driver.Navigate().GoToUrl("https://www.microsoft.com/en-us");
        RandomDelay(1000, 3000);

        // Click nút "Đăng ký"
        IWebElement signUpButton = driver.FindElement(By.XPath("/html/body/div[3]/div[1]/div/div/header/div/div/div[2]/div[2]/div/a/div"));
        actions.MoveToElement(signUpButton).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
        RandomDelay(1000, 3000);

        int truonghop = 0;

        // Click nút "Đăng ký"
        try
        {
            signUpButton = driver.FindElement(By.XPath("/html/body/div[1]/div/div/div/div/div[1]/div/div/div/div/form/div[2]/div/span/span/span"));
            actions.MoveToElement(signUpButton).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
            Console.WriteLine("Clicked first sign-up button successfully.");
            truonghop = 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"First sign-up button not found or clickable: {ex.Message}. Trying second button...");
            try
            {
                signUpButton = driver.FindElement(By.XPath("/html/body/div[1]/div/div/div/div[2]/div[1]/div/div/div/div[1]/div[2]/div/div/div/form/div[3]/div/div/span"));
                actions.MoveToElement(signUpButton).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
                Console.WriteLine("Clicked second sign-up button successfully.");
                truonghop = 2;
            }
            catch (Exception ex2)
            {
                Console.WriteLine($"Second sign-up button also failed: {ex2.Message}.");
                throw; // Ném lỗi để báo hiệu thất bại
            }
        }
        RandomDelay(1000, 3000);
        IWebElement emailInput;
        emailInput = (truonghop == 2) ? driver.FindElement(By.Id("usernameInput")) : driver.FindElement(By.CssSelector("input[type='email']"));

        actions.MoveToElement(emailInput).Click().Perform();
        foreach (char c in mail)
        {
            emailInput.SendKeys(c.ToString());
            RandomDelay(50, 200);
        }
        RandomDelay(1000, 3000);

        // Click nút "next"
        IWebElement nextUpButton = driver.FindElement(By.CssSelector("button[type='submit']"));
        actions.MoveToElement(nextUpButton).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
        RandomDelay(1000, 3000);

        if (truonghop == 2)
        {
            // Nhập mật khẩu
            IWebElement passwordInput = driver.FindElement(By.Id("Password"));
            new Actions(driver).MoveToElement(passwordInput).Click().Perform();
            foreach (char c in password)
            {
                passwordInput.SendKeys(c.ToString());
                RandomDelay(50, 200);
            }
            RandomDelay(1000, 3000);

            // Click nút "next"
            nextUpButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            actions.MoveToElement(nextUpButton).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
            RandomDelay(1000, 3000);


            // Nhập username
            IWebElement nameInput = driver.FindElement(By.Id("firstNameInput"));
            actions.MoveToElement(nameInput).Click().Perform();
            foreach (char c in user)
            {
                nameInput.SendKeys(c.ToString());
                RandomDelay(50, 200);
            }
            RandomDelay(1000, 3000);

            // Nhập username
            nameInput = driver.FindElement(By.Id("lastNameInput"));
            actions.MoveToElement(nameInput).Click().Perform();
            foreach (char c in lastname)
            {
                nameInput.SendKeys(c.ToString());
                RandomDelay(50, 200);
            }
            RandomDelay(1000, 3000);

            // Click nút "next"
            nextUpButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            actions.MoveToElement(nextUpButton).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
            RandomDelay(1000, 3000);

            // Chọn tháng
            IWebElement monthSelect = driver.FindElement(By.Id("BirthMonth"));
            SelectElement monthDropdown = new SelectElement(monthSelect);
            monthDropdown.SelectByValue(birthValues[0]);  // Chọn March (tháng 3)

            // Chọn ngày (giả sử có trường "BirthDay" với id "BirthDay")
            IWebElement daySelect = driver.FindElement(By.Id("BirthDay"));
            SelectElement dayDropdown = new SelectElement(daySelect);
            dayDropdown.SelectByValue(birthValues[1]);  // Chọn ngày 15

            // Chọn năm (giả sử có trường "BirthYear" với id "BirthYear")
            IWebElement yearSelect = driver.FindElement(By.Id("BirthYear"));
            //SelectElement yearDropdown = new SelectElement(yearSelect);
            yearSelect.SendKeys(birthValues[2]);  // Chọn năm 1990
            RandomDelay(1000, 3000);

            // Click nút "next"
            nextUpButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            actions.MoveToElement(nextUpButton).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
            RandomDelay(1000, 3000);

            //((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
            //var windows = driver.WindowHandles;
            //driver.SwitchTo().Window(windows[1]);

            //string verificationCode = GetVerificationCode(driver, mail, password);

            //RandomDelay(1000, 3000);

            //IWebElement codeVe = driver.FindElement(By.Id("VerificationCode"));
            //codeVe.SendKeys(verificationCode);
            //RandomDelay(1000, 3000);

            //// Click nút "next"
            //nextUpButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            //actions.MoveToElement(nextUpButton).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
            //RandomDelay(1000, 3000);

        }
        //else if(truonghop == 1)
        //{
        //    ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
        //    var windows = driver.WindowHandles;
        //    driver.SwitchTo().Window(windows[1]);

        //    string verificationCode = GetVerificationCode(driver, mail, password);

        //    // Duyệt qua các ô nhập mã và nhập từng ký tự vào
        //    for (int i = 0; i < verificationCode.Length; i++)
        //    {
        //        // Tìm ô nhập liệu theo id, giả sử id của các ô nhập liệu theo mẫu "codeEntry-0", "codeEntry-1", ...
        //        string inputId = "codeEntry-" + i;
        //        IWebElement codeInput = driver.FindElement(By.Id(inputId));

        //        // Nhập ký tự vào ô
        //        codeInput.SendKeys(verificationCode[i].ToString());

        //        // Để đảm bảo mỗi lần nhập không quá nhanh, có thể thêm một độ trễ ngẫu nhiên
        //        RandomDelay(100, 200);
        //    }

        //    RandomDelay(1000, 3000); // Để đảm bảo có đủ thời gian để gửi mã
        //    // Click nút "next"
        //    nextUpButton = driver.FindElement(By.CssSelector("button[type='submit']"));
        //    actions.MoveToElement(nextUpButton).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
        //    RandomDelay(1000, 3000);
        //}



        //// Xử lý CAPTCHA
        //IWebElement iframeElement = driver.FindElement(By.Id("arkoseFrame"));
        //actions.MoveToElement(iframeElement).Perform();
        //driver.SwitchTo().Frame(iframeElement);
        //RandomDelay(1000, 2000);

        //iframeElement = driver.FindElement(By.CssSelector("iframe[src*='client-api.arkoselabs.com'][data-e2e='enforcement-frame']"));
        //driver.SwitchTo().Frame(iframeElement);
        //RandomDelay(1000, 2000);

        //iframeElement = driver.FindElement(By.Id("game-core-frame"));
        //driver.SwitchTo().Frame(iframeElement);
        //RandomDelay(1000, 2000);

        //IWebElement authenticate = driver.FindElement(By.XPath("//*[@id='root']/div/div[1]/button"));
        //actions.MoveToElement(authenticate).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
        //RandomDelay(15000, 25000); // Đợi lâu hơn cho CAPTCHA

        return truonghop;
    }


    // Hàm hoàn tất đăng ký sau khi có mã xác minh
    static void CompleteRegistration(IWebDriver driver, Actions actions, string code, int truonghop)
    {
        Random random = new Random();
        if (truonghop == 2)
        {

            IWebElement codeVe = driver.FindElement(By.Id("VerificationCode"));
            codeVe.SendKeys(code);
            RandomDelay(1000, 3000);

            // Click nút "next"
            IWebElement nextUpButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            actions.MoveToElement(nextUpButton).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
            RandomDelay(1000, 3000);

        }
        else if (truonghop == 1)
        {
            string verificationCode = code;

            // Duyệt qua các ô nhập mã và nhập từng ký tự vào
            for (int i = 0; i < verificationCode.Length; i++)
            {
                // Tìm ô nhập liệu theo id, giả sử id của các ô nhập liệu theo mẫu "codeEntry-0", "codeEntry-1", ...
                string inputId = "codeEntry-" + i;
                IWebElement codeInput = driver.FindElement(By.Id(inputId));

                // Nhập ký tự vào ô
                codeInput.SendKeys(verificationCode[i].ToString());

                // Để đảm bảo mỗi lần nhập không quá nhanh, có thể thêm một độ trễ ngẫu nhiên
                RandomDelay(100, 200);
            }

            RandomDelay(1000, 3000); // Để đảm bảo có đủ thời gian để gửi mã
                                     // Click nút "next"
            IWebElement nextUpButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            actions.MoveToElement(nextUpButton).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
            RandomDelay(1000, 3000);
        }
    }

static void Main()
    {
        int truonghop = 0;
        // Đường dẫn đến extension CAPTCHA
        string extensionFolderPath = "C:\\Users\\vongu\\OneDrive\\Desktop\\tool\\Betacaptcha2";
        int index = 3; // Chỉ số tài khoản trong file

        try
        {
            // Bước 1: Đọc thông tin tài khoản
            var (user, mail, password, proxyUser, proxyPass, proxyAddress) = LoadAccountInfo(index);

            // Bước 2: Lấy User-Agent ngẫu nhiên
            string userAgent = GetRandomUserAgent();
            Console.WriteLine($"User-Agent: {userAgent}");

            // Bước 3: Tạo ngày sinh ngẫu nhiên
            string[] birthValues = GenerateRandomBirthDate();

            // Bước 4: Cấu hình trình duyệt
            var (driver, actions) = ConfigureBrowser(userAgent, proxyAddress, extensionFolderPath);
            string lastname = GetRandomLastName();
            try
            {
                // Xóa cookies để tăng tính riêng tư
                driver.Manage().Cookies.DeleteAllCookies();

                // Bước 5: Xác thực proxy
                AuthenticateProxy(driver, proxyUser, proxyPass);

                // Bước 6: Đăng ký tài khoản trên x.com
                truonghop = RegisterAccount(driver, actions, user, mail, birthValues, password, lastname);

                // Bước 7: Lấy mã xác minh
                ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
                var windows = driver.WindowHandles;
                driver.SwitchTo().Window(windows[1]);
                string code = GetVerificationCode(driver, mail, password);

                // Bước 8: Hoàn tất đăng ký
                driver.SwitchTo().Window(windows[0]);
                if (!string.IsNullOrEmpty(code))
                {
                    CompleteRegistration(driver, actions, code, truonghop);
                }
                else
                {
                    Console.WriteLine("Không lấy được mã xác minh, bỏ qua bước hoàn tất.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong quá trình đăng ký: {ex.Message}");
            }
            finally
            {
                // Đóng trình duyệt
                // driver.Quit();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi tải thông tin tài khoản: {ex.Message}");
        }
    }
}