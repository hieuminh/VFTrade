using CreateAccount;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VFTrade.HttpRequest
{
    public class Manager
    {
        public string Username;
        public string Password;
        public int RECV_BUFF_SIZE = 1024 * 8;

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
            return "";
        }        

        public string CreateAccount( Account account )
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

                //Nộp tiền/ rút tiền
                IWebElement btnNapRutTien = _driver.FindElement(By.LinkText("Nộp tiền/ rút tiền"));
                btnNapRutTien.Click();
                Thread.Sleep(500);

                IWebElement btnNapTien = _driver.FindElement(By.LinkText("Nộp tiền"));
                btnNapTien.Click();
                Thread.Sleep(500);

                //formBankCode
                SelectElement formBankCode = new SelectElement(_driver.FindElement(By.Name("formBankCode")));
                formBankCode.SelectByIndex(1);
                Thread.Sleep(250);

                IWebElement formAccountCode = _driver.FindElement(By.Name("formAccountCode"));
                formAccountCode.SendKeys(account.SoTK);
                Thread.Sleep(250);

                //formAmount
                IWebElement formAmount = _driver.FindElement(By.Name("formAmount"));
                formAmount.SendKeys(account.Credit.ToString());
                Thread.Sleep(250);

                //formContent
                IWebElement formContent = _driver.FindElement(By.Name("formContent"));
                formContent.SendKeys("Deposited by tool");
                Thread.Sleep(200);

                //class="mx-2 mr-auto btn-search fz-14 btn btn-primary"
                IWebElement submitTien = _driver.FindElement(By.CssSelector("button.mx-2.mr-auto.btn-search.fz-14.btn.btn-primary"));
                submitTien.Click();
                Thread.Sleep(1000);                
                return "";
            }
            catch {
                return "Error";
            }
        }

        public string ButToan(Account account)
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
                            Thread.Sleep(2000);
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

        public void Close()
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}
