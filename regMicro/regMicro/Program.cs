using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using System;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Linq;

class Program
{
    // Hàm thiết lập proxy bằng Chrome extension
    static string SetupProxy(string proxyHost, string proxyPort, string proxyUser, string proxyPass)
    {
        string proxyExtensionPath = Path.Combine(Directory.GetCurrentDirectory(), "proxy_auth_extension");

        try
        {
            if (!Directory.Exists(proxyExtensionPath))
            {
                Directory.CreateDirectory(proxyExtensionPath);
            }

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

            File.WriteAllText(Path.Combine(proxyExtensionPath, "manifest.json"), manifestJson);
            File.WriteAllText(Path.Combine(proxyExtensionPath, "background.js"), backgroundJs);

            return proxyExtensionPath;
        }
        catch (Exception ex)
        {
            if (Directory.Exists(proxyExtensionPath))
            {
                Directory.Delete(proxyExtensionPath, true);
            }
            throw new Exception($"Lỗi khi thiết lập proxy: {ex.Message}");
        }
    }

    // Hàm tạo độ trễ ngẫu nhiên
    static void RandomDelay(int minMs, int maxMs)
    {
        Random random = new Random();
        Thread.Sleep(random.Next(minMs, maxMs));
    }

    // Hàm lấy họ ngẫu nhiên
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

        Random random = new Random((int)DateTime.Now.Ticks);
        int index = random.Next(lastNames.Count);
        return lastNames[index];
    }

    // Hàm tạo username ngẫu nhiên
    static string GenerateRandomUsername()
    {
        Random random = new Random();
        StringBuilder username = new StringBuilder();
        string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string numbers = "0123456789";
        string allowedChars = letters + numbers + "_";

        int length = random.Next(6, 16);
        username.Append(letters[random.Next(letters.Length)]);

        for (int i = 1; i < length - 2; i++)
        {
            username.Append(allowedChars[random.Next(allowedChars.Length)]);
        }

        username.Append(numbers[random.Next(numbers.Length)]);
        username.Append(random.Next(10, 10000));

        return username.ToString();
    }

    // Hàm tạo ngày sinh ngẫu nhiên
    static string[] GenerateRandomBirthDate()
    {
        DateTime currentDate = DateTime.Now;
        Random random = new Random();
        int age = random.Next(18, 41);
        int birthYear = currentDate.Year - age;
        int birthMonth = random.Next(1, 13);
        int maxDays = DateTime.DaysInMonth(birthYear, birthMonth);
        int birthDay = random.Next(1, maxDays + 1);
        return new string[] { birthMonth.ToString(), birthDay.ToString(), birthYear.ToString() };
    }

    // Hàm lấy User-Agent ngẫu nhiên
    static string GetRandomUserAgent(string userAgentFilePath = "../../../userAgent.txt")
    {
        try
        {
            string[] userAgents = File.ReadAllLines(userAgentFilePath);

            if (userAgents.Length == 0)
            {
                Console.WriteLine("File userAgent.txt rỗng. Trả về User-Agent mặc định.");
                return "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";
            }

            var chromeUserAgents = userAgents
                .Where(ua => !string.IsNullOrWhiteSpace(ua) &&
                             ua.Contains("Chrome") &&
                             (ua.Contains("Windows NT") || ua.Contains("Macintosh") || ua.Contains("Linux x86_64")) &&
                             !ua.Contains("Mobile") && !ua.Contains("iPhone") && !ua.Contains("iPad") && !ua.Contains("Android"))
                .ToList();

            if (chromeUserAgents.Count == 0)
            {
                Console.WriteLine("Không tìm thấy User-Agent Chrome desktop. Trả về User-Agent mặc định.");
                return "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";
            }

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

    // Hàm đọc thông tin tài khoản
    static (string user, string mail, string password, string proxyUser, string proxyPass, string proxyAddress) LoadAccountInfo(int index)
    {
        string mailPass = "../../../mailPass.txt";
        string[] linesMailPass = File.ReadAllLines(mailPass);

        string proxyfile = "../../../proxyfile.txt";
        string[] linesProxyfile = File.ReadAllLines(proxyfile);

        string[] mailPassParts = linesMailPass[index].Split('|');
        string[] proxyfileParts = linesProxyfile[index].Split(':');

        string user = GenerateRandomUsername();
        string mail = mailPassParts[0];
        string password = mailPassParts[1];
        string proxyUser = proxyfileParts[2];
        string proxyPass = proxyfileParts[3];
        string proxyAddress = proxyfileParts[0] + ':' + proxyfileParts[1];

        Console.WriteLine("Thông tin tài khoản:");
        Console.WriteLine($"Username: {user}");
        Console.WriteLine($"Email: {mail}");
        Console.WriteLine($"Mật khẩu: {password}");
        Console.WriteLine($"Proxy: {proxyAddress}");
        Console.WriteLine($"Proxy User: {proxyUser}");
        Console.WriteLine($"Proxy Pass: {proxyPass}");

        return (user, mail, password, proxyUser, proxyPass, proxyAddress);
    }

    // Hàm cấu hình trình duyệt
    static (IWebDriver driver, Actions actions) ConfigureBrowser(string userAgent, string extensionFolderPath, string extensionProxyPath)
    {
        ChromeOptions options = new ChromeOptions();
        options.AddArgument($"--user-agent={userAgent}");
        options.AddArgument("--disable-webrtc");
        options.AddArgument("--disable-features=WebRtcHideLocalIpsWithMdns");
        options.AddArgument("--disable-blink-features=AutomationControlled");
        options.AddArgument("--lang=en-US");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--disable-notifications");
        options.AddArgument($"--load-extension={Path.GetFullPath(extensionFolderPath)},{Path.GetFullPath(extensionProxyPath)}");
        options.AddArgument($"--disable-extensions-except={Path.GetFullPath(extensionFolderPath)},{Path.GetFullPath(extensionProxyPath)}");
        options.AddArgument("--ignore-certificate-errors");

        IWebDriver driver = new ChromeDriver(options);
        Actions actions = new Actions(driver);

        Random random = new Random();
        int[] widths = { 1920, 1366, 1440, 1600 };
        int[] heights = { 1080, 768, 900, 1050 };
        driver.Manage().Window.Size = new Size(widths[random.Next(widths.Length)], heights[random.Next(heights.Length)]);

        ((IJavaScriptExecutor)driver).ExecuteScript("Object.defineProperty(navigator, 'webdriver', { get: () => undefined });");

        return (driver, actions);
    }

    // Hàm xác thực proxy
    static void AuthenticateProxy(IWebDriver driver, string proxyUser, string proxyPass, string proxyHost)
    {
        RandomDelay(500, 2000);
        driver.Navigate().GoToUrl("https://api.ipify.org");
        RandomDelay(2000, 3000);

        var windowHandles = driver.WindowHandles;
        Console.WriteLine($"Số lượng cửa sổ đang mở: {windowHandles.Count}");
        if (windowHandles.Count > 1)
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

        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        IWebElement body = wait.Until(d => d.FindElement(By.TagName("body")));

        Console.WriteLine(body.Text);
        if (body.Text == proxyHost)
        {
            Console.WriteLine("Proxy đang hoạt động.");
        }
    }

    // Hàm lấy mã xác minh từ email
    static string GetVerificationCode(IWebDriver driver, string mail, string password)
    {
        try
        {
            driver.Navigate().GoToUrl("https://mail.tm");
            RandomDelay(2000, 5000);

            IWebElement openButton = driver.FindElement(By.XPath("/html/body/div[1]/div/div[2]/div/div/div[2]/button[3]"));
            new Actions(driver).MoveToElement(openButton).Pause(TimeSpan.FromMilliseconds(new Random().Next(100, 300))).Click().Perform();
            RandomDelay(1000, 3000);

            IWebElement loginButton = driver.FindElement(By.XPath("/html/body/div[4]/div/div[2]/button[2]"));
            new Actions(driver).MoveToElement(loginButton).Pause(TimeSpan.FromMilliseconds(new Random().Next(100, 300))).Click().Perform();
            RandomDelay(1000, 3000);

            IWebElement emailInput = driver.FindElement(By.XPath("/html/body/div[8]/div/div/div[1]/form/div[1]/div[2]/div/input"));
            new Actions(driver).MoveToElement(emailInput).Click().Perform();
            foreach (char c in mail)
            {
                emailInput.SendKeys(c.ToString());
                RandomDelay(50, 200);
            }
            RandomDelay(1000, 3000);

            IWebElement passwordInput = driver.FindElement(By.XPath("/html/body/div[8]/div/div/div[1]/form/div[2]/div[2]/div/input"));
            new Actions(driver).MoveToElement(passwordInput).Click().Perform();
            foreach (char c in password)
            {
                passwordInput.SendKeys(c.ToString());
                RandomDelay(50, 200);
            }
            RandomDelay(1000, 3000);

            IWebElement logButton = driver.FindElement(By.XPath("/html/body/div[8]/div/div/div[2]/span[1]/button"));
            new Actions(driver).MoveToElement(logButton).Pause(TimeSpan.FromMilliseconds(new Random().Next(100, 300))).Click().Perform();
            RandomDelay(2000, 5000);

            string codeText = "";
            try
            {
                IWebElement nextButton = driver.FindElement(By.XPath("/html/body/div[1]/div/div[2]/main/div[2]/div[2]/ul/li[1]/a/div/div/div[2]/div[1]/div[2]"));
                new Actions(driver).MoveToElement(nextButton).Pause(TimeSpan.FromMilliseconds(new Random().Next(100, 300))).Click().Perform();
                RandomDelay(1000, 3000);

                IWebElement iframeElement = driver.FindElement(By.XPath("/html/body/div[1]/div/div[2]/main/div[2]/div[2]/div[2]/div/iframe"));
                driver.SwitchTo().Frame(iframeElement);
                RandomDelay(1000, 2000);

                IWebElement code = driver.FindElement(By.XPath("/html/body/table/tbody/tr[1]/td/table/tbody/tr[1]/td/table[1]/tbody/tr/td[2]/table/tbody/tr[10]/td"));
                Console.WriteLine($"Mã xác minh: {code.Text}");
                codeText = code.Text;
            }
            catch
            {
                driver.Navigate().Refresh();
                RandomDelay(2000, 5000);
                IWebElement nextButton = driver.FindElement(By.XPath("/html/body/div[1]/div/div[2]/main/div[2]/div[2]/ul/li[1]/embra/div/div/div[2]/div[1]/div[2]"));
                new Actions(driver).MoveToElement(nextButton).Pause(TimeSpan.FromMilliseconds(new Random().Next(100, 300))).Click().Perform();
                RandomDelay(1000, 3000);

                IWebElement iframeElement = driver.FindElement(By.XPath("/html/body/div[1]/div/div[2]/main/div[2]/div[2]/div[2]/div/iframe"));
                driver.SwitchTo().Frame(iframeElement);
                RandomDelay(1000, 2000);

                //IWebElement code = driver.FindElement(By.XPath("/html/body/table/tbody/tr[1]/td/table/tbody/tr[1]/td/table[1]/tbody/tr/td[2]/table/tbody/tr[10]/td"));
                IWebElement iframe = driver.FindElement(By.Id("iFrameResizer0"));
                driver.SwitchTo().Frame(iframe);
                IWebElement code = driver.FindElement(By.XPath("/html/body/table/tbody/tr[4]/td/span"));
                Console.WriteLine($"Mã xác minh: {code.Text}");
                codeText = code.Text;
            }

            return codeText;
        }
        catch
        {
            Console.WriteLine("Lỗi khi lấy mã xác minh");
            return "";
        }
        finally
        {
            driver.Close();
        }
    }

    // Hàm đăng ký tài khoản trên Microsoft
    static int RegisterAccount(IWebDriver driver, Actions actions, string user, string mail, string[] birthValues, string password, string lastname)
    {
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        Random random = new Random();

        driver.Navigate().GoToUrl("https://www.microsoft.com/en-us");
        RandomDelay(1000, 3000);

        IWebElement signUpButton = driver.FindElement(By.XPath("/html/body/div[3]/div[1]/div/div/header/div/div/div[2]/div[2]/div/a/div"));
        actions.MoveToElement(signUpButton).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
        RandomDelay(1000, 3000);

        int truonghop = 0;

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
                throw;
            }
        }
        RandomDelay(1000, 3000);

        IWebElement emailInput;
        emailInput = (truonghop == 2) ? driver.FindElement(By.Id("usernameInput")) : driver.FindElement(By.CssSelector("input[type='email']"));
        actions.MoveToElement(emailInput).Click().Perform();
        foreach (char c in mail)
        {
            emailInput.SendKeys(c.ToString());
            RandomDelay(50, 100);
        }
        RandomDelay(1000, 3000);

        IWebElement nextUpButton = driver.FindElement(By.CssSelector("button[type='submit']"));
        actions.MoveToElement(nextUpButton).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
        RandomDelay(3000, 4000);

        if (truonghop == 2)
        {
            IWebElement passwordInput = driver.FindElement(By.XPath("/html/body/div[2]/div/div/div/div[2]/div[1]/div/div/div/div/div[2]/div[2]/div/form/div[3]/div/div[1]/div/div/input"));
            new Actions(driver).MoveToElement(passwordInput).Click().Perform();
            foreach (char c in password)
            {
                passwordInput.SendKeys(c.ToString());
                RandomDelay(50, 100);
            }
            RandomDelay(1000, 3000);

            nextUpButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            actions.MoveToElement(nextUpButton).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
            RandomDelay(1000, 3000);

            IWebElement nameInput = driver.FindElement(By.Id("firstNameInput"));
            actions.MoveToElement(nameInput).Click().Perform();
            foreach (char c in user)
            {
                nameInput.SendKeys(c.ToString());
                RandomDelay(50, 200);
            }
            RandomDelay(1000, 3000);

            nameInput = driver.FindElement(By.Id("lastNameInput"));
            actions.MoveToElement(nameInput).Click().Perform();
            foreach (char c in lastname)
            {
                nameInput.SendKeys(c.ToString());
                RandomDelay(50, 200);
            }
            RandomDelay(1000, 3000);

            nextUpButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            actions.MoveToElement(nextUpButton).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
            RandomDelay(1000, 3000);

            IWebElement monthSelect = driver.FindElement(By.Id("BirthMonth"));
            SelectElement monthDropdown = new SelectElement(monthSelect);
            monthDropdown.SelectByValue(birthValues[0]);

            IWebElement daySelect = driver.FindElement(By.Id("BirthDay"));
            SelectElement dayDropdown = new SelectElement(daySelect);
            dayDropdown.SelectByValue(birthValues[1]);

            IWebElement yearSelect = driver.FindElement(By.Id("BirthYear"));
            yearSelect.SendKeys(birthValues[2]);
            RandomDelay(1000, 3000);

            nextUpButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            actions.MoveToElement(nextUpButton).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
            RandomDelay(1000, 3000);
        }

        RandomDelay(1000, 3000);
        return truonghop;
    }

    // Hàm hoàn tất đăng ký
    static void CompleteRegistration(IWebDriver driver, Actions actions, string code, int truonghop)
    {
        Random random = new Random();
        if (truonghop == 2)
        {
            IWebElement codeVe = driver.FindElement(By.Id("VerificationCode"));
            codeVe.SendKeys(code);
            RandomDelay(1000, 3000);

            IWebElement nextUpButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            actions.MoveToElement(nextUpButton).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
            RandomDelay(1000, 3000);
        }
        else if (truonghop == 1)
        {
            string verificationCode = code;

            for (int i = 0; i < verificationCode.Length; i++)
            {
                string inputId = "codeEntry-" + i;
                IWebElement codeInput = driver.FindElement(By.Id(inputId));
                codeInput.SendKeys(verificationCode[i].ToString());
                RandomDelay(100, 200);
            }

            RandomDelay(1000, 3000);

            IWebElement nextUpButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            actions.MoveToElement(nextUpButton).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
            RandomDelay(1000, 3000);
        }
    }

    // Hàm chính xử lý quy trình đăng ký
    static void Main(int index)
    {
        int truonghop = 0;
        string extensionFolderPath = "C:\\Users\\lit\\Desktop\\tool\\Betacaptcha2";

        try
        {
            var (user, mail, password, proxyUser, proxyPass, proxyAddress) = LoadAccountInfo(index);
            string userAgent = GetRandomUserAgent();
            Console.WriteLine($"User-Agent: {userAgent}");

            string[] birthValues = GenerateRandomBirthDate();
            string[] proxyParts = proxyAddress.Split(':');
            string proxyHost = proxyParts[0];
            string proxyPort = proxyParts[1];

            string extensionProxyPath = SetupProxy(proxyHost, proxyPort, proxyUser, proxyPass);
            var (driver, actions) = ConfigureBrowser(userAgent, extensionFolderPath, extensionProxyPath);
            string lastname = GetRandomLastName();

            try
            {
                driver.Manage().Cookies.DeleteAllCookies();
                AuthenticateProxy(driver, proxyUser, proxyPass, proxyHost);
                truonghop = RegisterAccount(driver, actions, user, mail, birthValues, password, lastname);

                ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
                var windows = driver.WindowHandles;
                driver.SwitchTo().Window(windows[1]);
                string code = GetVerificationCode(driver, mail, password);

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
                driver.Quit();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi tải thông tin tài khoản: {ex.Message}");
        }
    }

    // Hàm Main gốc
    static void Main(string[] args)
    {
        try
        {
            string proxyfile = "../../../proxyfile.txt";
            int n = File.ReadAllLines(proxyfile).Length;

            for (int i = 0; i < n; i++)
            {
                Console.WriteLine($"Đang xử lý tài khoản thứ {i + 1}/{n}");
                Main(i);
                RandomDelay(5000, 10000);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi đọc file proxyfile.txt hoặc chạy vòng lặp: {ex.Message}");
        }
    }
}