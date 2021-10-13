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

            ListAccount = Import.FromExcel(txtFilePath.Text.Trim());
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
        }

        private void btnTao_Click(object sender, EventArgs e)
        {
            btnTao.Enabled = false;
            Task.Factory.StartNew(() => 
            {
                for( int i=0;i<ListAccount.Count;++i)
                {
                    var res = myManager.CreateAccount(ListAccount[i]);
                    this.Invoke(new ThreadStart(() => 
                    {
                        dgvAccounts[12, i].Value = res == "" ? "OK" : res;
                    }));

                    if (res == "")
                    {
                        myManager.ButToan(ListAccount[i]);
                    }
                    Thread.Sleep(500);
                }

                this.Invoke(new ThreadStart(() =>
                {
                    btnTao.Enabled = true;
                }));
            });
        }
    }
}
