using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO.Ports;
namespace HUEREN
{
    public delegate void ShowWindow();
    public delegate void HideWindow();
    public delegate void OpenPort();
    public delegate void ClosePort();
    public delegate Point GetMainPos();
    public delegate int GetMainWidth();
    public partial class Form1 : Form
    {
        Form2 Displayer;
        private long receive_count = 0;
        private long send_count = 0;
        private StringBuilder sb = new StringBuilder();     //为了避免在接收处理函数中反复调用，依然声明为一个全局变量
        private DateTime current_time = new DateTime();    //为了避免在接收处理函数中反复调用，依然声明为一个全局变量
        private StringBuilder builder = new StringBuilder();    //避免在事件处理方法中反复创建，定义为全局
        public Form1()
        {
            InitializeComponent();
            serialPort1.Encoding = Encoding.GetEncoding("GB2312");

            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;


        }
        public void ClosePort()//关闭串口，供委托调用
        {
            try
            {
                serialPort1.Close();    //关闭串口
                button1.Text = "打开串口";
                label5.Text = "串口已关闭";
                label5.ForeColor = Color.Red;
                button1.BackColor = Color.ForestGreen;
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
                comboBox5.Enabled = true;
            }
            catch (System.Exception)
            {

            }
        }

        private Point GetMyPos()//供委托调用
        {
            return this.Location;
        }

        public void OpenPort()//打开串口，供委托调用
        {
            try
            {
                //串口已经处于关闭状态，则设置好串口属性后打开
                comboBox1.Enabled = false;
                comboBox2.Enabled = false;
                comboBox3.Enabled = false;
                comboBox4.Enabled = false;
                comboBox5.Enabled = false;

                serialPort1.PortName = comboBox1.Text;
                serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);
                serialPort1.DataBits = Convert.ToInt16(comboBox3.Text);



                if (comboBox4.Text.Equals("无"))
                    serialPort1.Parity = System.IO.Ports.Parity.None;
                else if (comboBox4.Text.Equals("奇校验"))
                    serialPort1.Parity = System.IO.Ports.Parity.Odd;
                else if (comboBox4.Text.Equals("偶校验"))
                    serialPort1.Parity = System.IO.Ports.Parity.Even;
                //else if (comboBox4.Text.Equals("Mark"))
                //  serialPort1.Parity = System.IO.Ports.Parity.Mark;
                // else if (comboBox4.Text.Equals("Space"))
                //  serialPort1.Parity = System.IO.Ports.Parity.Space;

                if (comboBox5.Text.Equals("1"))
                    serialPort1.StopBits = System.IO.Ports.StopBits.One;
                else if (comboBox5.Text.Equals("1.5"))
                    serialPort1.StopBits = System.IO.Ports.StopBits.OnePointFive;
                else if (comboBox5.Text.Equals("2"))
                    serialPort1.StopBits = System.IO.Ports.StopBits.Two;

                serialPort1.Open();     //打开串口
                button1.Text = "关闭串口";
                button1.BackColor = Color.Firebrick;
                label5.Text = "串口已打开";
                label5.ForeColor = Color.Green;
            }
            catch (System.Exception)
            {
                MessageBox.Show("串口打开失败，请检查", "错误");
            }
        }
        public void ShowMe()//供委托调用
        {
            this.Show();
        }
        public void HideMe()//供委托调用
        {
            this.Hide();
        }
        private void CreateNewDrawer()//创建ADC绘制窗口
        {
            Displayer = new Form2();//创建新对象
            Displayer.ShowMainWindow = new ShowWindow(ShowMe);//初始化类成员委托
            Displayer.HideMainWindow = new HideWindow(HideMe);
            Displayer.GetMainPos = new GetMainPos(GetMyPos);
            Displayer.CloseSerialPort = new ClosePort(ClosePort);
            Displayer.OpenSerialPort = new OpenPort(OpenPort);
            Displayer.GetMainWidth = new GetMainWidth(GetMyWidth);
            Displayer.Show();//显示窗口
        }

        int GetMyWidth()//供委托调用
        {
            return this.Width;
        }

        private void CreateDisplayer()
        {
            this.Left = 0;
            CreateNewDrawer();
            Rectangle Rect = Screen.GetWorkingArea(this);
            Displayer.SetWindow(Rect.Width - this.Width, new Point(this.Width, this.Top));//设置绘制窗口宽度，以及坐标
        }
       
        private void button5_Click(object sender, EventArgs e)
        {
            if (Displayer == null)//第一次创建Displayer = null
            {
                CreateDisplayer();
            }
            else
            {
                if (Displayer.IsDisposed)//多次创建通过判断IsDisposed确定串口是否已关闭，避免多次创建
                {
                    CreateDisplayer();
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            int i;
            for (i = 300; i <= 38400; i = i * 2)
            {
                comboBox2.Items.Add(i.ToString());  //添加波特率列表
            }
            //批量添加波特率列表
            string[] baud = { "43000", "56000", "57600", "115200", "128000", "230400", "256000", "460800" };
            comboBox2.Items.AddRange(baud);
            //设置默认值
            comboBox1.Text = "";
            comboBox2.Text = "115200";
            comboBox3.Text = "8";
            comboBox4.Text = "无";
            comboBox5.Text = "1";
            comboBox1.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
            
        }
        
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            


        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //将可能产生异常的代码放置在try块中
                //根据当前串口属性来判断是否打开
                if (serialPort1.IsOpen)
                {
                    //串口已经处于打开状态
                    serialPort1.Close();    //关闭串口
                    button1.Text = "打开串口";
                    label5.Text = "串口已关闭";
                    label5.ForeColor = Color.Red;
                    button1.BackColor = Color.ForestGreen;
                    comboBox1.Enabled = true;
                    comboBox2.Enabled = true;
                    comboBox3.Enabled = true;
                    comboBox4.Enabled = true;
                    comboBox5.Enabled = true;
                    //textBox2_receive.Text = "";  //清空接收区
                    //textBox1_send.Text = "";     //清空发送区
                }
                else
                {
                    //串口已经处于关闭状态，则设置好串口属性后打开
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = false;
                    comboBox3.Enabled = false;
                    comboBox4.Enabled = false;
                    comboBox5.Enabled = false;

                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);
                    serialPort1.DataBits = Convert.ToInt16(comboBox3.Text);
                    


                    if (comboBox4.Text.Equals("无"))
                        serialPort1.Parity = System.IO.Ports.Parity.None;
                    else if (comboBox4.Text.Equals("奇校验"))
                        serialPort1.Parity = System.IO.Ports.Parity.Odd;
                    else if (comboBox4.Text.Equals("偶校验"))
                        serialPort1.Parity = System.IO.Ports.Parity.Even;
                    //else if (comboBox4.Text.Equals("Mark"))
                      //  serialPort1.Parity = System.IO.Ports.Parity.Mark;
                   // else if (comboBox4.Text.Equals("Space"))
                     //  serialPort1.Parity = System.IO.Ports.Parity.Space;

                    if (comboBox5.Text.Equals("1"))
                        serialPort1.StopBits = System.IO.Ports.StopBits.One;
                    else if (comboBox5.Text.Equals("1.5"))
                        serialPort1.StopBits = System.IO.Ports.StopBits.OnePointFive;
                    else if (comboBox5.Text.Equals("2"))
                        serialPort1.StopBits = System.IO.Ports.StopBits.Two;

                    serialPort1.Open();     //打开串口
                    button1.Text = "关闭串口";
                    button1.BackColor = Color.Firebrick;
                    label5.Text = "串口已打开";
                    label5.ForeColor = Color.Green;
                }
            }
            catch (Exception ex)
            {
               //捕获可能发生的异常并进行处理

                //捕获到异常，创建一个新的对象，之前的不可以再用
               // serialPort1 = new System.IO.Ports.SerialPort();
                //刷新COM口选项
                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
                //响铃并显示异常给用户
                System.Media.SystemSounds.Beep.Play();
                button1.Text = "打开串口";
                button1.BackColor = Color.ForestGreen;
                MessageBox.Show(ex.Message);
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
                comboBox5.Enabled = true;       
            }

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            byte[] temp = new byte[1];
            try
            {
                //首先判断串口是否开启
                if (serialPort1.IsOpen)
                {
                    int num = 0;   //获取本次发送字节数
                    //串口处于开启状态，将发送区文本发送

                    //判断发送模式
                    if (radioButton4.Checked)
                    {
                        //以HEX模式发送
                        //首先需要用正则表达式将用户输入字符中的十六进制字符匹配出来
                        string buf = textBox1_send.Text;
                        string pattern = @"\s";
                        string replacement = "";
                        Regex rgx = new Regex(pattern);
                        string send_data = rgx.Replace(buf, replacement);

                        //不发送新行
                        num = (send_data.Length - send_data.Length % 2) / 2;
                        for (int i = 0; i < num; i++)
                        {
                            temp[0] = Convert.ToByte(send_data.Substring(i * 2, 2), 16);
                            serialPort1.Write(temp, 0, 1);  //循环发送
                        }
                        //如果用户输入的字符是奇数，则单独处理
                        if (send_data.Length % 2 != 0)
                        {
                            temp[0] = Convert.ToByte(send_data.Substring(textBox1_send.Text.Length - 1, 1), 16);
                            serialPort1.Write(temp, 0, 1);
                            num++;
                        }
                        //判断是否需要发送新行
                        if (checkBox2.Checked)
                        {
                            //自动发送新行
                            
                            serialPort1.WriteLine("");
                        }
                    }
                    else
                    {
                        //以ASCII模式发送
                        //判断是否需要发送新行
                        if (checkBox2.Checked)
                        {
                            //自动发送新行
                            serialPort1.WriteLine(textBox1_send.Text);
                            
                            num = textBox1_send.Text.Length + 2; //回车占两个字节
                        }
                        else
                        {
                            //不发送新行
                            serialPort1.Write(textBox1_send.Text);
                            num = textBox1_send.Text.Length;
                        }
                    }

                    send_count += num;      //计数变量累加
                    label6.Text = "Tx:" + send_count.ToString() + "Bytes";   //刷新界面
                }
            }
            catch (Exception ex)
            {
                serialPort1.Close();
                //捕获到异常，创建一个新的对象，之前的不可以再用
                //serialPort1 = new System.IO.Ports.SerialPort();
                //刷新COM口选项
                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
                //响铃并显示异常给用户
                System.Media.SystemSounds.Beep.Play();
                button1.Text = "打开串口";
                button1.BackColor = Color.ForestGreen;
                MessageBox.Show(ex.Message);
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
                comboBox5.Enabled = true;
                checkBox2.Enabled = true;
            }
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {


            if (radioButton5.Checked)
            {
                int num = serialPort1.BytesToRead;      //获取接收缓冲区中的字节数
              //  byte[] received_buf = new byte[num];    //声明一个大小为num的字节数据用于存放读出的byte型数据

                receive_count += num;
                try
                {
                    //因为要访问UI资源，所以需要使用invoke方式同步ui
                    Invoke((EventHandler)(delegate
                    {
                        if (checkBox1.Checked)
                        {
                            current_time = System.DateTime.Now; //获取当前时间

                            textBox2_receive.AppendText(current_time.ToString("HH:mm:ss") + "  " + serialPort1.ReadExisting());
                          

                        }
                        else
                        {
                            //不显示时间
                         textBox2_receive.AppendText(serialPort1.ReadExisting());
                        }
                        label7.Text = "Rx:" + receive_count.ToString() + "Bytes";
                    }
                      )
                    );
                }
                catch (Exception ex)
                {
                    //响铃并显示异常给用户
                    System.Media.SystemSounds.Beep.Play();
                    MessageBox.Show(ex.Message);

                }
            }
            else
            {
                int num = serialPort1.BytesToRead;      //获取接收缓冲区中的字节数
                byte[] received_buf = new byte[num];    //声明一个大小为num的字节数据用于存放读出的byte型数据

                receive_count += num;                   //接收字节计数变量增加nun
                serialPort1.Read(received_buf, 0, num);   //读取接收缓冲区中num个字节到byte数组中
                if (Displayer != null)
                    Displayer.AddData(received_buf);

                sb.Clear();     //防止出错,首先清空字符串构造器
                                //遍历数组进行字符串转化及拼接

                if (radioButton2.Checked)
                {
                    //选中HEX模式显示
                    foreach (byte b in received_buf)
                    {
                        sb.Append(b.ToString("X2") + ' ');    //将byte型数据转化为2位16进制文本显示,用空格隔开
                    }
                }
                else if  (radioButton1.Checked)
                {
                    //选中ASCII模式显示

                    sb.Append(Encoding.ASCII.GetString(received_buf));  //将整个数组解码为ASCII数组
                }


                try
                {
                    //因为要访问UI资源，所以需要使用invoke方式同步ui
                    Invoke((EventHandler)(delegate
                    {
                        if (checkBox1.Checked)
                        {
                            current_time = System.DateTime.Now; //获取当前时间
                            
                            textBox2_receive.AppendText(current_time.ToString("HH:mm:ss") + "  " + sb.ToString());

                        }
                        else
                        {
                            //不显示时间
                            if (radioButton5.Checked)
                            {

                            }
                            else
                                textBox2_receive.AppendText(sb.ToString());


                        }
                        label7.Text = "Rx:" + receive_count.ToString() + "Bytes";
                    }
                      )
                    );
                }
                catch (Exception ex)
                {
                    //响铃并显示异常给用户
                    System.Media.SystemSounds.Beep.Play();
                    MessageBox.Show(ex.Message);

                }

            }
        }
      
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox2_receive.Text = "";  //清空接收文本框
            receive_count = 0;          //计数清零
            label7.Text = "Rx:" + receive_count.ToString() + "Bytes";   //刷新界面

        }

        private void button4_Click(object sender, EventArgs e)
        {
            
            textBox1_send.Text = "";     //清空发送文本框
            send_count = 0;          //计数清零
            label6.Text = "Tx:" + send_count.ToString() + "Bytes";   //刷新界面
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                //自动发送功能选中,开始自动发送
                numericUpDown1.Enabled = false;     //失能时间选择
                try
                {
                    timer1.Interval = (int)numericUpDown1.Value;     //定时器赋初值
                    timer1.Start();     //启动定时器
                    label5.Text = "串口已打开" + " 自动发送中...";
                }
                //catch (Exception ex)
                catch(System.Exception)
                {
                    //响铃并显示异常给用户
                    System.Media.SystemSounds.Beep.Play();
                    timer1.Stop();     //关闭定时器
                    label5.Text = "串口已关闭" + " 停止自动发送";
                                        //MessageBox.Show(ex.Message);
                    MessageBox.Show("请正确设置时间","提示");

                }
               // timer1.Start();     //启动定时器
                //label5.Text = "串口已打开" + " 自动发送中...";
            }
            else
            {
                //自动发送功能未选中,停止自动发送
                numericUpDown1.Enabled = true;     //使能时间选择
                timer1.Stop();     //停止定时器
                label5.Text = "串口已打开";

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //定时时间到
            button2_Click(button2, new EventArgs());    //调用发送按钮回调函数
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
