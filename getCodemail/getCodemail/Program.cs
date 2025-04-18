using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.Threading;

class Program
{
    static void RandomDelay(int minMs, int maxMs)
    {
        Random random = new Random();
        Thread.Sleep(random.Next(minMs, maxMs));
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
            RandomDelay(2000, 5000);

            // Click nút mở menu
            By openButtonLocator = By.XPath("/html/body/div[1]/div/div[2]/div/div/div[2]/button[3]");
            if (!WaitForElement(driver, openButtonLocator, 10))
                throw new Exception("Không tìm thấy nút mở menu");
            IWebElement openButton = driver.FindElement(openButtonLocator);
            new Actions(driver).MoveToElement(openButton).Pause(TimeSpan.FromMilliseconds(new Random().Next(100, 300))).Click().Perform();
            RandomDelay(1000, 3000);

            // Click nút đăng nhập
            By loginButtonLocator = By.XPath("/html/body/div[4]/div/div[2]/button[2]");
            if (!WaitForElement(driver, loginButtonLocator, 10))
                throw new Exception("Không tìm thấy nút đăng nhập");
            IWebElement loginButton = driver.FindElement(loginButtonLocator);
            new Actions(driver).MoveToElement(loginButton).Pause(TimeSpan.FromMilliseconds(new Random().Next(100, 300))).Click().Perform();
            RandomDelay(1000, 3000);

            // Nhập email
            By emailInputLocator = By.Id("v-1-15");
            if (!WaitForElement(driver, emailInputLocator, 10))
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
            if (!WaitForElement(driver, passwordInputLocator, 10))
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
            if (!WaitForElement(driver, logButtonLocator, 10))
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
                RandomDelay(1000, 2000);

                // Lấy mã xác minh
                By codeLocator = By.CssSelector("span[style*='font-weight:bold'][style*='font-size:14px']");
                if (!WaitForElement(driver, codeLocator, 10))
                    throw new Exception("Không tìm thấy mã xác minh");
                IWebElement code = driver.FindElement(codeLocator);
                Console.WriteLine($"Mã xác minh: {code.Text}");
                codeText = code.Text;
            }
            catch
            {
                // Thử lại nếu không lấy được mã
                driver.Navigate().Refresh();
                RandomDelay(2000, 5000);

                By nextButtonLocator = By.XPath("/html/body/div[1]/div/div[2]/main/div[2]/div[2]/ul/li[1]/a/div/div/div[2]/div[1]/div[2]");
                if (!WaitForElement(driver, nextButtonLocator, 10))
                    throw new Exception("Không tìm thấy email mới nhất");
                IWebElement nextButton = driver.FindElement(nextButtonLocator);
                new Actions(driver).MoveToElement(nextButton).Pause(TimeSpan.FromMilliseconds(new Random().Next(100, 300))).Click().Perform();
                RandomDelay(1000, 3000);

                By iframeLocator = By.XPath("/html/body/div[1]/div/div[2]/main/div[2]/div[2]/div[2]/div/iframe");
                if (!WaitForElement(driver, iframeLocator, 10))
                    throw new Exception("Không tìm thấy iframe");
                IWebElement iframeElement = driver.FindElement(iframeLocator);
                driver.SwitchTo().Frame(iframeElement);
                RandomDelay(1000, 2000);

                By codeLocator = By.XPath("/html/body/table/tbody/tr[1]/td/table/tbody/tr[1]/td/table[1]/tbody/tr/td[2]/table/tbody/tr[10]/td");
                if (!WaitForElement(driver, codeLocator, 10))
                    throw new Exception("Không tìm thấy mã xác minh");
                IWebElement code = driver.FindElement(codeLocator);
                Console.WriteLine($"Mã xác minh: {code.Text}");
                codeText = code.Text;
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

    static void Main()
    {
        ChromeOptions options = new ChromeOptions();
        IWebDriver driver = new ChromeDriver(options);
        // 05cpltw7oi6vau7@ptct.net|Fk4upWsBL5h
        string code = GetVerificationCode(driver, "05cpltw7oi6vau7@ptct.net", "Fk4upWsBL5h");
        Console.WriteLine(code);
    }
}
