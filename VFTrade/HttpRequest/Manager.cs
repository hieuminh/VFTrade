using CreateAccount;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VFTrade.HttpRequest
{
    public static class WebElementCustom
    {
        public static IWebElement SetAttribute(this IWebElement element, string name, string value)
        {
            var driver = ((IWrapsDriver)element).WrappedDriver;
            var jsExecutor = (IJavaScriptExecutor)driver;
            jsExecutor.ExecuteScript("arguments[0].setAttribute(arguments[1], arguments[2]);", element, name, value);

            return element;
        }
    }

    public class Manager
    {
        public string Username;
        public string Password;
        public int RECV_BUFF_SIZE = 1024 * 8;
        public bool IsOnline = false;

        private readonly object selfLock = new object();

        public void CheckIn()
        {
            if (Monitor.TryEnter(selfLock))
            {
                try
                {
                    _driver.Navigate().Refresh();
                }
                catch { }
                finally
                {
                    Monitor.Exit(selfLock);
                }
            }
        }

        protected readonly object _lockCookie = new object();
        protected CookieCollection _cookieCollection;
        protected CookieContainer _cookiesContainer;

        protected HttpWebRequest CreateHttpWebRequest(string url)
        {
            HttpWebRequest request;
            request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.71 Safari/537.36";
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.AllowAutoRedirect = true;
            return request;
        }

        public string FetchGet( string url, string referer )
        {
            try
            {
                HttpWebRequest request = CreateHttpWebRequest(url);
                request.Method = "GET";
                request.Accept = "*/*";
                if ( !string.IsNullOrEmpty(referer))
                    request.Referer = referer;
                request.KeepAlive = true;
                request.Timeout = 15000;
                if (referer.Length > 0)
                    request.Referer = referer;
                request.CookieContainer = _cookiesContainer;

                // Get the response
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    response.Cookies = request.CookieContainer.GetCookies(request.RequestUri);

                    // Add cookie
                    lock (_lockCookie)
                    {
                        _cookieCollection.Add(response.Cookies);
                        _cookiesContainer.Add(_cookieCollection);
                    }

                    #region Read response from the stream
                    char[] chars = new char[RECV_BUFF_SIZE];
                    using (BufferedStream buffer = new BufferedStream(response.GetResponseStream(), RECV_BUFF_SIZE))
                    {
                        // Read the response from the stream
                        using (StreamReader responseStream = new StreamReader(buffer, Encoding.UTF8, true, RECV_BUFF_SIZE))
                        {
                            StringBuilder result = new StringBuilder();
                            int charRead = responseStream.Read(chars, 0, RECV_BUFF_SIZE);
                            while (charRead > 0)
                            {
                                result.Append(chars, 0, charRead);
                                charRead = responseStream.Read(chars, 0, RECV_BUFF_SIZE);
                            }                            
                            return result.ToString();
                        }
                    }
                    #endregion
                }
            }
            catch (Exception e)
            {
                return "Error";
            }            
        }

        private Encoding _encoding = Encoding.GetEncoding(1252);
        public string FetchPost( string url, string referer, string postData)
        {
            try
            {
                #region Request Initialization
                HttpWebRequest request = CreateHttpWebRequest(url);
                request.Method = "POST";
                request.Accept = "*/*";
                request.Headers["Accept-Language"] = "en-US,en;q=0.9";
                request.ContentType = @"application/x-www-form-urlencoded";
                request.Headers["Origin"] = "http://vftrade.vn:8888";
                if (string.IsNullOrEmpty(url))
                    request.Referer = referer;
                request.Timeout = 15000;
                request.CookieContainer = _cookiesContainer;
                #endregion

                #region Include post data
                byte[] postBuffer = _encoding.GetBytes(postData);
                request.ContentLength = postBuffer.Length;
                Stream postDataStream = request.GetRequestStream();
                postDataStream.Write(postBuffer, 0, postBuffer.Length);
                postDataStream.Close();
                #endregion

                #region Get Response
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    response.Cookies = request.CookieContainer.GetCookies(request.RequestUri);
                    lock (_lockCookie)
                    {
                        _cookieCollection.Add(response.Cookies);
                        _cookiesContainer.Add(_cookieCollection);
                    }

                    char[] chars = new char[RECV_BUFF_SIZE];
                    using (BufferedStream buffer = new BufferedStream(response.GetResponseStream(), RECV_BUFF_SIZE))
                    {
                        // Read the response from the stream
                        using (StreamReader responseStream = new StreamReader(buffer, Encoding.UTF8, true, RECV_BUFF_SIZE))
                        {
                            StringBuilder result = new StringBuilder();
                            int charRead = responseStream.Read(chars, 0, RECV_BUFF_SIZE);
                            while (charRead > 0)
                            {
                                result.Append(chars, 0, charRead);
                                charRead = responseStream.Read(chars, 0, RECV_BUFF_SIZE);
                            }
                            return result.ToString();
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                return "Error";
            }
        }

        public string LogIn()
        {
            lock (_lockCookie)
            {
                _cookieCollection = new CookieCollection();
                _cookiesContainer = new CookieContainer();
            }

            string postData = $"user={Username}&pass={Password}&channel=D";
            string response = FetchPost("http://vftrade.vn:8888/BDlogin", "http://vftrade.vn:8888/", postData);

            var json = JObject.Parse(response);
            if ( json["rs"].ToString() == "{}" )
            {
                return "";
            }
            else
            {
                return json["rs"].ToString();
            }
        }

        private IWebDriver _driver = null;

        public void StartSelenium()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddExcludedArgument("enable-automation");
            chromeOptions.AddArgument("ignore-certificate-errors");
            chromeOptions.AddArgument("allow-insecure-localhost");
            chromeOptions.AcceptInsecureCertificates = true;
            _driver = new ChromeDriver(Directory.GetCurrentDirectory(), chromeOptions);
            _driver.Manage().Window.Maximize();
        }

        public string LogInSelenium()
        {
            StartSelenium();
            _driver.Navigate().GoToUrl("http://vftrade.vn:8888");
            IWebElement btnLogin = _driver.FindElement(By.ClassName("btn-login"));
            btnLogin.Click();

            IWebElement usernameElement = _driver.FindElement(By.Name("username"));
            usernameElement.SendKeys(Username);
            Thread.Sleep(1000);
            IWebElement passwordElement = _driver.FindElement(By.Name("password"));
            passwordElement.SendKeys(Password);
            Thread.Sleep(1000);

            IWebElement btnDangNhap = _driver.FindElement(By.CssSelector("button.mt-2.btn.btn-primary"));
            btnDangNhap.Click();
            IsOnline = true;
            return "";
        }        

        public string CreateAccount( Account account )
        {
            if (!IsOnline)
                return "Error";
            lock (selfLock)
            {
                try 
                {
                    _driver.Navigate().Refresh();
                    IWebElement btnBroker = _driver.FindElement(By.LinkText("Broker"));
                    btnBroker.Click();
                    Thread.Sleep(500);
                    //.btn-action-add
                    IWebElement btnAdd = _driver.FindElement(By.CssSelector("button.btn-action-add.fz-14.btn.btn-primary"));
                    btnAdd.Click();

                    IWebElement stk = _driver.FindElement(By.Name("accountCode"));
                    stk.SendKeys(account.SoTK);
                    Thread.Sleep(300);

                    SelectElement productCode = new SelectElement(_driver.FindElement(By.Name("productCode")));
                    productCode.SelectByText(account.SanPham);
                    Thread.Sleep(300);

                    SelectElement cardIdType = new SelectElement(_driver.FindElement(By.Name("cardIdType")));
                    cardIdType.SelectByText(account.LoaiGiayTo);
                    Thread.Sleep(300);

                    IWebElement cardId = _driver.FindElement(By.Name("cardId"));
                    cardId.SendKeys(account.SoID);
                    Thread.Sleep(300);

                    //issuePlace
                    IWebElement issuePlace = _driver.FindElement(By.Name("issuePlace"));
                    issuePlace.SendKeys("Hanoi");
                    Thread.Sleep(300);

                    //custMobile
                    IWebElement custMobile = _driver.FindElement(By.Name("custMobile"));
                    custMobile.SendKeys(account.Phone);
                    Thread.Sleep(300);

                    IWebElement custEmail = _driver.FindElement(By.Name("custEmail"));
                    custEmail.SendKeys(account.Email);
                    Thread.Sleep(300);

                    //custAddress
                    IWebElement custAddress = _driver.FindElement(By.Name("custAddress"));
                    custAddress.SendKeys("Hanoi");
                    Thread.Sleep(300);

                    //content
                    IWebElement content = _driver.FindElement(By.Name("content"));
                    content.SendKeys(account.GhiChu);
                    Thread.Sleep(300);

                    //class="fz-14 btn btn-success"
                    //accountName
                    IWebElement accountName = _driver.FindElement(By.Name("accountName"));
                    accountName.SendKeys(account.HoTen);
                    Thread.Sleep(300);

                    SelectElement commCode = new SelectElement(_driver.FindElement(By.Name("commCode")));
                    commCode.SelectByText(account.GoiPhi);
                    Thread.Sleep(300);

                    IWebElement submit = _driver.FindElement(By.CssSelector("button.fz-14.btn.btn-success"));
                    submit.Click();
                    Thread.Sleep(2000);

                    try
                    {
                        submit = _driver.FindElement(By.CssSelector("button.fz-14.btn.btn-success"));
                        return "Error";
                    }
                    catch { }

                    /* Navigate to Nap tien **/
                    //item-head nav-link px-3 text-white fz-13 active // MENU
                    try
                    {
                        IWebElement navNghiepVuTien = _driver.FindElement(By.CssSelector("img.img-collapse"));
                        navNghiepVuTien.Click();
                        Thread.Sleep(500);
                    }
                    catch (NoSuchElementException) { }

                    //class="item-head nav-link px-3 text-white fz-13 "
                    var navNghiepVuTien1s = _driver.FindElements(By.CssSelector("div.item-head.nav-link.px-3.text-white.fz-13"));
                    navNghiepVuTien1s[2].Click();
                    Thread.Sleep(500);

                    return "";
                }
                catch
                {
                    return "Error";
                }
            }
        }

        public string NopTien( DepositInfo info )
        {
            if (!IsOnline)
                return "Error";
            lock (selfLock)
            {
                try
                {
                    _driver.Navigate().Refresh();
                    IWebElement btnBroker = _driver.FindElement(By.LinkText("Broker"));
                    btnBroker.Click();
                    Thread.Sleep(500);
                    /* Navigate to Nap tien **/
                    //item-head nav-link px-3 text-white fz-13 active // MENU
                    try
                    {
                        IWebElement navNghiepVuTien = _driver.FindElement(By.CssSelector("img.img-collapse"));
                        navNghiepVuTien.Click();
                        Thread.Sleep(500);
                    }
                    catch (NoSuchElementException) { }

                    //class="item-head nav-link px-3 text-white fz-13 "
                    var navNghiepVuTien1s = _driver.FindElements(By.CssSelector("div.item-head.nav-link.px-3.text-white.fz-13"));
                    navNghiepVuTien1s[2].Click();
                    Thread.Sleep(500);

                    //Nộp tiền/ rút tiền
                    IWebElement btnNapRutTien = _driver.FindElement(By.LinkText("Nộp tiền/ rút tiền"));
                    btnNapRutTien.Click();
                    Thread.Sleep(500);
                    
                    IWebElement btnNapTien = _driver.FindElement(By.LinkText("Nộp tiền"));
                    btnNapTien.Click();
                    Thread.Sleep(500);

                    //formBankCode
                    SelectElement formBankCode = new SelectElement(_driver.FindElement(By.Name("formBankCode")));
                    formBankCode.SelectByText(info.TKTong);
                    Thread.Sleep(150);

                    IWebElement formAccountCode = _driver.FindElement(By.Name("formAccountCode"));
                    formAccountCode.SendKeys(info.SoTK);
                    Thread.Sleep(150);

                    //formAmount
                    IWebElement formAmount = _driver.FindElement(By.Name("formAmount"));
                    formAmount.SendKeys(info.Money.ToString());
                    Thread.Sleep(100);

                    //formFileDate
                    IWebElement formFileDate = _driver.FindElement(By.Name("formFileDate"));
                    formFileDate.SetAttribute("value", info.DepositDate);
                    Thread.Sleep(100);

                    //formTransactionDate
                    IWebElement formTransactionDate = _driver.FindElement(By.Name("formTransactionDate"));
                    formTransactionDate.SetAttribute("value", info.RecordedDate);
                    Thread.Sleep(100);

                    //formContent
                    IWebElement formContent = _driver.FindElement(By.Name("formContent"));
                    formContent.SendKeys("Deposited by tool");
                    Thread.Sleep(100);

                    //class="mx-2 mr-auto btn-search fz-14 btn btn-primary"
                    IWebElement submitTien = _driver.FindElement(By.CssSelector("button.mx-2.mr-auto.btn-search.fz-14.btn.btn-primary"));
                    submitTien.Click();
                    Thread.Sleep(500);

                    /*** But toan ***/
                    IWebElement buttoan = _driver.FindElement(By.LinkText("Bút toán tiền"));
                    buttoan.Click();
                    Thread.Sleep(500);
                    var task = Task.Factory.StartNew(() =>
                    {
                        bool staleElement = true;
                        while (staleElement)
                        {
                            try
                            {
                                IWebElement table = _driver.FindElement(By.CssSelector("table.table.table-bordered"));
                                var allrows = table.FindElements(By.TagName("tr"));
                                var row1 = allrows[1];
                                var cell1s = row1.FindElements(By.TagName("td"));
                                var cell1 = cell1s[0];
                                var acc2 = new Actions(_driver);
                                var acts = acc2.DoubleClick(cell1);
                                acts.Perform();
                                staleElement = false;
                                Thread.Sleep(1000);
                            }
                            catch (StaleElementReferenceException e)
                            {
                                staleElement = true;
                            }
                        }
                    });
                    task.Wait();
                    //class="mx-2 mr-auto btn-search fz-14 btn btn-success"
                    IWebElement duyet = _driver.FindElement(By.CssSelector("button.mx-2.mr-auto.btn-search.fz-14.btn.btn-success"));
                    duyet.Click();
                    return "";
                }
                catch
                {
                    return "Error";
                }
            }
        }        

        public string PlaceOrder( Order order )
        {
            if (!IsOnline)
                return "Error";
            lock (selfLock)
            {
                try
                {
                    _driver.Navigate().Refresh();

                    //class="btn-trade btn btn-primary"
                    IWebElement btnDatLenh = _driver.FindElement(By.LinkText("Đặt lệnh"));
                    btnDatLenh.Click();
                    Thread.Sleep(500);

                    //orderAccount
                    IWebElement orderAccount = _driver.FindElement(By.Name("orderAccount"));
                    orderAccount.SendKeys(order.SoTK);
                    Thread.Sleep(150);

                    if ( order.OrderType == "MUA" )
                    {
                        //btn btn-type-trade text-uppercase buy active
                        IWebElement buyType = _driver.FindElement(By.CssSelector("button.btn.btn-type-trade.text-uppercase.buy"));
                        buyType.Click();
                        Thread.Sleep(150);
                    }
                    else
                    {
                        //class="btn btn-type-trade text-uppercase sell "
                        IWebElement sellType = _driver.FindElement(By.CssSelector("button.btn.btn-type-trade.text-uppercase.sell"));
                        sellType.Click();
                        Thread.Sleep(150);
                    }

                    //orderSymbol
                    IWebElement orderSymbol = _driver.FindElement(By.Name("orderSymbol"));
                    orderSymbol.Clear();
                    orderSymbol.SendKeys(order.MaCK.ToUpper());
                    Thread.Sleep(100);

                    //orderVolume
                    IWebElement orderVolume = _driver.FindElement(By.Name("orderVolume"));
                    orderVolume.Clear();
                    orderVolume.SendKeys(order.Volume.ToString());
                    Thread.Sleep(100);

                    //orderPrice
                    IWebElement orderPrice = _driver.FindElement(By.Name("orderPrice"));
                    orderPrice.SendKeys(order.Price);
                    Thread.Sleep(100);

                    //Place order
                    //class="btn font-weight-bold fz-14 btn-order order-sell"
                    IWebElement submit = _driver.FindElement(By.CssSelector("button.btn.font-weight-bold.fz-14.btn-order"));
                    submit.Click();
                    Thread.Sleep(350);

                    //class="font-weight-bold fz-14 mx-2 btn btn-primary"
                    IWebElement confirm = _driver.FindElement(By.CssSelector("button.font-weight-bold.fz-14.mx-2.btn.btn-primary"));
                    confirm.Click();
                    Thread.Sleep(250);

                    for (int i = 0; i < 5; ++i)
                    {
                        try
                        {
                            //Toastify__toast-body 
                            //Đặt lệnh thành công!
                            IWebElement msgElement = _driver.FindElement(By.ClassName("Toastify__toast-body"));
                            string msg = msgElement.Text.Trim();

                            if (msg != "Đặt lệnh thành công!" && msg != "" )
                            {
                                return msg;
                            }
                            else if (msg != "")
                            {
                                return "";
                            }
                        }
                        catch { }
                        Thread.Sleep(100);
                    }
                    return "";
                }
                catch
                {
                    return "Error";
                }
            }
        }

        public string ConfirmOrder( Order order)
        {
            if (!IsOnline)
                return "Error";
            lock (selfLock)
            {
                try
                {
                    _driver.Navigate().Refresh();

                    IWebElement btnBroker = _driver.FindElement(By.LinkText("Admin"));
                    btnBroker.Click();
                    Thread.Sleep(500);

                    /* Navigate to So Lenh **/
                    //item-head nav-link px-3 text-white fz-13 active // MENU
                    try
                    {
                        IWebElement navMenu = _driver.FindElement(By.CssSelector("img.img-collapse"));
                        navMenu.Click();
                        Thread.Sleep(300);
                    }
                    catch (NoSuchElementException) { }
                    
                    //Trạng thái lệnh
                    IWebElement btnTrangThaiLenh = _driver.FindElement(By.LinkText("Sổ lệnh Manual"));
                    btnTrangThaiLenh.Click();
                    Thread.Sleep(1000);


                    // tim Lenh vua dat trong So Lenh
                    IWebElement table = _driver.FindElement(By.CssSelector("table.table.table-bordered"));
                    var allrows = table.FindElements(By.TagName("tr"));
                    IWebElement row;
                    IWebElement cell9 = null;
                    for( int i=1;i<allrows.Count;++i)
                    {
                        row = allrows[i];
                        var cells = row.FindElements(By.TagName("td"));
                        if (cells[1].Text.Trim() != order.SoTK)
                            continue;

                        if (order.OrderType == "MUA" && cells[2].Text.Trim() != "B")
                            continue;
                        else if (order.OrderType != "MUA" && cells[2].Text.Trim() == "B")
                            continue;

                        if (cells[3].Text.Trim() != order.MaCK)
                            continue;

                        decimal vol = decimal.Parse(cells[4].Text.Trim().Replace(",","") );
                        if (vol != order.Volume)
                            continue;

                        decimal price;
                        if (decimal.TryParse(order.Price, out price))
                        {
                            decimal price1;
                            if (decimal.TryParse(cells[5].Text.Trim(), out price1))
                            {
                                decimal diff = Math.Abs(price - price1);
                                if (diff > 0.0001M)
                                {
                                    continue;
                                }
                            }
                            else
                                continue;
                        }
                        else
                        {
                            if (cells[5].Text.Trim() != order.Price)
                                continue;
                        }

                        cell9 = cells[9];
                        break;
                    }
                    
                    IWebElement btnConfirm = cell9.FindElement(By.CssSelector("button.btn.btn-primary"));
                    btnConfirm.Click();
                    Thread.Sleep(350);

                    //formCTCK
                    WebDriverWait wait = new WebDriverWait(_driver, new TimeSpan(0, 0, 3));
                    wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Name("formCTCK")));
                    //_driver.Manage.wa
                    SelectElement formCTCK = new SelectElement(_driver.FindElement(By.Name("formCTCK")));
                    try
                    {
                        formCTCK.SelectByText(order.CtyCK.Trim());
                    }
                    catch (NoSuchElementException ex)
                    {
                        return $"Khớp lệnh lỗi, không tồn tại cty CK '{order.CtyCK.Trim()}', kiểm tra lại dữ liệu";
                    }
                    Thread.Sleep(100);

                    //formTKT
                    wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Name("formTKT")));
                    SelectElement formTKT = new SelectElement(_driver.FindElement(By.Name("formTKT")));
                    try
                    {
                        formTKT.SelectByText(order.TKTong);
                    }
                    catch (NoSuchElementException ex)
                    {
                        return $"Khớp lệnh lỗi, không tồn tại tk tổng {order.TKTong.Trim()} tại {order.CtyCK}, kiểm tra lại dữ liệu";
                    }
                    Thread.Sleep(100);

                    //formOrderNo
                    IWebElement formOrderNo = _driver.FindElement(By.Name("formOrderNo"));
                    formOrderNo.SendKeys(order.SHL);
                    Thread.Sleep(100);

                    //font-weight-bold fz-14 mx-2 btn btn-primary
                    IWebElement btnXacNhan = _driver.FindElement(By.CssSelector("button.font-weight-bold.fz-14.mx-2.btn.btn-primary"));
                    btnXacNhan.Click();
                    Thread.Sleep(500);

                    //class="mx-1 btn btn-success"
                    IWebElement btnKhop = cell9.FindElement(By.CssSelector("button.mx-1.btn.btn-success"));
                    btnKhop.Click();
                    Thread.Sleep(400);

                    //formMatchPrice
                    IWebElement formMatchPrice = _driver.FindElement(By.Name("formMatchPrice"));
                    formMatchPrice.SendKeys(order.MatchedPrice);
                    Thread.Sleep(100);

                    //formMatchPrice
                    IWebElement formMatchVol = _driver.FindElement(By.Name("formMatchVol"));
                    formMatchVol.SendKeys(order.Volume.ToString());
                    Thread.Sleep(100);

                    //font-weight-bold fz-14 mx-2 btn btn-primary
                    IWebElement btnXacNhanKhop = _driver.FindElement(By.CssSelector("button.font-weight-bold.fz-14.mx-2.btn.btn-primary"));
                    btnXacNhanKhop.Click();
                    Thread.Sleep(300);

                    for (int i = 0; i < 5; ++i)
                    {
                        try
                        {
                            //Toastify__toast-body 
                            //Đặt lệnh thành công!
                            IWebElement msgElement = _driver.FindElement(By.ClassName("Toastify__toast-body"));
                            string msg = msgElement.Text.Trim();

                            if ( !msg.Contains("thành công") && msg != "")
                            {
                                return msg;
                            }
                            else if (msg != "")
                            {
                                return "";
                            }
                        }
                        catch { }
                        Thread.Sleep(100);
                    }
                    return "";

                    /*
                    IWebElement msgElement = _driver.FindElement(By.ClassName("Toastify__toast-body"));
                    string msg = msgElement.Text.Trim();

                    if (!msg.Contains("thành công") )
                    {
                        return "Error: " + msg;
                    }*/

                    return "";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
                    return "Error";
                }
            }
        }

        private void PickReactDateTime(string time)
        {
            // dd/MM/yyyy
            DateTime date = DateTime.ParseExact(time, "dd/MM/yyyy", CultureInfo.InvariantCulture );
            //react-datepicker__current-month
            while (true)
            {
                IWebElement currentMonth = _driver.FindElement(By.ClassName("react-datepicker__current-month"));
                var currentTime = DateTime.Parse(currentMonth.Text);
                if (currentTime.Year == date.Year && currentTime.Month == date.Month)
                    break;
                else if ( currentTime.Year > date.Year || currentTime.Month > date.Month )
                {
                    //react-datepicker__navigation react-datepicker__navigation--previous
                    IWebElement previous = _driver.FindElement(By.CssSelector("button.react-datepicker__navigation.react-datepicker__navigation--previous"));
                    previous.Click();
                    Thread.Sleep(300);
                }
                else if (currentTime.Year < date.Year || currentTime.Month < date.Month)
                {
                    //react-datepicker__navigation react-datepicker__navigation--next
                    IWebElement next = _driver.FindElement(By.CssSelector("button.react-datepicker__navigation.react-datepicker__navigation--next"));
                    next.Click();
                    Thread.Sleep(300);
                }
            }

            //class="react-datepicker__day react-datepicker__day--012"
            IWebElement dayPick = _driver.FindElement(By.CssSelector($"div.react-datepicker__day.react-datepicker__day--{date.Day.ToString("D3")}"));
            dayPick.Click();
        }

        public string UpdateClosePrice( ClosePriceInfo closePrice )
        {
            if (!IsOnline)
                return "Error";
            lock (selfLock)
            {
                try
                {
                    _driver.Navigate().Refresh();

                    IWebElement btnBroker = _driver.FindElement(By.LinkText("Broker"));
                    btnBroker.Click();
                    Thread.Sleep(500);

                    /* Navigate to Navigation Menu **/
                    //item-head nav-link px-3 text-white fz-13 active // MENU
                    try
                    {
                        IWebElement navMenuIcon = _driver.FindElement(By.CssSelector("img.img-collapse"));
                        navMenuIcon.Click();
                        Thread.Sleep(400);
                    }
                    catch (NoSuchElementException) { }

                    // Bao cao tai san
                    //class="item-head nav-link px-3 text-white fz-13 "
                    var navMenu = _driver.FindElements(By.CssSelector("div.item-head.nav-link.px-3.text-white.fz-13"));
                    navMenu[4].Click();
                    Thread.Sleep(350);

                    //Tra cứu tài sản
                    IWebElement traCuuTaiSan = _driver.FindElement(By.LinkText("Tra cứu tài sản"));
                    traCuuTaiSan.Click();
                    Thread.Sleep(500);

                    //Tra cứu giá đóng cửa CK cuối ngày
                    IWebElement btnTraCuuGiaDongCua = _driver.FindElement(By.LinkText("Tra cứu giá đóng cửa CK cuối ngày"));
                    btnTraCuuGiaDongCua.Click();
                    Thread.Sleep(500);

                    //formAccountCode
                    IWebElement formAccountCode = _driver.FindElement(By.Name("formAccountCode"));
                    formAccountCode.SendKeys(closePrice.MaCK);
                    Thread.Sleep(100);

                    //formFromDate
                    IWebElement formFromDate = _driver.FindElement(By.Name("formFromDate"));
                    formFromDate.Click();
                    PickReactDateTime(closePrice.Date);
                    Thread.Sleep(100);

                    //formToDate
                    IWebElement formToDate = _driver.FindElement(By.Name("formToDate"));
                    formToDate.Click();
                    PickReactDateTime(closePrice.Date);
                    Thread.Sleep(100);

                    //class="mx-2 mr-auto btn-search fz-14 align-self-end btn btn-primary"
                    IWebElement searchSubmit = _driver.FindElement(By.CssSelector("button.mx-2.mr-auto.btn-search.fz-14.align-self-end.btn.btn-primary"));
                    searchSubmit.Click();
                    Thread.Sleep(1000);

                    IWebElement table = _driver.FindElement(By.CssSelector("table.table.table-bordered"));
                    var allrows = table.FindElements(By.TagName("tr"));

                    if ( allrows.Count >= 2 )
                    {
                        // update lai gia (neu truoc do da co)
                        var cells = allrows[1].FindElements(By.TagName("td"));
                        var cell1 = cells[3];
                        var acc2 = new Actions(_driver);
                        var acts = acc2.DoubleClick(cell1);
                        acts.Perform();
                        Thread.Sleep(500);

                        //formClosePrice
                        IWebElement formClosePrice = _driver.FindElement(By.Name("formClosePrice"));
                        formClosePrice.Clear();
                        formClosePrice.SendKeys( (closePrice.GiaDongCua * 1000).ToString("N0") );
                        Thread.Sleep(100);

                        //fz-14 btn btn-success
                        IWebElement btnSuccess = _driver.FindElement(By.CssSelector("button.fz-14.btn.btn-success"));
                        btnSuccess.Click();
                        Thread.Sleep(300);
                    }
                    else
                    {
                        // new gia moi
                        //class="btn-action-add fz-14 align-self-end btn btn-primary"
                        IWebElement btnAddNew = _driver.FindElement(By.CssSelector("button.btn-action-add.fz-14.align-self-end.btn.btn-primary"));
                        btnAddNew.Click();
                        Thread.Sleep(500);

                        //formSymbol
                        IWebElement formSymbol = _driver.FindElement(By.Name("formSymbol"));
                        formSymbol.Clear();
                        formSymbol.SendKeys(closePrice.MaCK);
                        Thread.Sleep(100);

                        //formCeilPrice
                        IWebElement formCeilPrice = _driver.FindElement(By.Name("formCeilPrice"));
                        formCeilPrice.Clear();
                        formCeilPrice.SendKeys((closePrice.GiaTran * 1000).ToString("N0"));
                        Thread.Sleep(100);

                        //formFloorPrice
                        IWebElement formFloorPrice = _driver.FindElement(By.Name("formFloorPrice"));
                        formFloorPrice.Clear();
                        formFloorPrice.SendKeys((closePrice.GiaSan * 1000).ToString("N0"));
                        Thread.Sleep(100);

                        //formRefPrice
                        IWebElement formRefPrice = _driver.FindElement(By.Name("formRefPrice"));
                        formRefPrice.Clear();
                        formRefPrice.SendKeys((closePrice.GiaTC * 1000).ToString("N0"));
                        Thread.Sleep(100);

                        //formClosePrice
                        IWebElement formClosePrice = _driver.FindElement(By.Name("formClosePrice"));
                        formClosePrice.Clear();
                        formClosePrice.SendKeys((closePrice.GiaDongCua * 1000).ToString("N0"));
                        Thread.Sleep(100);

                        //fz-14 btn btn-success
                        IWebElement btnSuccess = _driver.FindElement(By.CssSelector("button.fz-14.btn.btn-success"));
                        btnSuccess.Click();
                        Thread.Sleep(300);
                    }
                    return "";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
                    return "Error";
                }
            }
        }

        public void Close()
        {
            IsOnline = false;
            _driver.Quit();
            _driver.Dispose();
        }
    }
}
