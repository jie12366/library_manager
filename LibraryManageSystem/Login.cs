using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryManageSystem
{
    public partial class Login : Form
    {
        string account;
        public Login()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Register r = new Register();
            r.ShowDialog();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ResetPW rp = new ResetPW();
            rp.ShowDialog();
        }

        private void linkALogin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            ALogin al = new ALogin();
            al.ShowDialog();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            Users r = new Users();
            r.Account = txtName.Text.Trim();
            r.Password = txtPassword.Text.Trim();
            DataSet ds=DBOperate.readDB("select * from UserInfo where UserAccount='"+r.Account+"' and UserPassword='"+r.Password+"' and UserType='读者'");
            if(ds.Tables[0].Rows.Count>0)
            {
                account = (ds.Tables[0].Rows[0][0]).ToString();
                ReaderMain rm = new ReaderMain();
                rm.S = account;
                rm.Show();
                this.Hide();
            }
            else
                MessageBox.Show("账号或者密码错误！");
        }

        private void linkAbout_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AboutBox1 R = new AboutBox1();
            R.ShowDialog();
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }

        // 监听窗体的关闭按钮，点击关闭按钮，立即退出进程
        protected override void WndProc(ref Message msg)
        {
            //Windows系统消息，winuser.h文件中有WM_...的定义
            //十六进制数字，0x是前导符后面是真正的数字
            const int WM_SYSCOMMAND = 0x0112;
            //winuser.h文件中有SC_...的定义
            const int SC_CLOSE = 0xF060;

            if (msg.Msg == WM_SYSCOMMAND && ((int)msg.WParam == SC_CLOSE))
            {
                // 点击winform右上关闭按钮
                // 退出进程
                this.Close();
            }
            base.WndProc(ref msg);
        }
    }
}
