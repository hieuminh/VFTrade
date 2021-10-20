using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFTrade;

namespace CreateAccount
{    
    public class Import
    {
        public static bool TestOpen( string filePath )
        {
            try
            {
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    // Auto-detect format, supports:
                    //  - Binary Excel files (2.0-2003 format; *.xls)
                    //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        // Choose one of either 1 or 2:
                        // 1. Use the reader methods
                        int line = 0;
                        while (reader.Read())
                        {
                            return true;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static List<Account> Accounts( string filePath )
        {
            try
            {
                List<Account> vres = new List<Account>();
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    // Auto-detect format, supports:
                    //  - Binary Excel files (2.0-2003 format; *.xls)
                    //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        // Choose one of either 1 or 2:
                        // 1. Use the reader methods
                        int line = 0;
                        while (reader.Read())
                        {
                            if (line > 0)
                            {
                                try
                                {
                                    var account = new Account();
                                    account.SoTK = reader.GetValue(1).ToString();
                                    account.HoTen = (reader.GetValue(2) ?? "").ToString();
                                    account.GhiChu = (reader.GetValue(3) ?? "").ToString();
                                    account.LoaiGiayTo = (reader.GetValue(4) ?? "").ToString();
                                    account.SoID = (reader.GetValue(5) ?? "").ToString();
                                    account.SanPham = (reader.GetValue(6) ?? "").ToString();
                                    account.GoiPhi = (reader.GetValue(7) ?? "").ToString();
                                    account.Phone = (reader.GetValue(8) ?? "").ToString();
                                    account.Email = (reader.GetValue(9) ?? "").ToString();
                                    //account.Credit = long.Parse((reader.GetValue(10) ?? "0").ToString());
                                    vres.Add(account);
                                }
                                catch { }
                            }
                            line += 1;
                        }
                    }
                }
                return vres;
            }
            catch (Exception ex) {
                System.IO.File.WriteAllText("Log.txt", ex.Message + Environment.NewLine + ex.StackTrace);
                return new List<Account>();
            }
        }

        public static List<DepositInfo> Deposit(string filePath)
        {
            List<DepositInfo> vres = new List<DepositInfo>();
            try
            {
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    // Auto-detect format, supports:
                    //  - Binary Excel files (2.0-2003 format; *.xls)
                    //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        while ( reader.Name.ToLower() != "nộp tiền")
                        {
                            if ( !reader.NextResult() )
                            {
                                return new List<DepositInfo>();
                            }
                        }                        
                        // Choose one of either 1 or 2:
                        // 1. Use the reader methods
                        int line = 0;
                        while (reader.Read())
                        {
                            if (line > 0)
                            {
                                try
                                {
                                    var info = new DepositInfo();
                                    info.SoTK = reader.GetValue(1).ToString();
                                    info.Money = decimal.Parse((reader.GetValue(2) ?? "0").ToString());
                                    info.DepositDate = (reader.GetValue(3) ?? "").ToString();
                                    info.RecordedDate = (reader.GetValue(4) ?? "").ToString();
                                    info.TKTong = (reader.GetValue(5) ?? "").ToString();
                                    vres.Add(info);
                                }
                                catch { }
                            }
                            line += 1;
                        }
                    }
                }
                return vres;
            }
            catch
            {
                return new List<DepositInfo>();
            }
        }

        public static List<Order> Orders(string filePath)
        {
            List<Order> vres = new List<Order>();
            try
            {
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    // Auto-detect format, supports:
                    //  - Binary Excel files (2.0-2003 format; *.xls)
                    //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        while (reader.Name.ToLower() != "đặt lệnh")
                        {
                            if (!reader.NextResult())
                            {
                                return vres;
                            }
                        }
                        // Choose one of either 1 or 2:
                        // 1. Use the reader methods                        
                        int line = 0;
                        while (reader.Read())
                        {
                            if (line > 0)
                            {
                                try
                                {
                                    var order = new Order();
                                    if (reader.GetValue(1) is DateTime)
                                    {
                                        order.Date = ((DateTime)reader.GetValue(1)).ToString("dd/MM/yyyy");
                                    }
                                    else
                                    {
                                        order.Date = reader.GetValue(1).ToString();
                                    }
                                    order.SoTK = reader.GetValue(2).ToString();
                                    order.OrderType = reader.GetValue(3).ToString();
                                    order.MaCK = reader.GetValue(4).ToString();
                                    order.Price = reader.GetValue(5).ToString();
                                    order.Volume = decimal.Parse((reader.GetValue(6) ?? "0").ToString());
                                    order.MatchedPrice = reader.GetValue(7).ToString();
                                    order.CtyCK = reader.GetValue(8).ToString();
                                    order.TKTong = reader.GetValue(9).ToString();
                                    order.SHL = reader.GetValue(10).ToString();
                                    vres.Add(order);
                                }
                                catch { }
                            }
                            line += 1;
                        }
                    }
                }
                return vres;
            }
            catch
            {
                return new List<Order>();
            }
        }

        private static decimal roundPrice( decimal price )
        {
            long llprice = (long)(price * 1000);
            if ( llprice % 50 == 0 )
            {
                //18.800
                //18.250
                return price;
            }
            else
            {
                llprice -= llprice % 50;
                return llprice / 1000M;
            }
        }

        public static List<ClosePriceInfo> ClosePrices( string filePath )
        {
            List<ClosePriceInfo> vres = new List<ClosePriceInfo>();
            try
            {
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    // Auto-detect format, supports:
                    //  - Binary Excel files (2.0-2003 format; *.xls)
                    //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        while (reader.Name.ToLower() != "giá đóng cửa")
                        {
                            if (!reader.NextResult())
                            {
                                return vres;
                            }
                        }
                        // Choose one of either 1 or 2:
                        // 1. Use the reader methods                        
                        int line = 0;
                        while (reader.Read())
                        {
                            if (line > 0)
                            {
                                var info = new ClosePriceInfo();
                                try
                                {
                                    if (reader.GetValue(1) is DateTime)
                                    {
                                        var dateTime = (DateTime)reader.GetValue(1);
                                        info.Date = dateTime.ToString("dd/MM/yyyy");
                                    }
                                    else
                                    {
                                        info.Date = reader.GetValue(1).ToString();
                                    }
                                    info.MaCK = reader.GetValue(2).ToString();
                                    info.GiaDongCua = decimal.Parse(reader.GetValue(3).ToString());
                                    try { info.GiaSan = decimal.Parse( reader.GetValue(4).ToString()); } catch { info.GiaSan = roundPrice(info.GiaDongCua * 0.93M); }
                                    try { info.GiaTran = decimal.Parse( reader.GetValue(5).ToString()); } catch { info.GiaTran = roundPrice(info.GiaDongCua * 1.067M); }
                                    try { info.GiaTC = decimal.Parse( reader.GetValue(6).ToString()); } catch { info.GiaTC = info.GiaDongCua; }
                                    vres.Add(info);
                                }
                                catch { }                                
                            }
                            line += 1;
                        }
                    }
                }
                return vres;
            }
            catch
            {
                return new List<ClosePriceInfo>();
            }
        }
    }
}
