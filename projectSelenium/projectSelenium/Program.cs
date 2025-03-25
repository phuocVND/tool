using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

class Program
{
    static void Main()
    {
        string user = "qSSHZrVDSVsX@rustyload.com";
        string password = "Q0NbhAxl4vDy";
        // Khởi tạo trình duyệt Chrome
        IWebDriver driver = new ChromeDriver();

        // Mở trang web
        driver.Navigate().GoToUrl("https://mail.tm");

        // Sử dụng WebDriverWait để chờ button xuất hiện
        //Thread.Sleep(3000);
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        IWebElement button = driver.FindElement(By.Id("reka-dropdown-menu-trigger-v-1-4"));
        button.Click();
        button = driver.FindElement(By.XPath("//*[@id='reka-dropdown-menu-content-v-1-9']/div[2]/button[2]"));
        button.Click();

        IWebElement inputElement = driver.FindElement(By.XPath("//*[@id='v-1-15']"));
        inputElement.SendKeys(user);

        inputElement = driver.FindElement(By.XPath("//*[@id='v-1-16']"));
        inputElement.SendKeys(password);

        button = driver.FindElement(By.XPath("/html/body/div[8]/div/div/div[2]/span[1]/button"));
        button.Click();

        button = driver.FindElement(By.XPath("//*[@id=\"__nuxt\"]/div/div[1]/div/div[2]/nav/a[2]"));
        button.Click();
       
    }
}
