using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateAccount
{    
    public class Import
    {
        public static List<Account> FromExcel( string filePath )
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
                                account.Credit = long.Parse((reader.GetValue(10) ?? "0").ToString());
                                vres.Add(account);
                            }
                            line += 1;
                        }
                    }
                }
                return vres;
            }
            catch {
                return new List<Account>();
            }
        }
    }
}
