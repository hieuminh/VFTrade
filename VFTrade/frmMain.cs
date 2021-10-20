using CreateAccount;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VFTrade.HttpRequest;

namespace VFTrade
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Maximized;
        }

        private void txtFilePath_Clicked(object sender, EventArgs e)
        {
            OpenFileDialog openFileDlg = new OpenFileDialog();
            if ( openFileDlg.ShowDialog() == DialogResult.OK )
            {
                txtFilePath.Text = openFileDlg.FileName;                
            }
        }

        private List<Account> ListAccount;
        private List<DepositInfo> ListDeposits;
        private List<Order> ListOrders;
        private List<ClosePriceInfo> ListClosePrices;

        private void btnImport_Click(object sender, EventArgs e)
        {
            string filePath = "";
            OpenFileDialog openFileDlg = new OpenFileDialog();
            if (openFileDlg.ShowDialog() == DialogResult.OK)
            {
                txtFilePath.Text = openFileDlg.FileName;
                filePath = openFileDlg.FileName;
            }
            else
                return;

            if ( !Import.TestOpen(filePath) )
            {
                MessageBox.Show("Lỗi mở file excel, vui lòng save file excel và đóng chương trình Microsoft Excel");
                return;
            }

            try
            {
                ListAccount = Import.Accounts(txtFilePath.Text.Trim());
                dgvAccounts.Rows.Clear();
                for (int i = 0; i < ListAccount.Count; ++i)
                {
                    var account = ListAccount[i];
                    dgvAccounts.Rows.Add(new object[]
                    {
                        (i+1),
                        account.SoTK,
                        account.HoTen,
                        account.GhiChu,
                        account.LoaiGiayTo,
                        account.SoID,
                        account.SanPham,
                        account.GoiPhi,
                        account.Phone,
                        account.Email,
                        account.Credit.ToString("N0"),
                        "Tạo",
                        ""
                    });
                }
            }
            catch {
            }

            try
            {
                ListDeposits = Import.Deposit(txtFilePath.Text.Trim());
                dgvDeposit.Rows.Clear();
                for (int i = 0; i < ListDeposits.Count; ++i)
                {
                    var info = ListDeposits[i];
                    dgvDeposit.Rows.Add(new object[]
                    {
                        (i+1),
                        info.SoTK,
                        info.Money.ToString("N0"),
                        info.TKTong,
                        "Nộp",
                        ""
                    });
                }
            }
            catch
            {
            }

            try
            {
                ListOrders = Import.Orders(txtFilePath.Text.Trim());
                dgvOrders.Rows.Clear();
                List<string> ListDate = new List<string>();
                for (int i = 0; i < ListOrders.Count; ++i)
                {
                    var info = ListOrders[i];
                    ListDate.Add(info.Date);
                    dgvOrders.Rows.Add(new object[]
                    {
                        (i+1),
                        info.Date,
                        info.SoTK,
                        info.OrderType,
                        info.MaCK,
                        info.Price,
                        info.Volume.ToString("N0"),
                        info.MatchedPrice,
                        "Thực hiện",
                        ""
                    });
                }

                ListDate.Sort(delegate (string x, string y) {
                    var xx = x.Split('/');
                    var yy = y.Split('/');
                    for (int i = xx.Length - 1; i >= 0; --i)
                    {
                        if (xx[i] != yy[i]) return xx[i].CompareTo(yy[i]);
                    }
                    return 0;
                });
                cbxOrderDate.Items.Clear();
                cbxOrderDate.Items.Add("Tất cả các ngày");
                for( int i=0;i<ListDate.Count;++i)
                {
                    if ( i == 0 || ListDate[i] != ListDate[i-1] )
                    {
                        cbxOrderDate.Items.Add(ListDate[i]);
                    }
                }
            }
            catch
            {
            }

            try
            {
                ListClosePrices = Import.ClosePrices(txtFilePath.Text.Trim());
                dgvClosePrice.Rows.Clear();
                List<string> ListDate = new List<string>();
                for (int i = 0; i < ListClosePrices.Count; ++i)
                {
                    var info = ListClosePrices[i];
                    ListDate.Add(info.Date);
                    dgvClosePrice.Rows.Add(new object[]
                    {
                        (i+1),
                        info.Date,
                        info.MaCK,
                        (info.GiaDongCua * 1000).ToString("N0"),
                        (info.GiaSan * 1000).ToString("N0"),
                        (info.GiaTran * 1000).ToString("N0"),
                        (info.GiaTC * 1000).ToString("N0"),
                        "Thực hiện",
                        ""
                    });
                }

                ListDate.Sort(delegate (string x, string y) {
                    var xx = x.Split('/');
                    var yy = y.Split('/');
                    for( int i=xx.Length-1;i>=0;--i)
                    {
                        if (xx[i] != yy[i]) return xx[i].CompareTo(yy[i]);
                    }
                    return 0;
                });
                cbxCloseDate.Items.Clear();
                cbxCloseDate.Items.Add("Tất cả các ngày");
                for (int i = 0; i < ListDate.Count; ++i)
                {
                    if (i == 0 || ListDate[i] != ListDate[i - 1])
                    {
                        cbxCloseDate.Items.Add(ListDate[i]);
                    }
                }
            }
            catch
            {
            }
        }

        private System.Threading.Timer checkInTimer;

        private void CheckIn(object obj)
        {
            if ( myManager.IsOnline )
                myManager.CheckIn();
        }

        private Manager myManager;

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if ( btnLogin.Text != "Đăng nhập")
            {
                btnLogin.Text = "Đăng nhập";
                btnLogin.ForeColor = Color.Black;

                if ( myManager != null )
                {
                    myManager.Close();
                }
                return;
            }
            else
            {

            }

            if ( string.IsNullOrEmpty( txtUsername.Text.Trim() )  )
            {

                MessageBox.Show("Username rỗng");
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txtPassword.Text.Trim()))
            {

                MessageBox.Show("Password rỗng");
                txtPassword.Focus();
                return;
            }

            if (IsRemember)
            {
                Properties.Settings.Default.Username = txtUsername.Text.Trim();
                Properties.Settings.Default.Password = txtPassword.Text.Trim();
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.Username = "";
                Properties.Settings.Default.Password = "";
                Properties.Settings.Default.Save();
            }

            myManager = new Manager();
            myManager.Username = txtUsername.Text.Trim().ToUpper();
            myManager.Password = txtPassword.Text.Trim();

            WriteLog($"User {myManager.Username} bắt đầu đăng nhập ...");

            var res = myManager.LogIn();

            if ( string.IsNullOrEmpty(res) )
            {
                WriteLog($"{myManager.Username} đăng nhập OK ...");

                myManager.LogInSelenium();
                if (checkInTimer == null)
                {
                    checkInTimer = new System.Threading.Timer(CheckIn, null, 30000, 60000);
                }

                btnLogin.Text = "Đăng xuất";
                btnLogin.ForeColor = Color.Red;
            }
            else
            {
                WriteLog($"{myManager.Username} đăng nhập lỗi: {res}");                
                MessageBox.Show($"Đăng nhập {myManager.Username} lỗi: {res}");
            }
        }

        private void WriteLog( string info )
        {
            this.Invoke(new ThreadStart(() => 
            {
                txtLog.AppendText($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} - {info}{Environment.NewLine}");
            }));
        }

        private bool IsRemember = true;

        private void chkRemember_CheckedChanged(object sender, EventArgs e)
        {
            IsRemember = chkRemember.Checked;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            if ( Properties.Settings.Default.Username != "" )
            {
                txtUsername.Text = Properties.Settings.Default.Username;
            }

            if ( Properties.Settings.Default.Password != "" )
            {
                txtPassword.Text = Properties.Settings.Default.Password;
            }

            dgvDeposit.Left = (this.ClientSize.Width - dgvDeposit.Width) / 2;
            dgvOrders.Left = (this.ClientSize.Width - dgvOrders.Width) / 2;
            grbDataSource.Left = (this.ClientSize.Width - grbDataSource.Width) / 2;
            grbThaoTac.Left = (this.ClientSize.Width - grbThaoTac.Width) / 2;
            btnTao.Left = (this.ClientSize.Width - btnTao.Width) / 2;
            btnNopTienAll.Left = (this.ClientSize.Width - btnNopTienAll.Width) / 2;
            grbClosePrice.Left = (this.ClientSize.Width - grbClosePrice.Width) / 2;
        }



        private void btnTao_Click(object sender, EventArgs e)
        {
            btnTao.Enabled = false;
            Task.Factory.StartNew(() => 
            {
                int success = 0;
                for( int i=0;i<ListAccount.Count;++i)
                {
                    this.Invoke(new ThreadStart(() =>
                    {
                        dgvAccounts[12, i].Value = "Đang tạo tài khoản ...";
                    }));
                    var res = myManager.CreateAccount(ListAccount[i]);
                    this.Invoke(new ThreadStart(() => 
                    {
                        dgvAccounts[12, i].Value = res == "" ? "OK" : res;
                    }));

                    if (res == "")
                    {
                        success += 1;
                        ListAccount[i].IsCreated = true;
                    }
                    Thread.Sleep(500);
                }

                this.Invoke(new ThreadStart(() =>
                {
                    btnTao.Enabled = true;
                    MessageBox.Show($"Đã tạo xong {success} / {ListAccount.Count} tài khoản");
                }));
            });
        }

        private void frmMain_SizeChanged(object sender, EventArgs e)
        {
            dgvDeposit.Left = (this.ClientSize.Width - dgvDeposit.Width) / 2;
            dgvOrders.Left = (this.ClientSize.Width - dgvOrders.Width) / 2;
            grbDataSource.Left = (this.ClientSize.Width - grbDataSource.Width) / 2;
            grbThaoTac.Left = (this.ClientSize.Width - grbThaoTac.Width) / 2;
            btnTao.Left = (this.ClientSize.Width - btnTao.Width) / 2;
            btnNopTienAll.Left = (this.ClientSize.Width - btnNopTienAll.Width) / 2;
            grbClosePrice.Left = (this.ClientSize.Width - grbClosePrice.Width) / 2;
        }

        private void btnNopTienAll_Click(object sender, EventArgs e)
        {
            btnNopTienAll.Enabled = false;
            Task.Factory.StartNew(() =>
            {
                int success = 0;
                for (int i = 0; i < ListDeposits.Count; ++i)
                {
                    this.Invoke(new ThreadStart(() =>
                    {
                        dgvDeposit[5, i].Value = "Đang nộp tiền ...";
                    }));
                    var res = myManager.NopTien(ListDeposits[i]);
                    this.Invoke(new ThreadStart(() =>
                    {
                        dgvDeposit[5, i].Value = res == "" ? "OK" : res;
                    }));

                    if (res == "")
                        success += 1;
                    Thread.Sleep(500);
                }

                this.Invoke(new ThreadStart(() =>
                {
                    btnNopTienAll.Enabled = true;
                    MessageBox.Show($"Đã nộp tiền xong {success} / {ListAccount.Count} tài khoản");
                }));
            });
        }

        private bool IsIndividualOrder = true;
        private Dictionary<string, string> PlacedOrders = new Dictionary<string, string>();

        private void btnPlaceOrderAllDay_Click(object sender, EventArgs e)
        {
            btnPlaceOrderAllDay.Enabled = false;

            string day = "";
            try
            {
                day = cbxOrderDate.SelectedItem.ToString();
            }
            catch
            {
                MessageBox.Show("Phải chọn ngày");
                btnPlaceOrderAllDay.Enabled = true;                
                return;
            }

            IsIndividualOrder = false;

            Task.Factory.StartNew(() =>
            {
                int success = 0;
                int totalLenh = 0;
                int successKhop = 0;
                for (int i = 0; i < ListOrders.Count; ++i)
                {
                    if ( ListOrders[i].Date != day )
                    {
                        continue;
                    }

                    if ( PlacedOrders.ContainsKey($"{ListOrders[i].SoTK}-{ListOrders[i].CtyCK}-{ListOrders[i].SHL}") )
                    {
                        string val = PlacedOrders[$"{ListOrders[i].SoTK}-{ListOrders[i].CtyCK}-{ListOrders[i].SHL}"];
                        if (val == "")
                            continue;
                    }

                    totalLenh += 1;
                    this.Invoke(new ThreadStart(() =>
                    {
                        dgvOrders[9, i].Value = "Đang đặt lệnh...";
                    }));
                    var res = myManager.PlaceOrder(ListOrders[i]);
                    this.Invoke(new ThreadStart(() =>
                    {
                        dgvOrders[9, i].Value = res == "" ? "Đặt lệnh thành công" : "Đặt lệnh lỗi: " + res;
                    }));

                    PlacedOrders[$"{ListOrders[i].SoTK}-{ListOrders[i].CtyCK}-{ListOrders[i].SHL}"] = res;
                    if (res == "")
                    {
                        success += 1;
                        res = myManager.ConfirmOrder(ListOrders[i]);
                        if ( res == "" )
                        {
                            successKhop += 1;
                        }
                        this.Invoke(new ThreadStart(() =>
                        {
                            dgvOrders[9, i].Value = res == "" ? "Khớp lệnh thành công" : res;
                        }));
                    }
                    Thread.Sleep(500);
                }
                IsIndividualOrder = true;
                this.Invoke(new ThreadStart(() =>
                {
                    btnPlaceOrderAllDay.Enabled = true;
                    MessageBox.Show($"Đã đặt lệnh xong {success} / {totalLenh} lệnh {Environment.NewLine}Đã khớp thành công {successKhop} / {totalLenh} lệnh");
                }));
            });
        }

        private void dgvAccounts_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 11 && e.RowIndex >= 0 && e.RowIndex < dgvAccounts.RowCount)
            {
                var account = ListAccount[e.RowIndex];
                Task.Factory.StartNew(() =>
                {
                    this.Invoke(new ThreadStart(() =>
                    {
                        dgvAccounts[12, e.RowIndex].Value = " Đang tạo ...";
                    }));
                    var res = myManager.CreateAccount(account);
                    this.Invoke(new ThreadStart(() =>
                    {
                        dgvAccounts[12, e.RowIndex].Value = res == "" ? "OK" : res;
                    }));
                });
            }
        }

        private void dgvDeposit_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4 && e.RowIndex >= 0 && e.RowIndex < dgvDeposit.RowCount)
            {
                var info = ListDeposits[e.RowIndex];
                Task.Factory.StartNew(() =>
                {
                    this.Invoke(new ThreadStart(() =>
                    {
                        dgvDeposit[5, e.RowIndex].Value = "Đang nộp tiền ...";
                    }));
                    var res = myManager.NopTien(info);
                    this.Invoke(new ThreadStart(() =>
                    {
                        dgvDeposit[5, e.RowIndex].Value = res == "" ? "OK" : res;
                    }));
                });
            }
        }

        private void dgvOrders_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!IsIndividualOrder)
            {
                MessageBox.Show("Phần mềm đang bận đặt lệnh");
                return;
            }

            if (e.ColumnIndex == 8 && e.RowIndex >= 0 && e.RowIndex < dgvOrders.RowCount)
            {
                IsIndividualOrder = false;
                int id = int.Parse(dgvOrders[0, e.RowIndex].Value.ToString() ) ;
                var order = ListOrders[id-1];
                Task.Factory.StartNew(() =>
                {
                    this.Invoke(new ThreadStart(() =>
                    {
                        dgvOrders[9, e.RowIndex].Value = "Đang đặt lệnh ...";
                    }));
                    var res = myManager.PlaceOrder(order);
                    this.Invoke(new ThreadStart(() =>
                    {
                        dgvOrders[9, e.RowIndex].Value = res == "" ? "Đặt lệnh thành công" : res;
                    }));

                    if (res == "")
                    {
                        res = myManager.ConfirmOrder(order);
                        this.Invoke(new ThreadStart(() =>
                        {
                            dgvOrders[9, e.RowIndex].Value = res == "" ? "Khớp lệnh thành công" : res;
                        }));
                    }
                    IsIndividualOrder = true;
                });
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            PlacedOrders.Clear();
            MessageBox.Show("Đã clear hết dữ liệu lệnh");
        }

        private void btnUpdatePrice_Click(object sender, EventArgs e)
        {
            btnUpdatePrice.Enabled = false;
            string day = "";
            try
            {
                day = cbxCloseDate.SelectedItem.ToString();
            }
            catch
            {
                MessageBox.Show("Phải chọn ngày");
                btnUpdatePrice.Enabled = true;
                return;
            }

            Task.Factory.StartNew(() =>
            {
                int success = 0;
                int totalLenh = 0;
                for (int i = 0; i < ListClosePrices.Count; ++i)
                {
                    if (ListClosePrices[i].Date != day)
                    {
                        continue;
                    }
                    
                    totalLenh += 1;
                    this.Invoke(new ThreadStart(() =>
                    {
                        dgvClosePrice[8, i].Value = "Đang cập nhật ...";
                    }));

                    var res = myManager.UpdateClosePrice(ListClosePrices[i]);
                    this.Invoke(new ThreadStart(() =>
                    {
                        dgvClosePrice[8, i].Value = res == "" ? "Cập nhật giá đóng cửa thành công" : "Cập nhật giá lỗi: " + res;
                    }));

                    if (res == "")
                        success += 1;                        
                    Thread.Sleep(500);
                }

                this.Invoke(new ThreadStart(() =>
                {
                    btnUpdatePrice.Enabled = true;
                    MessageBox.Show($"Đã cập nhật giá đóng cửa ngày {day} của {success} / {totalLenh} mã");
                }));
            });
        }

        private void dgvClosePrice_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 7 && e.RowIndex >= 0 && e.RowIndex < dgvClosePrice.RowCount)
            {
                int id = int.Parse(dgvClosePrice[0, e.RowIndex].Value.ToString());
                var closePriceInfo = ListClosePrices[id-1];
                Task.Factory.StartNew(() =>
                {
                    this.Invoke(new ThreadStart(() =>
                    {
                        dgvClosePrice[8, e.RowIndex].Value = "Đang cập nhật ...";
                    }));
                    var res = myManager.UpdateClosePrice(closePriceInfo);
                    this.Invoke(new ThreadStart(() =>
                    {
                        dgvClosePrice[8, e.RowIndex].Value = res == "" ? "Cập nhật giá đóng cửa thành công" : res;
                    }));                    
                });
            }
        }

        private void cbxCloseDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string day = cbxCloseDate.SelectedItem.ToString();
                if ( cbxCloseDate.SelectedIndex == 0 )
                {
                    for (int i = 0; i < dgvClosePrice.Rows.Count; ++i) dgvClosePrice.Rows[i].Visible = true;
                }
                else
                {
                    for (int i = 0; i < dgvClosePrice.Rows.Count; ++i)
                    {
                        string dayVal = dgvClosePrice[1, i].Value.ToString();
                        if (dayVal == day) dgvClosePrice.Rows[i].Visible = true;
                        else dgvClosePrice.Rows[i].Visible = false;
                    }
                }
            }
            catch { }
        }

        private void cbxOrderDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string day = cbxOrderDate.SelectedItem.ToString();
                if (cbxOrderDate.SelectedIndex == 0)
                {
                    for (int i = 0; i < dgvOrders.Rows.Count; ++i) dgvOrders.Rows[i].Visible = true;
                }
                else
                {
                    for (int i = 0; i < dgvOrders.Rows.Count; ++i)
                    {
                        string dayVal = dgvOrders[1, i].Value.ToString();
                        if (dayVal == day) dgvOrders.Rows[i].Visible = true;
                        else dgvOrders.Rows[i].Visible = false;
                    }
                }
            }
            catch { }
        }
    }
}
