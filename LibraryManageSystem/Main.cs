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
using Qiniu.Http;

namespace LibraryManageSystem
{
    public partial class Main : Form
    {
        Regex account = new Regex(@"^[a-zA-Z][a-zA-Z0-9]{6,14}$");
        Regex passWord = new Regex(@"^[a-zA-Z0-9]{4,10}$");
        Regex name = new Regex(@"^[\u4e00-\u9fa5]{2,}$");
        Regex tel = new Regex(@"^0?(13|14|15|17|18|19)[0-9]{9}$");
        Regex mail = new Regex(@"^[\w!#$%&'*+/=?^_`{|}~-]+(?:\.[\w!#$%&'*+/=?^_`{|}~-]+)*@(?:[\w](?:[\w-]*[\w])?\.)+[\w](?:[\w-]*[\w])?$");
        public Main()
        {
            InitializeComponent();
        }

        private void tabControl2_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabWenZi(sender, e);
        }
        //TabControl横向显示---方法
        private static void TabWenZi(object sender, DrawItemEventArgs e)
        {
            string text = ((TabControl)sender).TabPages[e.Index].Text;
            SolidBrush brush = new SolidBrush(Color.Black);
            StringFormat sf = new StringFormat(StringFormatFlags.DirectionRightToLeft);
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString(text, SystemInformation.MenuFont, brush, e.Bounds, sf);
        }

        private void tabControl3_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabWenZi(sender, e);
        }

        private void tabControl4_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabWenZi(sender, e);
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Application.Exit();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            Users u=new Users();
            DataSet ds=null;
            u.Name= "%" + txtSearch.Text.Trim() + "%";
            if (txtSearch.Text == "")
            {
                ds = DBOperate.readDB("select UserAccount 账号,UserName 姓名,UserMobile 手机号 from UserInfo where UserType='读者'");
                dataGridView1.DataSource = ds.Tables[0];
            }
            else
            {
                ds = DBOperate.readDB("select UserAccount 账号,UserName 姓名,UserMobile 手机号 from UserInfo where UserName like '" + u.Name + "' and UserType='读者'");
                dataGridView1.DataSource = ds.Tables[0];
                if (ds.Tables[0].Rows.Count > 0)
                {
                    txtNumInfo.Text = (ds.Tables[0].Rows[0][0]).ToString();
                    txtMobileInfo.Text = (ds.Tables[0].Rows[0][2]).ToString();
                }
            }
           
        }

        private void btnWhole_Click(object sender, EventArgs e)
        {
            DataSet ds = DBOperate.readDB("select Books.ISBN,bookname 书名,bookstyle 类型,price 价格,press 出版社,author 作者,entertime 购入时间,isborrow 是否借出,UserAccount 账号,borrowtime 借出时间,returntime 归还时间 from Books left outer join BRBooks on Books.ISBN=BRBooks.ISBN");
            dataGridView2.DataSource = ds.Tables[0];
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtNum.Text = "";
            txtPass.Text = "";
            textPass2.Text = "";
            txtName.Text = "";
            textPhone.Text = "";
        }
        string upUserStr;//设置一个用于接收上传图片路径的字符串
        private void btnUp_Click(object sender, EventArgs e)
        {
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
            Users u = new Users();
            u.Account = txtNum.Text.Trim();
            u.Password = txtPass.Text.Trim();
            u.Name = txtName.Text.Trim();
            u.Mobile = textPhone.Text.Trim();
            DataSet ds = DBOperate.readDB("select * from UserInfo where UserAccount='" + u.Account + "'");
            DataSet ds1 = DBOperate.readDB("select * from UserInfo where UserMobile='" + u.Mobile +  "'");
            if (txtPass.Text.Trim() != textPass2.Text.Trim())
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
                        if (DBOperate.writeDB("insert into UserInfo values('" + u.Account + "','" + u.Password + "','" + u.Name + "','" + u.Mobile + "','读者')") > 0)
                        {
                            ///传图
                            if (txtName.Text.Trim() != null)
                            {
                                Mac mac = new Mac("1VC_A1ZMeAe3PYJoEXPPVWlHbHLdJ9gTH8hZY0WC", "Eg0e8u3qlHWAH_GdASR1xXdkzYbEb-v85PyNWKhX");
                                // 上传文件名 
                                string key = @"users/" + u.Account + ".jpg";
                                // 本地文件路径
                                string filePath = @"" + upUserStr + "";
                                // 存储空间名
                                string Bucket = "windsearcher";

                                PutPolicy putPolicy = new PutPolicy();
                                // 设置要上传的目标空间
                                putPolicy.Scope = Bucket;
                                // 上传策略的过期时间(单位:秒)
                                putPolicy.SetExpires(3600);
                                // 文件上传完毕后，在多少天后自动被删除
                                putPolicy.DeleteAfterDays = 1;
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
                        }
                        else
                        {
                            MessageBox.Show("注册失败!");
                        }
                    }
                }
            }
        }
        string upUserStrBook;
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtISBN.Text == "" || txtBookName.Text == "" || comboBox1.Text == "" || txtPrice.Text == "" || txtPress.Text == "" || txtAuthor.Text == "")
            {
                MessageBox.Show("信息不能为空!");
            }
            else
            {
                Book b = new Book();
                b.ISBN = txtISBN.Text.Trim();
                b.Name = txtBookName.Text.Trim();
                b.Style = comboBox1.Text.Trim();
                b.Price = Convert.ToSingle(txtPrice.Text.Trim());
                b.Press = txtPress.Text.Trim();
                b.Author = txtAuthor.Text.Trim();
                b.EnterTime = dateTimePicker1.Value.ToShortDateString();
                if (DBOperate.writeDB("insert into Books values('" + b.ISBN + "','" + b.Name + "','" + b.Style + "','" + b.Price + "','" + b.Press + "','" + b.Author + "','" + b.EnterTime + "','否')") > 0)
                { ///把选择的图片上传给阿里云oss
                    if (txtName.Text.Trim() != null)
                    {
                        Mac mac = new Mac("1VC_A1ZMeAe3PYJoEXPPVWlHbHLdJ9gTH8hZY0WC", "Eg0e8u3qlHWAH_GdASR1xXdkzYbEb-v85PyNWKhX");
                        // 上传文件名 
                        string key = @"books/" + txtISBN.Text.Trim() + ".jpg";
                        // 本地文件路径
                        string filePath = @"" + upUserStrBook + "";
                        // 存储空间名
                        string Bucket = "windsearcher";

                        PutPolicy putPolicy = new PutPolicy();
                        // 设置要上传的目标空间
                        putPolicy.Scope = Bucket;
                        // 上传策略的过期时间(单位:秒)
                        putPolicy.SetExpires(3600);
                        // 文件上传完毕后，在多少天后自动被删除
                        putPolicy.DeleteAfterDays = 1;
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
                        MessageBox.Show("保存成功!");
                    }
                    else
                    {
                        MessageBox.Show("保存失败!");
                    }
                }
            }
        }

        private void btnAClear_Click(object sender, EventArgs e)
        {
            txtAName.Text = "";
            txtAMobile.Text = "";
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            int index = dataGridView1.CurrentRow.Index; //获取选中行的行号
            txtNumInfo.Text = dataGridView1.Rows[index].Cells[0].Value.ToString();
            txtMobileInfo.Text=dataGridView1.Rows[index].Cells[2].Value.ToString();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            Users u = new Users();
            u.Account = txtNumInfo.Text.Trim();
            u.Mobile = txtMobileInfo.Text.Trim();
            if (DBOperate.writeDB("update  UserInfo set UserMobile='" + u.Mobile +  "' where UserAccount='" + u.Account + "'") > 0)
            {
                MessageBox.Show("修改成功！");
                txtNumInfo.Text = "";
                txtMobileInfo.Text = "";
            }
        }

        private void btnDele_Click(object sender, EventArgs e)
        {
            Users u = new Users();
            u.Account = txtNumInfo.Text.Trim();
            if (DBOperate.writeDB("delete from UserInfo where UserAccount='" + u.Account + "'") > 0)
            {
                MessageBox.Show("删除成功！");
                txtNumInfo.Text = "";
                txtMobileInfo.Text = "";
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            Book b = new Book();
            b.ISBN = txtBookISBN1.Text.Trim();
            b.Name = txtBName1.Text.Trim();
            DataSet ds = null;
            string search = "%" + txtBSearch.Text.Trim() + "%";
            if (txtBSearch.Text.Trim()=="")
            {
                ds = DBOperate.readDB("select ISBN,BookName 书籍名称,BookStyle 书籍类型,Price 价格,Press 出版社,Author 作者,EnterTime 购入时间,IsBorrow 是否借出 from Books");
                dataGridView2.DataSource = ds.Tables[0];
            }
            else if (cboBook.Text == "ISBN")
            {
                ds = DBOperate.readDB("select ISBN,BookName 书籍名称,BookStyle 书籍类型,Price 价格,Press 出版社,Author 作者,EnterTime 购入时间,IsBorrow 是否借出 from Books where ISBN like '" + search + "'");
                dataGridView2.DataSource = ds.Tables[0];
            }
            else if (cboBook.Text == "图书名称")
            {
                ds = DBOperate.readDB("select ISBN,BookName 书籍名称,BookStyle 书籍类型,Price 价格,Press 出版社,Author 作者,EnterTime 购入时间,IsBorrow 是否借出 from Books where BookName like '" + search + "'");
                dataGridView2.DataSource = ds.Tables[0];
            }
        }

        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            int index = dataGridView2.CurrentRow.Index; //获取选中行的行号
            //让文本框中数据对应到数据库中的数据
            txtBookISBN1.Text = dataGridView2.Rows[index].Cells[0].Value.ToString();
            txtBName1.Text = dataGridView2.Rows[index].Cells[1].Value.ToString();
            txtBStyle.Text= dataGridView2.Rows[index].Cells[2].Value.ToString();
            txtPrice1.Text = dataGridView2.Rows[index].Cells[3].Value.ToString();
            txtPress1.Text = dataGridView2.Rows[index].Cells[4].Value.ToString();
            txtAuthor1.Text = dataGridView2.Rows[index].Cells[5].Value.ToString();
            dateTimePicker2.Value = Convert.ToDateTime( dataGridView2.Rows[index].Cells[6].Value);
            IsBorrow1.Text = dataGridView2.Rows[index].Cells[7].Value.ToString();
            //让文本框中的数据变为只读
            txtBookISBN1.ReadOnly = true;
            txtBName1.ReadOnly = true;
            txtBStyle.ReadOnly = true;
            txtPrice1.ReadOnly = true;
            txtPress1.ReadOnly = true;
            txtAuthor1.ReadOnly = true;
            dateTimePicker2.Enabled = false;
            IsBorrow1.ReadOnly = true;
            btnAlter.Enabled = false;
            btnDelete.Enabled = false;
        }
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            txtBName1.ReadOnly = false;
            txtBStyle.ReadOnly = false;
            txtPrice1.ReadOnly = false;
            txtPress1.ReadOnly = false;
            txtAuthor1.ReadOnly = false;
            dateTimePicker2.Enabled = true;
            IsBorrow1.ReadOnly = false;
            btnAlter.Enabled = true;
            btnDelete.Enabled = true;
        }

        private void btnAlter_Click(object sender, EventArgs e)
        {
            Book b = new Book();
            b.ISBN = txtBookISBN1.Text.Trim();
            b.Name = txtBName1.Text.Trim();
            b.Style = txtBStyle.Text.Trim();
            b.Price = Convert.ToSingle(txtPrice1.Text.Trim());
            b.Press = txtPress1.Text.Trim();
            b.Author = txtAuthor1.Text.Trim();
            b.EnterTime = dateTimePicker2.Value.ToShortDateString();
            b.IsBorrow=IsBorrow1.Text.Trim();
            if (DBOperate.writeDB("update Books set BookName='" + b.Name + "',BookStyle='" + b.Style + "',Price='" + b.Price + "',Press='" + b.Press + "',Author='" + b.Author + "',EnterTime='" + b.EnterTime + "',IsBorrow='" + b.IsBorrow + "' where ISBN='" + b.ISBN + "'")>0)
            {
                MessageBox.Show("修改成功!");
            }
            else
            {
                MessageBox.Show("修改失败!");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            Book b = new Book();
            b.ISBN = txtBookISBN1.Text.Trim();
            if (DBOperate.writeDB("delete from Books where ISBN='" + b.ISBN + "'") > 0)
            {
                MessageBox.Show("删除成功！");
            }
            else
                MessageBox.Show("删除失败！");
        }
 
        private void buttonUp_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd1 = new OpenFileDialog();
            ofd1.Filter = "图片文件|*.jpg;*.png;";
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
                pictureUpR.Image = imgClone;
            }
        }

        private void buttonUpPic_Click(object sender, EventArgs e)
        {
            if(txtName.Text.Trim()!=null)
            {
                Mac mac = new Mac("1VC_A1ZMeAe3PYJoEXPPVWlHbHLdJ9gTH8hZY0WC", "Eg0e8u3qlHWAH_GdASR1xXdkzYbEb-v85PyNWKhX");
                // 上传文件名 
                string key = @"users/" + txtName.Text.Trim() + ".jpg";
                // 本地文件路径
                string filePath = @"" + upUserStr + "";
                // 存储空间名
                string Bucket = "windsearcher";

                PutPolicy putPolicy = new PutPolicy();
                // 设置要上传的目标空间
                putPolicy.Scope = Bucket;
                // 上传策略的过期时间(单位:秒)
                putPolicy.SetExpires(3600);
                // 文件上传完毕后，在多少天后自动被删除
                putPolicy.DeleteAfterDays = 1;
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
                MessageBox.Show("请在左侧输入姓名");
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Trim() != null)
            {
                Mac mac = new Mac("1VC_A1ZMeAe3PYJoEXPPVWlHbHLdJ9gTH8hZY0WC", "Eg0e8u3qlHWAH_GdASR1xXdkzYbEb-v85PyNWKhX");
                // 上传文件名 
                string key = @"users/" + txtName.Text.Trim() + ".jpg";
                // 本地文件路径
                string filePath = @"" + upUserStr + "";
                // 存储空间名
                string Bucket = "windsearcher";

                PutPolicy putPolicy = new PutPolicy();
                // 设置要上传的目标空间
                putPolicy.Scope = Bucket;
                // 上传策略的过期时间(单位:秒)
                putPolicy.SetExpires(3600);
                // 文件上传完毕后，在多少天后自动被删除
                putPolicy.DeleteAfterDays = 1;
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
                MessageBox.Show(result.ToString());
            }
        }
        private void btnGSearch_Click(object sender, EventArgs e)
        {
            Users u = new Users();
            DataSet ds = null;
            u.Name = "%" + txtGSearch.Text.Trim() + "%";
            if (txtGAccount.Text.Trim() == "")
            {
                ds = DBOperate.readDB("select UserAccount 账号,UserName 姓名,UserMobile 手机号 from UserInfo where UserType='管理员'");
                dataGridView3.DataSource = ds.Tables[0];
            }
            else
            {
                ds = DBOperate.readDB("select UserAccount 账号,UserName 姓名,UserMobile 手机号 from UserInfo where UserName = '" + u.Name + "' and UserType='管理员'");
                dataGridView3.DataSource = ds.Tables[0];
                if (ds.Tables[0].Rows.Count > 0)
                {
                    txtGAccount.Text = (ds.Tables[0].Rows[0][0]).ToString();
                    txtGMobile.Text = (ds.Tables[0].Rows[0][2]).ToString();
                    txtGEmail.Text = (ds.Tables[0].Rows[0][3]).ToString();
                }
            }
        }

        private void dataGridView3_SelectionChanged(object sender, EventArgs e)
        {
            int index = dataGridView3.CurrentRow.Index; //获取选中行的行号
            txtGAccount.Text = dataGridView3.Rows[index].Cells[0].Value.ToString();
            txtGMobile.Text = dataGridView3.Rows[index].Cells[2].Value.ToString();
            txtGEmail.Text = dataGridView3.Rows[index].Cells[3].Value.ToString();
            txtGMobile.ReadOnly = true;
            txtGEmail.ReadOnly = true;
            btnGUpdate.Enabled = false;
            btnGDele.Enabled = false;
        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            txtGMobile.ReadOnly = false;
            txtGEmail.ReadOnly = false;
            btnGUpdate.Enabled = true;
            btnGDele.Enabled = true;
        }

        private void btnGUpdate_Click(object sender, EventArgs e)
        {
            Users u = new Users();
            u.Account = txtGAccount.Text.Trim();
            u.Mobile = txtGMobile.Text.Trim();
            if (DBOperate.writeDB("update UserInfo set UserMobile='" + u.Mobile + "' where UserAccount = '" + u.Account + "'") > 0)
            {
                MessageBox.Show("修改成功！");
            }
            else
                MessageBox.Show("修改失败！");
        }

        private void btnGDele_Click(object sender, EventArgs e)
        {
             Users u = new Users();
             u.Account = txtGAccount.Text.Trim();
            if (DBOperate.writeDB("delete from UserInfo where UserAccount='" + u.Account + "'") > 0)
            {
                MessageBox.Show("删除成功！");
            }
            else
                MessageBox.Show("删除失败！");
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog ofd1 = new OpenFileDialog();
            ofd1.Filter = "图片文件|*.jpg;*.png;";
            if (ofd1.ShowDialog() == DialogResult.OK)
            {
                upUserStrBook = ofd1.FileName;
                // 获取图片文件
                Image imgPhoto = Image.FromFile(ofd1.FileName);
                // 克隆这个图片文件
                Image imgClone = new Bitmap(imgPhoto);
                // 将原始的图片文件销毁
                imgPhoto.Dispose();
                // 将克隆的图片文件赋值
                pictureUpBook.Image = imgClone;
            }
        }

        private void btnCancellation_Click(object sender, EventArgs e)
        {
            Login a = new Login();
            a.Show();
            this.Close();
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
                // 退出进程并把所有线程关闭
                System.Environment.Exit(0);
            }
            base.WndProc(ref msg);
        }

        private void btnAUp_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label32_Click(object sender, EventArgs e)
        {

        }

        private void logout_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }
    }
}
