using ZhenziSms;
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
    public partial class ResetPW : Form
    {
        public ResetPW()
        {
            InitializeComponent();
        }
        string newpassword;//新密码
        private void btnSmsReset_Click(object sender, EventArgs e)
        {
            Random rd = new Random();
            newpassword = (rd.Next(10000000, 100000000)).ToString();//随机生成一个8位数字密码
            DataSet ds = DBOperate.readDB("select * from UserInfo where UserMobile='" + txtSmsReset.Text.Trim() + "'");
            if (ds.Tables[0].Rows.Count > 0)
            {
                if (DBOperate.writeDB("update UserInfo set UserPassword='" + newpassword + "' where UserMobile='" + txtSmsReset.Text.Trim() + "'") > 0)
                {
                    var client = new ZhenziSmsClient("https://sms_developer.zhenzikj.com", "101348", "ZGZmNjM3MWYtZDVjMS00YWUyLWE4NmUtZDI5NjNmOGRjNTA1");

                    var parameters = new Dictionary<string, string>();
                    parameters.Add("message", "您的新密码为: " + newpassword);
                    parameters.Add("number", txtSmsReset.Text.Trim());
                    //parameters.Add("clientIp", "792.168.2.222");
                    //parameters.Add("messageId", "");
                    var result = client.Send(parameters);
                    Console.WriteLine("返回:" + result);
                    MessageBox.Show("重置密码成功、已发送到您的手机！");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("重置失败，请联系系统管理员!");
                }
            }
            else
            {

                MessageBox.Show("请输入注册过该平台的手机号");
            }
        }
    }
}
