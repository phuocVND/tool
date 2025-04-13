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
using System.Linq.Expressions;

class Program
{
    // Hàm tạo độ trễ ngẫu nhiên để mô phỏng hành vi con người
    static void RandomDelay(int minMs, int maxMs)
    {
        // Tạo số ngẫu nhiên trong khoảng minMs đến maxMs
        Random random = new Random();
        // Tạm dừng thread trong khoảng thời gian ngẫu nhiên
        Thread.Sleep(random.Next(minMs, maxMs));
    }

    // Hàm lấy họ ngẫu nhiên từ danh sách
    static string GetRandomLastName()
    {
        // Danh sách các họ phổ biến
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

        // Sử dụng thời gian hiện tại làm seed để tạo số ngẫu nhiên
        Random random = new Random((int)DateTime.Now.Ticks);
        // Lấy chỉ số ngẫu nhiên trong danh sách họ
        int index = random.Next(lastNames.Count);
        // Trả về họ được chọn ngẫu nhiên
        return lastNames[index];
    }

    // Hàm tạo username ngẫu nhiên
    static string GenerateRandomUsername()
    {
        // Khởi tạo đối tượng Random và StringBuilder
        Random random = new Random();
        StringBuilder username = new StringBuilder();
        // Danh sách ký tự chữ cái và số được phép
        string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string numbers = "0123456789";
        string allowedChars = letters + numbers + "_";

        // Xác định độ dài username ngẫu nhiên từ 6 đến 15 ký tự
        int length = random.Next(6, 16);

        // Bắt đầu username bằng một chữ cái
        username.Append(letters[random.Next(letters.Length)]);

        // Thêm các ký tự ngẫu nhiên vào username
        for (int i = 1; i < length - 2; i++)
        {
            username.Append(allowedChars[random.Next(allowedChars.Length)]);
        }

        // Kết thúc username bằng một số
        username.Append(numbers[random.Next(numbers.Length)]);

        // Thêm số ngẫu nhiên từ 2 đến 4 chữ số để tăng tính độc đáo
        username.Append(random.Next(10, 10000));

        // Trả về username dạng chuỗi
        return username.ToString();
    }

    // Hàm tạo ngày sinh ngẫu nhiên
    static string[] GenerateRandomBirthDate()
    {
        // Lấy ngày hiện tại
        DateTime currentDate = DateTime.Now;
        Random random = new Random();
        // Tạo tuổi ngẫu nhiên từ 18 đến 40
        int age = random.Next(18, 41);
        // Tính năm sinh dựa trên tuổi
        int birthYear = currentDate.Year - age;
        // Tạo tháng sinh ngẫu nhiên từ 1 đến 12
        int birthMonth = random.Next(1, 13);
        // Lấy số ngày tối đa của tháng/năm đó
        int maxDays = DateTime.DaysInMonth(birthYear, birthMonth);
        // Tạo ngày sinh ngẫu nhiên
        int birthDay = random.Next(1, maxDays + 1);
        // Trả về mảng chứa tháng, ngày, năm sinh
        return new string[] { birthMonth.ToString(), birthDay.ToString(), birthYear.ToString() };
    }

    // Hàm lấy User-Agent ngẫu nhiên từ file
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

            // Lọc các User-Agent thuộc Chrome desktop
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

            // Chọn một User-Agent ngẫu nhiên từ danh sách
            Random random = new Random();
            string selectedUserAgent = chromeUserAgents[random.Next(chromeUserAgents.Count)].Trim();
            return selectedUserAgent;
        }
        catch (Exception ex)
        {
            // Xử lý lỗi nếu không đọc được file
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

        // Tách thông tin tài khoản từ dòng thứ index
        string[] mailPassParts = linesMailPass[index].Split('|');
        string[] proxyfileParts = linesProxyfile[index].Split(':');

        // Tạo username ngẫu nhiên
        string user = GenerateRandomUsername();
        string mail = mailPassParts[0];
        string password = mailPassParts[1];
        string proxyUser = proxyfileParts[2];
        string proxyPass = proxyfileParts[3];
        string proxyAddress = proxyfileParts[0] + ':' + proxyfileParts[1];

        // In thông tin tài khoản để kiểm tra
        Console.WriteLine("Thông tin tài khoản:");
        Console.WriteLine($"Username: {user}");
        Console.WriteLine($"Email: {mail}");
        Console.WriteLine($"Mật khẩu: {password}");
        Console.WriteLine($"Proxy: {proxyAddress}");
        Console.WriteLine($"Proxy User: {proxyUser}");
        Console.WriteLine($"Proxy Pass: {proxyPass}");

        // Trả về tuple chứa thông tin tài khoản
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

        // Thêm các tùy chọn để tránh phát hiện bot
        options.AddArgument($"--user-agent={userAgent}"); // Đặt User-Agent
        options.AddArgument("--disable-webrtc"); // Tắt WebRTC để tránh rò rỉ IP
        options.AddArgument("--disable-features=WebRtcHideLocalIpsWithMdns");
        options.AddArgument("--disable-blink-features=AutomationControlled"); // Ẩn dấu hiệu Selenium
        options.AddArgument("--lang=en-US"); // Đặt ngôn ngữ tiếng Anh
        options.AddArgument("--no-sandbox"); // Tăng ổn định
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--disable-notifications"); // Tắt thông báo bật lên
        options.AddArgument($"--load-extension={extensionFolderPath}"); // Tải extension CAPTCHA
        //options.AddArgument("--headless"); // Chạy ẩn (đã comment)
        //options.AddArgument("--incognito"); // Chế độ ẩn danh (đã comment)
        options.Proxy = proxy;

        // Khởi tạo ChromeDriver với các tùy chọn
        IWebDriver driver = new ChromeDriver(options);
        Actions actions = new Actions(driver);

        // Ngẫu nhiên hóa kích thước cửa sổ trình duyệt
        Random random = new Random();
        int[] widths = { 1920, 1366, 1440, 1600 };
        int[] heights = { 1080, 768, 900, 1050 };
        driver.Manage().Window.Size = new Size(widths[random.Next(widths.Length)], heights[random.Next(heights.Length)]);

        // Ghi đè navigator.webdriver để che giấu Selenium
        ((IJavaScriptExecutor)driver).ExecuteScript("Object.defineProperty(navigator, 'webdriver', { get: () => undefined });");

        // Trả về tuple chứa driver và actions
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

        // Sử dụng AutoIt để nhập thông tin xác thực proxy
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
            // Truy cập trang mail.tm để lấy mã xác minh
            driver.Navigate().GoToUrl("https://mail.tm");
            Thread.Sleep(3000); // Chờ trang tải

            // Click nút "Mở"
            IWebElement openButton = driver.FindElement(By.XPath("/html/body/div[1]/div/div[2]/div/div/div[2]/button[3]"));
            openButton.Click();

            // Click nút "Login"
            IWebElement loginButton = driver.FindElement(By.XPath("/html/body/div[4]/div/div[2]/button[2]"));
            loginButton.Click();
            Thread.Sleep(1000); // Chờ trang tải

            // Nhập email
            IWebElement emailInput = driver.FindElement(By.XPath("/html/body/div[8]/div/div/div[1]/form/div[1]/div[2]/div/input"));
            emailInput.SendKeys(mail);

            // Nhập mật khẩu
            IWebElement passwordInput = driver.FindElement(By.XPath("/html/body/div[8]/div/div/div[1]/form/div[2]/div[2]/div/input"));
            passwordInput.SendKeys(password);
            Thread.Sleep(1000); // Chờ trang tải

            // Click nút đăng nhập
            IWebElement logButton = driver.FindElement(By.XPath("/html/body/div[8]/div/div/div[2]/span[1]/button"));
            logButton.Click();

            string codeText = "";
            Thread.Sleep(2000); // Chờ trang tải

            try
            {
                // Click vào email mới nhất
                IWebElement nextButton = driver.FindElement(By.XPath("/html/body/div[1]/div/div[2]/main/div[2]/div[2]/ul/li[1]/a/div/div/div[2]/div[1]/div[2]"));
                nextButton.Click();
                Thread.Sleep(1000); // Chờ trang tải
            }
            catch
            {
                // Làm mới trang nếu không tìm thấy email
                driver.Navigate().Refresh();
                Thread.Sleep(2000); // Chờ trang tải

                IWebElement nextButton = driver.FindElement(By.XPath("/html/body/div[1]/div/div[2]/main/div[2]/div[2]/ul/li[1]/a/div/div/div[2]/div[1]/div[2]"));
                nextButton.Click();
                Thread.Sleep(1000); // Chờ trang tải
            }

            try
            {
                // Chuyển sang iframe chứa mã xác minh
                IWebElement iframeElement = driver.FindElement(By.Id("iFrameResizer0"));
                driver.SwitchTo().Frame(iframeElement);

                // Lấy mã xác minh
                IWebElement code = driver.FindElement(By.XPath("/html/body/table/tbody/tr[4]/td/span"));
                codeText = code.Text;
            }
            catch
            {
                // Làm mới trang nếu không lấy được mã
                driver.Navigate().Refresh();
                Thread.Sleep(2000); // Chờ trang tải
            }
            return codeText;
        }
        catch
        {
            // Làm mới trang nếu có lỗi
            driver.Navigate().Refresh();
            Console.WriteLine("Đã xảy ra lỗi");
            return "";
        }
        finally
        {
            // Đóng trình duyệt
            driver.Close();
        }
    }

    // Hàm đăng ký tài khoản trên trang web
    static int RegisterAccount(IWebDriver driver, Actions actions, string user, string mail, string[] birthValues, string password, string lastname)
    {
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        Random random = new Random();

        // Truy cập trang web đăng ký
        driver.Navigate().GoToUrl("https://www.microsoft.com/en-us");
        RandomDelay(1000, 3000);

        // Click nút đăng ký
        IWebElement signUpButton = driver.FindElement(By.XPath("/html/body/div[3]/div[1]/div/div/header/div/div/div[2]/div[2]/div/a/div"));
        actions.MoveToElement(signUpButton).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
        RandomDelay(1000, 3000);

        int truonghop = 0;

        // Thử click nút đăng ký lần đầu
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
                // Thử click nút đăng ký lần hai nếu lần đầu thất bại
                signUpButton = driver.FindElement(By.XPath("/html/body/div[1]/div/div/div/div[2]/div[1]/div/div/div/div[1]/div[2]/div/div/div/form/div[3]/div/div/span"));
                actions.MoveToElement(signUpButton).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
                Console.WriteLine("Clicked second sign-up button successfully.");
                truonghop = 2;
            }
            catch (Exception ex2)
            {
                Console.WriteLine($"Second sign-up button also failed: {ex2.Message}.");
                throw; // Ném lỗi nếu cả hai lần đều thất bại
            }
        }
        RandomDelay(1000, 3000);

        // Nhập email vào trường nhập liệu
        IWebElement emailInput;
        emailInput = (truonghop == 2) ? driver.FindElement(By.Id("usernameInput")) : driver.FindElement(By.CssSelector("input[type='email']"));
        actions.MoveToElement(emailInput).Click().Perform();
        foreach (char c in mail)
        {
            emailInput.SendKeys(c.ToString());
            RandomDelay(50, 100);
        }
        RandomDelay(1000, 3000);

        // Click nút tiếp theo
        IWebElement nextUpButton = driver.FindElement(By.CssSelector("button[type='submit']"));
        actions.MoveToElement(nextUpButton).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
        RandomDelay(3000, 4000);

        if (truonghop == 2)
        {
            // Nhập mật khẩu
            IWebElement passwordInput = driver.FindElement(By.XPath("/html/body/div[2]/div/div/div/div[2]/div[1]/div/div/div/div/div[2]/div[2]/div/form/div[3]/div/div[1]/div/div/input"));
            new Actions(driver).MoveToElement(passwordInput).Click().Perform();
            foreach (char c in password)
            {
                passwordInput.SendKeys(c.ToString());
                RandomDelay(50, 100);
            }
            RandomDelay(1000, 3000);

            // Click nút tiếp theo
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

            // Nhập họ
            nameInput = driver.FindElement(By.Id("lastNameInput"));
            actions.MoveToElement(nameInput).Click().Perform();
            foreach (char c in lastname)
            {
                nameInput.SendKeys(c.ToString());
                RandomDelay(50, 200);
            }
            RandomDelay(1000, 3000);

            // Click nút tiếp theo
            nextUpButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            actions.MoveToElement(nextUpButton).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
            RandomDelay(1000, 3000);

            // Chọn tháng sinh
            IWebElement monthSelect = driver.FindElement(By.Id("BirthMonth"));
            SelectElement monthDropdown = new SelectElement(monthSelect);
            monthDropdown.SelectByValue(birthValues[0]);

            // Chọn ngày sinh
            IWebElement daySelect = driver.FindElement(By.Id("BirthDay"));
            SelectElement dayDropdown = new SelectElement(daySelect);
            dayDropdown.SelectByValue(birthValues[1]);

            // Nhập năm sinh
            IWebElement yearSelect = driver.FindElement(By.Id("BirthYear"));
            yearSelect.SendKeys(birthValues[2]);
            RandomDelay(1000, 3000);

            // Click nút tiếp theo
            nextUpButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            actions.MoveToElement(nextUpButton).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
            RandomDelay(1000, 3000);
        }

        RandomDelay(1000, 3000);
        return truonghop;
    }

    // Hàm hoàn tất đăng ký sau khi có mã xác minh
    static void CompleteRegistration(IWebDriver driver, Actions actions, string code, int truonghop)
    {
        Random random = new Random();
        if (truonghop == 2)
        {
            // Nhập mã xác minh
            IWebElement codeVe = driver.FindElement(By.Id("VerificationCode"));
            codeVe.SendKeys(code);
            RandomDelay(1000, 3000);

            // Click nút tiếp theo
            IWebElement nextUpButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            actions.MoveToElement(nextUpButton).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
            RandomDelay(1000, 3000);
        }
        else if (truonghop == 1)
        {
            string verificationCode = code;

            // Nhập từng ký tự mã xác minh vào các ô riêng
            for (int i = 0; i < verificationCode.Length; i++)
            {
                string inputId = "codeEntry-" + i;
                IWebElement codeInput = driver.FindElement(By.Id(inputId));
                codeInput.SendKeys(verificationCode[i].ToString());
                RandomDelay(100, 200);
            }

            RandomDelay(1000, 3000);

            // Click nút tiếp theo
            IWebElement nextUpButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            actions.MoveToElement(nextUpButton).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
            RandomDelay(1000, 3000);
        }
    }

    // Hàm chính xử lý quy trình đăng ký
    static void Main(int index) // Thêm tham số index
    {
        int truonghop = 0;
        // Đường dẫn đến extension CAPTCHA
        string extensionFolderPath = "C:\\Users\\lit\\Desktop\\tool\\Betacaptcha2";

        try
        {
            // Bước 1: Đọc thông tin tài khoản từ file với chỉ số index
            var (user, mail, password, proxyUser, proxyPass, proxyAddress) = LoadAccountInfo(index);

            // Bước 2: Lấy User-Agent ngẫu nhiên
            string userAgent = GetRandomUserAgent();
            Console.WriteLine($"User-Agent: {userAgent}");

            // Bước 3: Tạo ngày sinh ngẫu nhiên
            string[] birthValues = GenerateRandomBirthDate();

            // Bước 4: Cấu hình trình duyệt với proxy và extension
            var (driver, actions) = ConfigureBrowser(userAgent, proxyAddress, extensionFolderPath);
            string lastname = GetRandomLastName();
            try
            {
                // Xóa cookies để tránh bị theo dõi
                driver.Manage().Cookies.DeleteAllCookies();

                // Bước 5: Xác thực proxy
                AuthenticateProxy(driver, proxyUser, proxyPass);

                // Bước 6: Thực hiện đăng ký tài khoản
                truonghop = RegisterAccount(driver, actions, user, mail, birthValues, password, lastname);

                // Bước 7: Mở tab mới để lấy mã xác minh từ email
                ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
                var windows = driver.WindowHandles;
                driver.SwitchTo().Window(windows[1]);
                string code = GetVerificationCode(driver, mail, password);

                // Bước 8: Quay lại tab chính và hoàn tất đăng ký nếu có mã xác minh
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
                // Đóng trình duyệt (đã comment trong code gốc)
                 driver.Quit();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi tải thông tin tài khoản: {ex.Message}");
        }
    }

    // Hàm Main gốc để chạy vòng lặp
    static void Main(string[] args)
    {
        try
        {
            // Đọc file proxyfile.txt và đếm số dòng
            string proxyfile = "../../../proxyfile.txt";
            int n = File.ReadAllLines(proxyfile).Length;

            // Chạy vòng lặp n lần, mỗi lần gọi hàm Main với index tương ứng
            for (int i = 0; i < n; i++)
            {
                Console.WriteLine($"Đang xử lý tài khoản thứ {i + 1}/{n}");
                Main(i); // Gọi hàm Main với index = i
                RandomDelay(5000, 10000); // Độ trễ giữa các lần đăng ký để tránh bị chặn
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi đọc file proxyfile.txt hoặc chạy vòng lặp: {ex.Message}");
        }
    }
}