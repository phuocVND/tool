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
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Drawing; // Đã có nếu bạn dùng Bitmap
using System.Drawing.Imaging; // Thêm dòng này
class Program
{
    // Khai báo các đường dẫn toàn cục
    private static readonly string UserAgentFilePath = "userAgent.txt";
    private static readonly string MailPassFilePath = "mailPass.txt";
    private static readonly string ProxyFilePath = "proxyfile.txt";
    private static readonly string ExtensionFolderPath = @"../../Betacaptcha2";
    private const string AccountXFilePath = "accountX.txt";


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

    static (string user, string mail, string password, string proxyUser, string proxyPass, string proxyAddress) LoadAccountInfo(int index, string[] mailPassLines, string[] proxyLines)
    {
        if (index >= mailPassLines.Length)
            throw new IndexOutOfRangeException($"Chỉ số {index} vượt quá số dòng trong mailPass.txt ({mailPassLines.Length})");

        int proxyIndex = index % proxyLines.Length;
        if (proxyIndex >= proxyLines.Length)
            throw new IndexOutOfRangeException($"Chỉ số proxy {proxyIndex} vượt quá số dòng trong proxyfile.txt ({proxyLines.Length})");

        string[] mailPassParts = mailPassLines[index].Split('|');
        if (mailPassParts.Length < 2)
            throw new FormatException($"Dòng {index + 1} trong mailPass.txt không đúng định dạng: {mailPassLines[index]}");

        string[] proxyParts = proxyLines[proxyIndex].Split(':');
        if (proxyParts.Length < 4)
            throw new FormatException($"Dòng proxy {proxyIndex + 1} không đúng định dạng: {proxyLines[proxyIndex]}");

        string user = GetRandomLastName() + "_" + GenerateRandomUsername();
        string mail = mailPassParts[0];
        string password = mailPassParts[1];
        string proxyUser = proxyParts[2];
        string proxyPass = proxyParts[3];
        string proxyAddress = proxyParts[0] + ':' + proxyParts[1];

        return (user, mail, password, proxyUser, proxyPass, proxyAddress);
    }

    static (IWebDriver driver, Actions actions) ConfigureBrowser(string userAgent, string extensionFolderPath, string extensionProxyPath)
    {
        ChromeOptions options = new ChromeOptions();
        
        // options.AddArgument("--headless"); // Tạm bỏ để debug
        options.AddArgument($"--user-agent={userAgent}");
        options.AddArgument("--disable-webrtc");
        options.AddArgument("--disable-features=WebRtcHideLocalIpsWithMdns");
        options.AddArgument("--disable-blink-features=AutomationControlled");
        options.AddArgument("--lang=en-US");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--disable-notifications");
        // options.AddArgument($"--load-extension={Path.GetFullPath(extensionFolderPath)},{Path.GetFullPath(extensionProxyPath)}");
        // options.AddArgument($"--disable-extensions-except={Path.GetFullPath(extensionFolderPath)},{Path.GetFullPath(extensionProxyPath)}");
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

    static void SaveCredentialsToFile(string credentials)
    {
        try
        {
            File.AppendAllText("accountX.txt", credentials + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi lưu thông tin: {ex.Message}");
        }
    }
    static void AuthenticateProxy(IWebDriver driver, string proxyUser, string proxyPass, string proxyHost)
    {
        RandomDelay(500, 2000);
        driver.Navigate().GoToUrl("https://api.ipify.org");
        RandomDelay(500, 2000);

        var windowHandles = driver.WindowHandles;
        
        if (windowHandles.Count > 1)
        {
            
            string originalWindow = driver.CurrentWindowHandle;

            foreach (var handle in windowHandles)
            {
                if (handle != originalWindow)
                {
                    
                    driver.SwitchTo().Window(handle);
                    RandomDelay(500, 2000);
                    driver.Close();
                    // break;
                }
                else{
                    break;
                }
            }
            driver.SwitchTo().Window(originalWindow);
        }

        Console.WriteLine($"Số lượng cửa sổ đang mở: {windowHandles.Count}");

        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        IWebElement body = wait.Until(d => d.FindElement(By.TagName("body")));

        // Console.WriteLine(body.Text);
        if (body.Text == proxyHost)
        {
            Console.WriteLine("Proxy đang hoạt động.");
        }
    }

    static bool WaitForElement(IWebDriver driver, By locator, int timeoutInSeconds)
    {
        int elapsed = 0;
        while (elapsed < timeoutInSeconds * 1000)
        {
            try
            {
                driver.FindElement(locator);
                return true;
            }
            catch (NoSuchElementException)
            {
                Thread.Sleep(500);
                elapsed += 500;
            }
        }
        return false;
    }

    static string GetVerificationCode(IWebDriver driver, string mail, string password)
    {
        try
        {
            driver.Navigate().GoToUrl("https://mail.tm");
            RandomDelay(2000, 3000);

            // Click nút mở menu
            By openButtonLocator = By.XPath("/html/body/div[1]/div/div[2]/div/div/div[2]/button[3]");
            RandomDelay(2000, 3000);
            if (!WaitForElement(driver, openButtonLocator, 10))
                throw new Exception("Không tìm thấy nút mở menu");
            IWebElement openButton = driver.FindElement(openButtonLocator);
            new Actions(driver).MoveToElement(openButton).Pause(TimeSpan.FromMilliseconds(new Random().Next(100, 300))).Click().Perform();
            RandomDelay(1000, 3000);

            // Click nút đăng nhập
            By loginButtonLocator = By.XPath("/html/body/div[4]/div/div[2]/button[2]");
            if (!WaitForElement(driver, loginButtonLocator, 3))
                throw new Exception("Không tìm thấy nút đăng nhập");
            IWebElement loginButton = driver.FindElement(loginButtonLocator);
            new Actions(driver).MoveToElement(loginButton).Pause(TimeSpan.FromMilliseconds(new Random().Next(100, 300))).Click().Perform();
            RandomDelay(1000, 3000);

            // Nhập email
            By emailInputLocator = By.Id("v-1-15");
            if (!WaitForElement(driver, emailInputLocator, 3))
                throw new Exception("Không tìm thấy ô nhập email");
            IWebElement emailInput = driver.FindElement(emailInputLocator);
            new Actions(driver).MoveToElement(emailInput).Click().Perform();
            foreach (char c in mail)
            {
                emailInput.SendKeys(c.ToString());
                RandomDelay(50, 200);
            }
            // RandomDelay(1000, 3000);

            // Nhập mật khẩu
            By passwordInputLocator = By.Id("v-1-16");
            if (!WaitForElement(driver, passwordInputLocator, 3))
                throw new Exception("Không tìm thấy ô nhập mật khẩu");
            IWebElement passwordInput = driver.FindElement(passwordInputLocator);
            new Actions(driver).MoveToElement(passwordInput).Click().Perform();
            foreach (char c in password)
            {
                passwordInput.SendKeys(c.ToString());
                RandomDelay(50, 200);
            }
            // RandomDelay(1000, 3000);

            // Click nút đăng nhập
            By logButtonLocator = By.CssSelector("button[type='button'].bg-indigo-600");;
            if (!WaitForElement(driver, logButtonLocator, 3))
                throw new Exception("Không tìm thấy nút đăng nhập");
            IWebElement logButton = driver.FindElement(logButtonLocator);
            new Actions(driver).MoveToElement(logButton).Pause(TimeSpan.FromMilliseconds(new Random().Next(100, 300))).Click().Perform();
            RandomDelay(2000, 5000);

            string codeText = "";
            try
            {
                // Click vào email mới nhất
                By nextButtonLocator = By.XPath("/html/body/div[1]/div/div[2]/main/div[2]/div[2]/ul/li[1]/a/div/div/div[2]/div[1]/div[2]");
                if (!WaitForElement(driver, nextButtonLocator, 10))
                    throw new Exception("Không tìm thấy email mới nhất");
                IWebElement nextButton = driver.FindElement(nextButtonLocator);
                new Actions(driver).MoveToElement(nextButton).Pause(TimeSpan.FromMilliseconds(new Random().Next(100, 300))).Click().Perform();
                RandomDelay(1000, 3000);

                // Chuyển vào iframe
                By iframeLocator = By.XPath("/html/body/div[1]/div/div[2]/main/div[2]/div[2]/div[2]/div/iframe");
                if (!WaitForElement(driver, iframeLocator, 10))
                    throw new Exception("Không tìm thấy iframe");
                IWebElement iframeElement = driver.FindElement(iframeLocator);
                driver.SwitchTo().Frame(iframeElement);
                // RandomDelay(1000, 2000);

                try{

                    // Lấy mã xác minh
                    By codeLocator = By.CssSelector("span[style*='font-weight:bold'][style*='font-size:14px']");
                    if (!WaitForElement(driver, codeLocator, 10))
                        throw new Exception("Không tìm thấy mã xác minh");
                    IWebElement code = driver.FindElement(codeLocator);
                    Console.WriteLine($"Mã xác minh: {code.Text}");
                    codeText = code.Text;
                }
                catch{
                    // Lấy mã xác minh
                    By codeLocator = By.CssSelector("td.h1.black[style*='font-size:32px']");
                    if (!WaitForElement(driver, codeLocator, 10))
                        throw new Exception("Không tìm thấy mã xác minh");
                    IWebElement code = driver.FindElement(codeLocator);
                    Console.WriteLine($"Mã xác minh: {code.Text}");
                    codeText = code.Text;
                }
            }
            catch
            {
                // Thử lại nếu không lấy được mã
                driver.Navigate().Refresh();
                RandomDelay(2000, 3000);

                // Click vào email mới nhất
                By nextButtonLocator = By.XPath("/html/body/div[1]/div/div[2]/main/div[2]/div[2]/ul/li[1]/a/div/div/div[2]/div[1]/div[2]");
                if (!WaitForElement(driver, nextButtonLocator, 10))
                    throw new Exception("Không tìm thấy email mới nhất");
                IWebElement nextButton = driver.FindElement(nextButtonLocator);
                new Actions(driver).MoveToElement(nextButton).Pause(TimeSpan.FromMilliseconds(new Random().Next(100, 300))).Click().Perform();
                RandomDelay(1000, 3000);

                // Chuyển vào iframe
                By iframeLocator = By.XPath("/html/body/div[1]/div/div[2]/main/div[2]/div[2]/div[2]/div/iframe");
                if (!WaitForElement(driver, iframeLocator, 10))
                    throw new Exception("Không tìm thấy iframe");
                IWebElement iframeElement = driver.FindElement(iframeLocator);
                driver.SwitchTo().Frame(iframeElement);
                // RandomDelay(1000, 2000);

                try{

                    // Lấy mã xác minh
                    By codeLocator = By.CssSelector("span[style*='font-weight:bold'][style*='font-size:14px']");
                    if (!WaitForElement(driver, codeLocator, 10))
                        throw new Exception("Không tìm thấy mã xác minh");
                    IWebElement code = driver.FindElement(codeLocator);
                    Console.WriteLine($"Mã xác minh: {code.Text}");
                    codeText = code.Text;
                }
                catch{
                    // Lấy mã xác minh
                    By codeLocator = By.CssSelector("td.h1.black[style*='font-size:32px']");
                    if (!WaitForElement(driver, codeLocator, 10))
                        throw new Exception("Không tìm thấy mã xác minh");
                    IWebElement code = driver.FindElement(codeLocator);
                    Console.WriteLine($"Mã xác minh: {code.Text}");
                    codeText = code.Text;
                }
            }

            return codeText;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi lấy mã xác minh: {ex.Message}");
            return "";
        }
        finally
        {
            driver.SwitchTo().DefaultContent();
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
        RandomDelay(2000, 3000);

        IWebElement nameInput = driver.FindElement(By.CssSelector("input[name='name']"));
        actions.MoveToElement(nameInput).Click().Perform();
        foreach (char c in user)
        {
            nameInput.SendKeys(c.ToString());
            RandomDelay(10, 50);
        }
        RandomDelay(1000, 3000);
        
        IWebElement emailInput = wait.Until(d => 
        {
            IWebElement element = d.FindElement(By.XPath("//input[@name='email' and @type='email']"));
            return element.Displayed ? element : null;
        });

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

        // // Tìm tất cả các iframe trên trang
        // var iframes = driver.FindElements(By.TagName("iframe"));

        // // Kiểm tra nếu có ít nhất một iframe
        // if (iframes.Count > 0)
        // {
        //     // Chuyển vào iframe đầu tiên
        //     driver.SwitchTo().Frame(iframes[0]);
        // }

        // iframes = driver.FindElements(By.TagName("iframe"));

        // // Kiểm tra nếu có ít nhất một iframe
        // if (iframes.Count > 0)
        // {
        //     // Chuyển vào iframe đầu tiên
        //     driver.SwitchTo().Frame(iframes[0]);
        // }

        // iframes = driver.FindElements(By.TagName("iframe"));

        // // Kiểm tra nếu có ít nhất một iframe
        // if (iframes.Count > 0)
        // {
        //     // Chuyển vào iframe đầu tiên
        //     driver.SwitchTo().Frame(iframes[0]);
        // }
        for(int i =0;i<3;i++)
        {   var iframes = driver.FindElements(By.TagName("iframe"));
            driver.SwitchTo().Frame(iframes[0]);
        }

        IWebElement auth = driver.FindElement(By.XPath("/html/body/div/div/div[1]/button"));
        actions.MoveToElement(auth).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
        RandomDelay(1000, 3000);
        haha:
        // Đường dẫn thư mục lưu ảnh
        string imageFolder = "image";
        if (!Directory.Exists(imageFolder))
        {
            Directory.CreateDirectory(imageFolder);
        }
        
        try
        {
            // Tìm thẻ <img>
            IWebElement imgElement = wait.Until(d => d.FindElement(By.CssSelector("img.sc-7csxyx-1.blHsFq")));

            // Lấy thuộc tính style
            string style = imgElement.GetAttribute("style");

            // Trích xuất blob URL từ background-image
            var match = Regex.Match(style, @"url\(""([^""]+)""\)");
            if (!match.Success)
            {
                throw new Exception("Không tìm thấy URL trong background-image");
            }

            string blobUrl = match.Groups[1].Value;

            // Lưu cửa sổ hiện tại
            string originalWindow = driver.CurrentWindowHandle;

            // Mở blob URL trong tab mới
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript($"window.open('{blobUrl}');");

            // Chuyển sang tab mới
            var windows = driver.WindowHandles;

            string newWindow = windows.FirstOrDefault(w => w != originalWindow);
            if (newWindow == null)
            {
                throw new Exception("Không mở được tab mới");
            }
            driver.SwitchTo().Window(newWindow);

            // Chờ ảnh tải trong tab mới
            RandomDelay(2000, 4000);

            // Chụp ảnh chỉ phần tử <img> trong tab mới
            IWebElement newImgElement = wait.Until(d => d.FindElement(By.TagName("img"))); // Tìm <img> trong tab mới
            Point location = newImgElement.Location;
            Size size = newImgElement.Size;

            Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
            string imagePath = Path.Combine(imageFolder, $"image_{DateTime.Now.Ticks}.png");

            // Chuyển dữ liệu ảnh thành MemoryStream và cắt phần tử
            using (MemoryStream memoryStream = new MemoryStream(screenshot.AsByteArray))
            {
                using (Bitmap fullImage = new Bitmap(memoryStream))
                {
                    // Xác định khu vực cần cắt
                    Rectangle cropArea = new Rectangle(location.X, location.Y, size.Width, size.Height);

                    // Tạo ảnh mới với khu vực đã cắt
                    using (Bitmap croppedImage = fullImage.Clone(cropArea, fullImage.PixelFormat))
                    {
                        // Lưu ảnh đã cắt
                        croppedImage.Save(imagePath, ImageFormat.Png);
                        Console.WriteLine($"Ảnh phần tử đã được lưu tại: {imagePath}");
                    }
                }
            }

            // Đóng tab mới và quay lại tab gốc
            driver.Close();
            driver.SwitchTo().Window(originalWindow);
        }
    catch (Exception ex)
    {
        Console.WriteLine($"Lỗi khi xử lý ảnh: {ex.Message}");
    }

    for(int i =0;i<3;i++)
    {   var iframes = driver.FindElements(By.TagName("iframe"));
        driver.SwitchTo().Frame(iframes[0]);
    }


    // IWebElement nextButton = driver.FindElement(By.XPath("/html/body/div/div/div[1]/div/div/div[2]/div[1]/a[2]"));
    // actions.MoveToElement(nextButton).Pause(TimeSpan.FromMilliseconds(random.Next(10, 50))).Click().Perform();
    
    try{
        for(int i =0;i<3;i++)
        {   var iframes = driver.FindElements(By.TagName("iframe"));
            driver.SwitchTo().Frame(iframes[0]);
        }
        IWebElement reset = driver.FindElement(By.XPath("/html/body/div/div/div[3]/button[2]"));
        actions.MoveToElement(reset).Pause(TimeSpan.FromMilliseconds(random.Next(10, 50))).Click().Perform();
        RandomDelay(2000, 3000);
    }
    catch{

        IWebElement submit = driver.FindElement(By.XPath("/html/body/div/div/div[1]/div/button"));
        actions.MoveToElement(submit).Pause(TimeSpan.FromMilliseconds(random.Next(10, 50))).Click().Perform();
        RandomDelay(1000, 3000);
    }
    

    goto haha;



        // bool check = false;
        // Stopwatch stopwatch = Stopwatch.StartNew(); // Bắt đầu đếm thời gian

        // do
        // {
        //     try
        //     {
        //         IWebElement checkMail = driver.FindElement(By.XPath("//span[contains(@class, 'css-1jxf684') and contains(text(), '@')]"));
        //         if (checkMail.Text == mail)
        //         {
        //             // Console.WriteLine($"{checkMail.Text}");
        //             check = true;
        //         }
        //         else{
        //             // Kiểm tra nếu thời gian vượt quá 2 phút (120000ms)
        //             if (stopwatch.ElapsedMilliseconds > 80000)
        //             {
        //                 Console.WriteLine("Timeout after 2 minutes. Cancelling...");
        //                 driver.Close();
        //             }
        //         }

        //     }
        //     catch (NoSuchElementException)
        //     {
        //         // Console.WriteLine("Delay Check ************");
        //         RandomDelay(3000, 5000);
        //     }

        // } while (!check);

        // stopwatch.Stop(); // Dừng đếm thời gian

        // ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
        // var windows = driver.WindowHandles;
        // driver.SwitchTo().Window(windows[1]);

        // string code = GetVerificationCode(driver, mail, password);

        // driver.SwitchTo().Window(windows[0]);
        // if (!string.IsNullOrEmpty(code))
        // {
        //     CompleteRegistration(driver, actions, code, password, mail);
        // }
        // else
        // {
        //     Console.WriteLine("Không lấy được mã xác minh, bỏ qua bước hoàn tất.");
        // }
    }

    static void CompleteRegistration(IWebDriver driver, Actions actions, string code, string password, string mail)
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
        RandomDelay(2000, 3000);


        IWebElement passINput = wait.Until(d => 
        {
            IWebElement element = d.FindElement(By.CssSelector("input[name='password'][type='password']"));
            return element.Displayed ? element : null;
        });

        actions.MoveToElement(passINput).Click().Perform();
        foreach (char c in password)
        {
            passINput.SendKeys(c.ToString());
            RandomDelay(10, 50);
        }
        RandomDelay(1000, 3000);

        IWebElement signup = driver.FindElement(By.XPath("//div[contains(@class, 'css-146c3p1')]//span[contains(text(), 'Sign up')]"));
        actions.MoveToElement(signup).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
        RandomDelay(2000, 3000);

        IWebElement skip = driver.FindElement(By.XPath("/html/body/div[1]/div/div/div[1]/div[2]/div/div/div/div/div/div[2]/div[2]/div/div/div[2]/div[2]/div[2]/div/div/div/button/div/span/span"));
        actions.MoveToElement(skip).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
        RandomDelay(2000, 3000);

        // IWebElement nameInput = driver.FindElement(By.CssSelector("input[name='username']"));
        // actions.MoveToElement(nameInput).Click().Perform();
        // foreach (char c in user)
        // {
        //     nameInput.SendKeys(c.ToString());
        //     RandomDelay(10, 50);
        // }
        // RandomDelay(1000, 3000);

        
        skip = driver.FindElement(By.XPath("/html/body/div[1]/div/div/div[1]/div[2]/div/div/div/div/div/div[2]/div[2]/div/div/div[2]/div[2]/div[2]/div/div/div/button/div"));
        actions.MoveToElement(skip).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
        RandomDelay(2000, 3000);

        IWebElement clicked = driver.FindElement(By.XPath("/html/body/div[1]/div/div/div[1]/div[2]/div/div/div/div/div/div[2]/div[2]/div/div/div[2]/div[2]/div[1]/div/div[2]/section/div/div/div[3]/div/div/div/li[1]/div/div/div/button/div/div/div"));
        actions.MoveToElement(clicked).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
        RandomDelay(2000, 3000);
        clicked = driver.FindElement(By.XPath("/html/body/div[1]/div/div/div[1]/div[2]/div/div/div/div/div/div[2]/div[2]/div/div/div[2]/div[2]/div[1]/div/div[2]/section/div/div/div[4]/div/div/div/li[1]/div/div/div/button/div/div/div"));
        actions.MoveToElement(clicked).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
        RandomDelay(2000, 3000);
        clicked = driver.FindElement(By.XPath("/html/body/div[1]/div/div/div[1]/div[2]/div/div/div/div/div/div[2]/div[2]/div/div/div[2]/div[2]/div[1]/div/div[2]/section/div/div/div[3]/div/div/div/li[3]/div/div/div/button/div/div/div"));
        actions.MoveToElement(clicked).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
        RandomDelay(2000, 3000);


        nextButton = driver.FindElement(By.XPath("/html/body/div[1]/div/div/div[1]/div[2]/div/div/div/div/div/div[2]/div[2]/div/div/div[2]/div[2]/div[2]/div/div/button"));
        actions.MoveToElement(nextButton).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
        RandomDelay(2000, 3000);
        Console.WriteLine("Done");
        SaveCredentialsToFile($"{mail}|{password}");
        try {
            IWebElement follow = driver.FindElement(By.XPath("/html/body/div[1]/div/div/div[1]/div[2]/div/div/div/div/div/div[2]/div[2]/div/div/div[2]/div[2]/div[1]/div/section/div/div/div[3]/div/div/button/div/div[2]/div/div[2]/button"));
            actions.MoveToElement(follow).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
            RandomDelay(50, 500);

            follow = driver.FindElement(By.XPath("/html/body/div[1]/div/div/div[1]/div[2]/div/div/div/div/div/div[2]/div[2]/div/div/div[2]/div[2]/div[1]/div/section/div/div/div[4]/div/div/button/div/div[2]/div[1]/div[2]/button"));
            actions.MoveToElement(follow).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
            RandomDelay(50, 500);

            follow = driver.FindElement(By.XPath("/html/body/div[1]/div/div/div[1]/div[2]/div/div/div/div/div/div[2]/div[2]/div/div/div[2]/div[2]/div[1]/div/section/div/div/div[5]/div/div/button/div/div[2]/div[1]/div[2]/button"));
            actions.MoveToElement(follow).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
            RandomDelay(50, 500);

            follow = driver.FindElement(By.XPath("/html/body/div[1]/div/div/div[1]/div[2]/div/div/div/div/div/div[2]/div[2]/div/div/div[2]/div[2]/div[1]/div/section/div/div/div[6]/div/div/button/div/div[2]/div/div[2]/button"));
            actions.MoveToElement(follow).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
            RandomDelay(50, 500);
        }
        catch{
            Console.WriteLine("ko follow");

        }

        nextButton = driver.FindElement(By.XPath("/html/body/div[1]/div/div/div[1]/div[2]/div/div/div/div/div/div[2]/div[2]/div/div/div[2]/div[2]/div[2]/div/div/div/button"));
        actions.MoveToElement(nextButton).Pause(TimeSpan.FromMilliseconds(random.Next(100, 300))).Click().Perform();
        RandomDelay(2000, 3000);
    }

    static void Main()
    {
        try
        {
            // Đọc các file một lần duy nhất
            string[] proxyLines = File.ReadAllLines(ProxyFilePath);
            string[] mailPassLines = File.ReadAllLines(MailPassFilePath);

            // Kiểm tra nếu không có email hoặc proxy
            if (mailPassLines.Length == 0)
            {
                Console.WriteLine("File mailPass.txt rỗng!");
                return;
            }
            if (proxyLines.Length == 0)
            {
                Console.WriteLine("File proxyfile.txt rỗng!");
                return;
            }

            // Kiểm tra accountX.txt để xác định điểm bắt đầu
            int startIndex = 0;
            try
            {
                if (File.Exists(AccountXFilePath))
                {
                    string[] accountXLines = File.ReadAllLines(AccountXFilePath);
                    if (accountXLines.Length > 0)
                    {
                        string lastAccountLine = accountXLines[accountXLines.Length - 1].Trim();
                        if (lastAccountLine.Contains("|"))
                        {
                            string lastEmail = lastAccountLine.Split("|")[0];
                            if (!string.IsNullOrEmpty(lastEmail))
                            {
                                for (int i = 0; i < mailPassLines.Length; i++)
                                {
                                    var (user, mail, password, proxyUser, proxyPass, proxyAddress) = LoadAccountInfo(i, mailPassLines, proxyLines);
                                    if (mail == lastEmail)
                                    {
                                        startIndex = i + 1;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi đọc hoặc xử lý accountX.txt: {ex.Message}. Bắt đầu từ dòng đầu tiên.");
            }

            // Số vòng lặp dựa trên số email
            int n = mailPassLines.Length;
            Console.WriteLine($"Tổng số email: {n}, Tổng số proxy: {proxyLines.Length}");
            Console.WriteLine($"Bắt đầu xử lý từ email thứ {startIndex + 1}");

            // Lặp qua từng email, bắt đầu từ startIndex
            for (int index = startIndex; index < n; index++)
            {
                // Chọn proxy theo chỉ số quay vòng
                int proxyIndex = index % proxyLines.Length;
                Console.WriteLine($"Đang xử lý email thứ {index + 1}/{n} với proxy thứ {proxyIndex + 1}/{proxyLines.Length}");
                Console.WriteLine(mailPassLines[index]);
                try
                {
                    // Tải thông tin tài khoản và proxy
                    var (user, mail, password, proxyUser, proxyPass, proxyAddress) = LoadAccountInfo(index, mailPassLines, proxyLines);
                    string userAgent = GetRandomUserAgent();

                    // Tạo ngày sinh ngẫu nhiên
                    string[] birthValues = GenerateRandomBirthDate();
                    string[] proxyParts = proxyAddress.Split(':');
                    if (proxyParts.Length < 2)
                    {
                        Console.WriteLine($"Proxy thứ {proxyIndex + 1} không đúng định dạng: {proxyAddress}");
                        continue;
                    }
                    string proxyHost = proxyParts[0];
                    string proxyPort = proxyParts[1];

                    // Thiết lập proxy
                    string extensionProxyPath = SetupProxy(proxyHost, proxyPort, proxyUser, proxyPass);

                    // Cấu hình trình duyệt
                    var (driver, actions) = ConfigureBrowser(userAgent, ExtensionFolderPath, extensionProxyPath);

                    try
                    {
                        // Xóa cookies và xác thực proxy
                        driver.Manage().Cookies.DeleteAllCookies();

                        driver.Navigate().GoToUrl("https://api.ipify.org");
                        RandomDelay(1000, 2000);

                        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                        IWebElement body = wait.Until(d => d.FindElement(By.TagName("body")));

                        if (body.Text == proxyHost)
                        {
                            Console.WriteLine("Proxy đang hoạt động.");
                        }

                        RegisterAccount(driver, actions, user, mail, birthValues, password);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Lỗi trong quá trình đăng ký với email {mail} và proxy {proxyAddress}: {ex.Message}");
                    }
                    finally
                    {
                        try
                        {
                            driver.Quit();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Lỗi khi đóng trình duyệt: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi khi xử lý email thứ {index + 1}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi đọc file proxy hoặc mail/pass: {ex.Message}");
        }
    }
}