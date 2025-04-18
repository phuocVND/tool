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
    // Khai báo các đường dẫn toàn cục
    private static readonly string UserAgentFilePath = "../../../userAgent.txt";
    private static readonly string MailPassFilePath = "../../../mailPass.txt";
    private static readonly string ProxyFilePath = "../../../proxyfile.txt";
    private static readonly string ExtensionFolderPath = @"C:\Users\lit\Desktop\tool\Betacaptcha2";

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

    static void RandomDelay(int minMs, int maxMs)
    {
        Random random = new Random();
        Thread.Sleep(random.Next(minMs, maxMs));
    }

    static string GenerateRandomUsername()
    {
        Random random = new Random();
        StringBuilder username = new StringBuilder();
        string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string numbers = "0123456789";
        string allowedChars = letters + numbers + "_";

        int length = random.Next(3, 8);
        username.Append(letters[random.Next(letters.Length)]);

        for (int i = 1; i < length - 2; i++)
        {
            username.Append(allowedChars[random.Next(allowedChars.Length)]);
        }

        username.Append(numbers[random.Next(numbers.Length)]);
        username.Append(random.Next(10, 10000));

        return username.ToString();
    }

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

    static string GetRandomUserAgent()
    {
        try
        {
            string[] userAgents = File.ReadAllLines(UserAgentFilePath);

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

    static (string user, string mail, string password, string proxyUser, string proxyPass, string proxyAddress) LoadAccountInfo(int index)
    {
        string[] linesMailPass = File.ReadAllLines(MailPassFilePath);
        string[] linesProxyfile = File.ReadAllLines(ProxyFilePath);

        string[] mailPassParts = linesMailPass[index].Split('|');
        string[] proxyfileParts = linesProxyfile[index].Split(':');

        string user = GetRandomLastName() + "_" + GenerateRandomUsername();
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

    static void AuthenticateProxy(IWebDriver driver, string proxyUser, string proxyPass, string proxyHost)
    {
        RandomDelay(500, 2000);
        driver.Navigate().GoToUrl("https://api.ipify.org");
        RandomDelay(500, 2000);

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
                RandomDelay(10, 50);
            }
            RandomDelay(1000, 3000);

            IWebElement passwordInput = driver.FindElement(By.XPath("/html/body/div[8]/div/div/div[1]/form/div[2]/div[2]/div/input"));
            new Actions(driver).MoveToElement(passwordInput).Click().Perform();
            foreach (char c in password)
            {
                passwordInput.SendKeys(c.ToString());
                RandomDelay(10, 50);
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

    static void RegisterAccount(IWebDriver driver, Actions actions, string user, string mail, string[] birthValues, string password)
    {
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        Random random = new Random();

        driver.Navigate().GoToUrl("https://x.com");
        RandomDelay(1000, 3000);

        IWebElement signUpButton = driver.FindElement(By.XPath("//*[@id='react-root']/div/div/div[2]/main/div/div/div[1]/div/div/div[3]/a/div"));
        actions.MoveToElement(signUpButton).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
        RandomDelay(1000, 3000);

        IWebElement nameInput = driver.FindElement(By.XPath("//input[@name='name' and contains(@class, 'r-30o5oe')]"));
        actions.MoveToElement(nameInput).Click().Perform();
        foreach (char c in user)
        {
            nameInput.SendKeys(c.ToString());
            RandomDelay(10, 50);
        }
        RandomDelay(1000, 3000);

        IWebElement emailInput = driver.FindElement(By.XPath("//input[@name='email' and @type='email']"));
        actions.MoveToElement(emailInput).Click().Perform();
        foreach (char c in mail)
        {
            emailInput.SendKeys(c.ToString());
            RandomDelay(10, 50);
        }
        RandomDelay(1000, 3000);

        var selectElements = driver.FindElements(By.XPath("//select[contains(@class, 'r-30o5oe')]"));
        for (int i = 0; i < selectElements.Count; i++)
        {
            actions.MoveToElement(selectElements[i]).Click().Perform();
            SelectElement select = new SelectElement(selectElements[i]);
            select.SelectByValue(birthValues[i]);
            RandomDelay(10, 50);
        }
        RandomDelay(1000, 3000);

        IWebElement nextButton = driver.FindElement(By.XPath("//div[@class='css-175oi2r r-b9tw7p']/button"));
        actions.MoveToElement(nextButton).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
        RandomDelay(3000, 6000);

        IWebElement iframeElement = driver.FindElement(By.Id("arkoseFrame"));
        actions.MoveToElement(iframeElement).Perform();
        driver.SwitchTo().Frame(iframeElement);
        RandomDelay(1000, 2000);

        iframeElement = driver.FindElement(By.CssSelector("iframe[src*='client-api.arkoselabs.com'][data-e2e='enforcement-frame']"));
        driver.SwitchTo().Frame(iframeElement);
        RandomDelay(1000, 2000);

        iframeElement = driver.FindElement(By.Id("game-core-frame"));
        driver.SwitchTo().Frame(iframeElement);
        RandomDelay(1000, 2000);
        bool check = false;
        do
        {
            try
            {
                IWebElement checkMail = driver.FindElement(By.XPath("/html/body/div/div/div/div[1]/div[2]/div/div/div/div/div/div[2]/div[2]/div/div/div[2]/div[2]/div[1]/div/div[1]/div/div/div/div/span[2]/span"));
                if (checkMail.Text != mail)
                {
                    Console.WriteLine("Delay Check");
                    check = true;
                }
            }
            catch (NoSuchElementException)
            {
                RandomDelay(3000, 5000);
            }
        } while (!check);
        ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
        var windows = driver.WindowHandles;
        driver.SwitchTo().Window(windows[1]);
        string code = GetVerificationCode(driver, mail, password);

        driver.SwitchTo().Window(windows[0]);
        if (!string.IsNullOrEmpty(code))
        {
            CompleteRegistration(driver, actions, code);
        }
        else
        {
            Console.WriteLine("Không lấy được mã xác minh, bỏ qua bước hoàn tất.");
        }
    }

    static void CompleteRegistration(IWebDriver driver, Actions actions, string code)
    {
        Random random = new Random();
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        var elements = driver.FindElements(By.XPath("/html/body/div/div/div/div[1]/div[2]/div/div/div/div/div/div[2]/div[2]/div/div/div[2]/div[2]/div[1]/div/div[2]/label/div/div[2]/div/input"));
        if (elements.Count > 0)
        {
            IWebElement codeVeri = elements[0];
            actions.MoveToElement(codeVeri).Click().Perform();
            foreach (char c in code)
            {
                codeVeri.SendKeys(c.ToString());
                RandomDelay(50, 200);
            }
            Console.WriteLine("Đã nhập mã xác minh thành công.");
            RandomDelay(1000, 3000);
        }

        IWebElement nextButton = driver.FindElement(By.XPath("/html/body/div/div/div/div[1]/div[2]/div/div/div/div/div/div[2]/div[2]/div/div/div[2]/div[2]/div[2]/div/div/div/button/div"));
        actions.MoveToElement(nextButton).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
        RandomDelay(2000, 5000);
    }

    static void Main()
    {
        int index = 1;

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
            var (driver, actions) = ConfigureBrowser(userAgent, ExtensionFolderPath, extensionProxyPath);

            try
            {
                driver.Manage().Cookies.DeleteAllCookies();
                AuthenticateProxy(driver, proxyUser, proxyPass, proxyHost);
                RegisterAccount(driver, actions, user, mail, birthValues, password);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong quá trình đăng ký: {ex.Message}");
            }
            finally
            {
                // driver.Quit();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi tải thông tin tài khoản: {ex.Message}");
        }
    }
}