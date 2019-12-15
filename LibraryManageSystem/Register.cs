using System;
using Qiniu.Util;
using Qiniu.Storage;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZhenziSms;
using Qiniu.Http;

namespace LibraryManageSystem
{
    public partial class Register : Form
    {
        public Register()
        {
            InitializeComponent();
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtNum.Text = "";
            txtPass.Text = "";
            textPass2.Text = "";
            txtName.Text = "";
            textPhone.Text = "";
            textVerificationCode.Text = "";
        }
        string num;//验证码随机数
  
        private void btnGetVerificationCode_Click(object sender, EventArgs e)//获取短信验证码
        {
            Random rd = new Random();
             num = (rd.Next(100000, 1000000)).ToString();

            var client = new ZhenziSmsClient("https://sms_developer.zhenzikj.com", "101348", "ZGZmNjM3MWYtZDVjMS00YWUyLWE4NmUtZDI5NjNmOGRjNTA1");

            var parameters = new Dictionary<string, string>();
            parameters.Add("message", "您的验证码为: " + num);
            parameters.Add("number", textPhone.Text.Trim());
            var result = client.Send(parameters);
            Console.WriteLine("返回:" + result);
        }
        string upUserStr;//设置一个用于接收上传图片路径的字符串
        private void btnUp_Click(object sender, EventArgs e)
        {
            Regex account = new Regex(@"^[a-zA-Z][a-zA-Z0-9]{6,14}$");
            Regex passWord = new Regex(@"^[a-zA-Z0-9]{4,10}$");
            Regex name = new Regex(@"^[\u4e00-\u9fa5]{2,}$");
            Regex tel = new Regex(@"^0?(13|14|15|17|18|19)[0-9]{9}$");
            Users u = new Users();
            u.Account = txtNum.Text.Trim();
            u.Password = txtPass.Text.Trim();
            u.Name = txtName.Text.Trim();
            u.Mobile = textPhone.Text.Trim();
            if (!account.IsMatch(txtNum.Text.Trim()))
            {
                pictureBox2.Visible = true;
            }
            else
            {
                pictureBox2.Visible = false;
            }
            if (!passWord.IsMatch(txtPass.Text.Trim()))
            {
                pictureBox3.Visible = true;
            }
            else
            {
                pictureBox3.Visible = false;
            }
            if (!name.IsMatch(txtName.Text.Trim()))
            {
                pictureBox4.Visible = true;
            }
            else
            {
                pictureBox4.Visible = false;
            }
            if (!tel.IsMatch(textPhone.Text.Trim()))
            {
                pictureBox5.Visible = true;
            }
            else
            {
                pictureBox5.Visible = false;
            }
            DataSet ds = DBOperate.readDB("select * from UserInfo where UserAccount='" + u.Account + "'");
            DataSet ds1 = DBOperate.readDB("select * from UserInfo where UserMobile='" + u.Mobile + "'");                     
            if (txtPass.Text.Trim() !=textPass2.Text.Trim())
            {
                MessageBox.Show("两次密码输入不一致！");
                return;
            }
            if (ds.Tables[0].Rows.Count > 0)
            {
                MessageBox.Show("当前用户已被注册！");
            }
            else
            {
                if (pictureBox2.Visible || pictureBox3.Visible || pictureBox4.Visible || pictureBox5.Visible)
                {
                    MessageBox.Show("信息填写错误！");
                    return;
                }
                else
                {

                    if (ds1.Tables[0].Rows.Count > 0)
                    {
                        MessageBox.Show("当前用户的手机号已被注册！");
                    }
                    else
                    {
                            if (num == textVerificationCode.Text.Trim())
                            {
                            if (DBOperate.writeDB("insert into UserInfo values('" + u.Account + "','" + u.Password + "','" + u.Name + "','" + u.Mobile + "','读者')") > 0)
                            {
                                ///把选择的图片上传给阿里云oss
                                if (txtName.Text.Trim() != null)
                                {
                                    Mac mac = new Mac("1VC_A1ZMeAe3PYJoEXPPVWlHbHLdJ9gTH8hZY0WC", "Eg0e8u3qlHWAH_GdASR1xXdkzYbEb-v85PyNWKhX");
                                    // 上传文件名 
                                    string key = "users/" + txtNum.Text.Trim() + ".jpg";
                                    // 本地文件路径
                                    string filePath = upUserStr;
                                    // 存储空间名
                                    string Bucket = "windsearcher";

                                    PutPolicy putPolicy = new PutPolicy();
                                    // 设置要上传的目标空间
                                    putPolicy.Scope = Bucket;
                                    // 上传策略的过期时间(单位:秒)
                                    putPolicy.SetExpires(3600);
                                    // 生成上传token
                                    string token = Auth.CreateUploadToken(mac, putPolicy.ToJsonString());

                                    Config config = new Config();
                                    // 设置上传区域
                                    config.Zone = Zone.ZONE_CN_South;
                                    // 设置 http 或者 https 上传
                                    config.UseHttps = true;
                                    config.UseCdnDomains = true;
                                    config.ChunkSize = ChunkUnit.U512K;
                                    // 表单上传
                                    FormUploader target = new FormUploader(config);
                                    HttpResult result = target.UploadFile(filePath, key, token, null);
                                    Console.WriteLine("form upload result: " + result.ToString());
                                }
                                else
                                {
                                    MessageBox.Show("图片上传失败，请联系管理员");
                                }
                                MessageBox.Show("注册成功!");
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("注册失败!");
                            }
                            }
                            else
                            {
                            MessageBox.Show("请输入验证码，或验证码不正确！");
                            }
                    }
                }
            }
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd1 = new OpenFileDialog();
            ofd1.Filter = "图片文件（*.jpg）|*.jpg";
            if (ofd1.ShowDialog() == DialogResult.OK)
            {
                upUserStr = ofd1.FileName;
                // 获取图片文件
                Image imgPhoto = Image.FromFile(ofd1.FileName);
                // 克隆这个图片文件
                Image imgClone = new Bitmap(imgPhoto);
                // 将原始的图片文件销毁
                imgPhoto.Dispose();
                // 将克隆的图片文件赋值
                pictureUpRegin.Image = imgClone;
            }
        }
    }
}
