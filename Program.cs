
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
//以下是添加的命名空间
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;
using System.Threading;
private IPEndPoint ServerEndPoint = null;                             //定义网络端点
private UdpClient UDP_Server = new UdpClient();            //创建网络服务，即UDP的Socket
private System.Threading.Thread thdUdp;                               //创建一个线程
//定义了一个托管方法
public delegate void DataArrivalEventHandler(byte[] Data, IPAddress Ip, int Port);
//通过托管在控件中定义一个事件
public event DataArrivalEventHandler DataArrival;
private string localHost = "127.0.0.1";                               //设置默认的IP地址
//在“属性”窗口中显示localHost属性
[Browsable(true), Category("Local"), Description("本地IP地址")]
public string LocalHost                                            //封装字段
{
    get { return localHost; }
    set { localHost = value; }
}
private int localPort = 11000;                                    //设置默认端口号
//在“属性”窗口中显示localPort属性
[Browsable(true), Category("Local"), Description("本地端口号")]
public int LocalPort                                                 //封装字段
{
    get { return localPort; }
    set { localPort = value; }
}
private bool active = false;
[Browsable(true), Category("Local"), Description("激活监听")] //在“属性”窗口中显示active属性
public bool Active                                                    //封装字段
{
    get { return active; }
    set                                                                 //该属性读取值
    {
        active = value;
        if (active)                                                //当值为True时
        {
            OpenSocket();                                       //打开监听
        }
        else
        {
            CloseSocket();                                         //关闭监听
        }
    }
}
public UDPSocket ()
{
    InitializeComponent();
}
public UDPSocket (IContainer container)
{
    container.Add(this);
    InitializeComponent();
}
private void OpenSocket()
{
    Listener();                                //调用监听方法
}
private void CloseSocket()
{
    if (UDP_Server != null)              //当Socket不为空时
        UDP_Server.Close();            //关闭Socket
    if (thdUdp != null)                     //如果子线程正在被使用
    {
        Thread.Sleep(30);               //使主线程睡眠
        thdUdp.Abort();                   //中断子线程
    }
}
protected void Listener()   //监听
{
    //将IP地址和端口号以网络端点存储
    ServerEndPoint = new IPEndPoint(IPAddress.Any, localPort);
    if (UDP_Server != null)
        UDP_Server.Close();                                        //关闭初始化的Socket
    UDP_Server = new UdpClient(localPort);                        //创建一个新的端口号监听
    try
    {
        thdUdp = new Thread(new ThreadStart(GetUDPData));     //创建一个线程
        thdUdp.Start();                                                       //执行当前线程
    }
    catch (Exception e)
    {
        MessageBox.Show(e.ToString());                                   //显示线程的错误信息
    }
}
private void GetUDPData()                                            //获取当前接收的消息
{
    while (active)                                                   //当监听打开时
    {
        try
        {
            byte[] Data = UDP_Server.Receive(ref ServerEndPoint);//存储获取的远程消息
            if (DataArrival != null)
            {
                //调用DataArrival事件
                DataArrival(Data, ServerEndPoint.Address, ServerEndPoint.Port);
            }
            Thread.Sleep(0);
        }
        catch { }
    }
}
public void Send(System.Net.IPAddress Host, int Port, byte[] Data)
{
    try
    {
        IPEndPoint server = new IPEndPoint(Host, Port);      //用指定的IP地址和端口号初始化server
        UDP_Server.Send(Data, Data.Length, server);            //将消息发送给远程客户端
    }
    catch (Exception e)
    {
        MessageBox.Show(e.ToString());
    }
}
public class Publec_Class
{
    public static string ServerIP = "";                                     //定义服务器端的IP
    public static string ServerPort = "";                                  //定义服务器端的端口号
    public static string ClientIP = "";                                //定义客户端的IP
    public static string ClientName = "";                                 //定义客端的名称
    public static string UserName;                                  //定义用户名称
    public static string UserID;                                       //定义用户的ID号
    public string MyHostIP()                                      //遍历服务器端的所有IP地址
    {
        string hostname = Dns.GetHostName();                            //显示主机名
        IPHostEntry hostent = Dns.GetHostEntry(hostname); //主机信息
        Array addrs = hostent.AddressList;                    //与主机关联的IP地址列表
        IEnumerator it = addrs.GetEnumerator();                   //迭代器，添加命名空间using System.Collections
        while (it.MoveNext())                                       //循环到下一个IP 地址
        {
            IPAddress ip = (IPAddress)it.Current;         //获得IP地址，添加命名空间using System.Net
            return ip.ToString();
        }
        return "";
    }
    [DllImport("kernel32")]
    public static extern void GetWindowsDirectory(StringBuilder WinDir, int count);
    public string Get_windows()                                 //获取Windows目录
    {
        const int nChars = 255;
        StringBuilder Buff = new StringBuilder(nChars);            //定义一个nChars大小的可变字符串
        GetWindowsDirectory(Buff, nChars);                           //获取Windows目录
        return Buff.ToString();                                         //返回目录信息
    }
}
public void add(Form f)              //将接收的窗体添加到列表中
{
    base.InnerList.Add(f);
}
public void Romove(Form f)
{
    base.InnerList.Remove(f);         //在列表中移除指定的窗体
}
public Form this[int index]                //在窗体列表中查找指定的窗体
{
    get
    {
        return ((Form)List[index]);
    }
    set
    {
        List[index] = value;
    }
}
[Serializable]
public class ClassMsg
{
    public String SID = "";                                                    //发送方编号
    public String SIP = "";                                                     //发送方IP
    public String SPort = "";                                           //发送方端口号
    public String RID = "";                                             //接收方编号
    public String RIP = "";                                                    /接收方IP
    public String RPort = "";                                                 //接收方端口号
    public SendKind sendKind = SendKind.SendNone;                    //发送消息类型，默认为无类型
    public MsgCommand msgCommand = MsgCommand.None;     //消息命令
    public SendState sendState = SendState.None;                          /消息发送状态
    public String msgID = "";                                                //消息ID，GUID
    public byte[] Data;
}
/// <summary>
/// 用户注册信息
/// </summary>
[Serializable]
public class RegisterMsg
{
    public string UserName;                                           //用户名
    public string PassWord;                                           //密码
}
/// <summary>
/// 消息命令
/// </summary>
public enum MsgCommand                                                    //定义枚举类型
{
    None,
    Registering,                                                            //用户注册
    Registered,                                                             //用户注册结束
    Logining,                                                                //用户登录
    Logined,                                                                 //用户登录结束，上线
    SendToOne,                                                            //发送单用户
    SendToAll,                                                              //发送所有用户
    UserList,                                                                //用户列表
    UpdateState,                                                           //更新用户状态
    VideoOpen,                                                      //打开视频
    Videoing,                                                                //正在视频
    VideoClose,                                                      //关闭视频
    Close                                                                     //下线
}
/// <summary>
/// 发送类型
/// </summary>
public enum SendKind                                                    //定义枚举类型
{
    SendNone,                                                              //无类型
    SendCommand,                                                       //发送命令
    SendMsg,                                                               //发送消息
    SendFile                                                                 //发送文件
}
/// <summary>
/// 发送状态
/// </summary>
public enum SendState                                                    //定义枚举类型
{
    None,                                                                     //无状态
    Single,                                                             //单消息或文件
    Start,                                                                     //发送开始生成文件
    Sending,                                                                 //正在发送中，写入文件
    End                                                                        //发送结束
}
private string ConStr = @"Data Source=127.0.0.1;Initial Catalog=db_MyQQData;User ID=sa";
//执行任何SQL语句，返回所影响的行数
public int ExSQL(string SQLStr)
{
    try
    {
        SqlConnection cnn = new SqlConnection(ConStr);            //用SqlConnection对象与指定的数据库连接
        SqlCommand cmd = new SqlCommand(SQLStr, cnn);              //创建一个SqlCommand对象，执行SQL语句
        cnn.Open();                                                            //打开数据库的连接
        int i = 0;
        i = cmd.ExecuteNonQuery();                                           //获取当前所影响的行数
        cmd.Dispose();                                                       //释放cmd所使用的资源
        cnn.Close();                                                            //关闭与数据库的连接
        cnn.Dispose();                                                 //释放cnn所使用的资源
        return i;                                                           //返回行数
    }
    catch { return 0; }
}
public SqlDataReader ExSQLReDr(string SQLStr)      //执行任何SQL查询语句，返回一个SqlDataReader对象
{
    try
    {
        SqlConnection cnn = new SqlConnection(ConStr);            //用SqlConnection对象与指定的数据库相连接
        SqlCommand cmd = new SqlCommand(SQLStr, cnn);              //创建一个SqlCommand对象，执行SQL语句
        cnn.Open();                                                            //关闭数据库的连接
        SqlDataReader dr;                                            //定义一个SqlDataReader对象
        dr = cmd.ExecuteReader();                                      //将数据表中的信息存入到SqlDataReader对象中
        return dr;
    }
    catch { return null; }
}
public System.IO.MemoryStream SerializeBinary(object request)                    //将对象序列化为二进制流
{
    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter serializer = new
    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
    System.IO.MemoryStream memStream = new System.IO.MemoryStream();//创建一个内存流存储区
    serializer.Serialize(memStream, request);                                           //将对象序列化为二进制流
    return memStream;
}
public object DeSerializeBinary(System.IO.MemoryStream memStream)         //将二进制流反序列化为对象
{
    memStream.Position = 0;
    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter deserializer = new
    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
    object newobj = deserializer.Deserialize(memStream);                       //将二进制流反序列化为对象
    memStream.Close();                                                            //关闭内存流并释放
    return newobj;
}
[Serializable]
public class ClassUserInfo
{
    private string userID;                         //用户编号
    private string userIP;                          //IP地址
    private string userPort;                        //端口号
    private string userName;                     //用户名
    private String state;                      //当前用户状态
}
[Serializable]                                                   //指示一个类可以序列化
public class ClassUsers : System.Collections.CollectionBase
{
    public void add(ClassUserInfo userInfo)                    //将当前用户信息添加到列表中
    {
        base.InnerList.Add(userInfo);
    }
    public void Romove(ClassUserInfo userInfo)                    //在列表中移除指定的用户
    {
        base.InnerList.Remove(userInfo);
    }
    public ClassUserInfo this[int index]                           //根据索引号，在列表中查找指定的用户信息
    {
        get
        {
            return ((ClassUserInfo)List[index]);
        }
        set
        {
            List[index] = value;
        }
    }
}
public class VideoAPI  //视频API类
{
    // 视频API调用
    [DllImport("avicap32.dll")]
    public static extern IntPtr capCreateCaptureWindowA(byte[] lpszWindowName, int dwStyle, int x, int y, int
        nWidth, int nHeight, IntPtr hWndParent, int nID);
    [DllImport("avicap32.dll")]
    public static extern bool capGetDriverDescriptionA(short wDriver, byte[] lpszName, int cbName, byte[]
          lpszVer, int cbVer);
    [DllImport("User32.dll")]
    public static extern bool SendMessage(IntPtr hWnd, int wMsg, bool wParam, int lParam);
    [DllImport("User32.dll")]
    public static extern bool SendMessage(IntPtr hWnd, int wMsg, short wParam, int lParam);
    //常量
    public const int WM_USER = 0x400;
    public const int WS_CHILD = 0x40000000;
    public const int WS_VISIBLE = 0x10000000;
    public const int SWP_NOMOVE = 0x2;
    public const int SWP_NOZORDER = 0x4;
    public const int WM_CAP_DRIVER_CONNECT = WM_USER + 10;
    public const int WM_CAP_DRIVER_DISCONNECT = WM_USER + 11;
    public const int WM_CAP_SET_CALLBACK_FRAME = WM_USER + 5;
    public const int WM_CAP_SET_PREVIEW = WM_USER + 50;
    public const int WM_CAP_SET_PREVIEWRATE = WM_USER + 52;
    public const int WM_CAP_SET_VIDEOFORMAT = WM_USER + 45;
    public const int WM_CAP_START = WM_USER;
    public const int WM_CAP_SAVEDIB = WM_CAP_START + 25;
}
public class cVideo                                                            //视频类
{
    private IntPtr lwndC;                                                 //保存无符号句柄
    private IntPtr mControlPtr;                                             //保存管理指示器
    private int mWidth;
    private int mHeight;
    public cVideo(IntPtr handle, int width, int height)
    {
        mControlPtr = handle;                                      //显示视频控件的句柄
        mWidth = width;                                                 //视频宽度
        mHeight = height;                                                 //视频高度
    }
    public void StartWebCam()                                              //打开视频设备
    {
        byte[] lpszName = new byte[100];
        byte[] lpszVer = new byte[100];
        VideoAPI.capGetDriverDescriptionA(0, lpszName, 100, lpszVer, 100);
        this.lwndC = VideoAPI.capCreateCaptureWindowA(lpszName, VideoAPI.WS_CHILD |
            VideoAPI.WS_VISIBLE, 0, 0, mWidth, mHeight, mControlPtr, 0);
        if (VideoAPI.SendMessage(lwndC, VideoAPI.WM_CAP_DRIVER_CONNECT, 0, 0))
        {
            VideoAPI.SendMessage(lwndC, VideoAPI.WM_CAP_SET_PREVIEWRATE, 100, 0);
            VideoAPI.SendMessage(lwndC, VideoAPI.WM_CAP_SET_PREVIEW, true, 0);
        }
    }
    public void CloseWebcam()                                           //关闭视频设备
    {
        VideoAPI.SendMessage(lwndC, VideoAPI.WM_CAP_DRIVER_DISCONNECT, 0, 0);
    }
    public void GrabImage(IntPtr hWndC, string path)                  //拍照
    {
        IntPtr hBmp = Marshal.StringToHGlobalAnsi(path);
        VideoAPI.SendMessage(lwndC, VideoAPI.WM_CAP_SAVEDIB, 0, hBmp.ToInt32());
    }
}
private void button_OK_Click(object sender, EventArgs e)
{
    if (text_PassWord.Text.Trim() == text_PassWord2.Text.Trim())
    {
        RegisterMsg registermsg = new RegisterMsg();          //创建并引用QQClass类库中的RegisterMsg类
        registermsg.UserName = text_Name.Text;                 //记录用户名
        registermsg.PassWord = text_PassWord.Text;            //记录密码
        //通过类库中的SerializeBinary方法，将registermsg序列化为二进制流
        byte[] registerData = new ClassSerializers().SerializeBinary(registermsg).ToArray();
        ClassMsg msg = new ClassMsg();                                   //引用类库中的ClassMsg类
        msg.sendKind = SendKind.SendCommand;         //设置为发送命令
        msg.msgCommand = MsgCommand.Registering;       //将消息命令设为用户注册
        msg.Data = registerData;                                   //将二制流存入到类库中的二进制变量Data中
        serID = text_IP.Text.Trim();                                     //存储主机的IP地址
        udpSocket1.Send(IPAddress.Parse(serID), Convert.ToInt32(text_IP5.Text.Trim()), new
ClassSerializers().SerializeBinary(msg).ToArray());                   //用udpSocket1控件的Send方法将消息发给服务器
    }
    else
    {
        text_PassWord.Text = "";
        text_PassWord2.Text = "";
        MessageBox.Show("密码与确认密码不匹配，请重新输入。");
    }
}
private void sockUDP1_DataArrival(byte[] Data, System.Net.IPAddress Ip, int Port)
{
    DataArrivaldelegate outdelegate = new DataArrivaldelegate(DataArrival);   //托管
    this.BeginInvoke(outdelegate, new object[] { Data, Ip, Port });                //异步执行托管
}
//定义写入INI文件的API函数
[DllImport("kernel32")]
private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
private void DataArrival(byte[] Data, System.Net.IPAddress Ip, int Port)
{
    try
    {
        ClassMsg msg = new ClassSerializers().DeSerializeBinary((new System.IO.MemoryStream(Data)))
as ClassMsg;
        switch (msg.msgCommand)
        {
            case MsgCommand.Registered:         //注册成功
                DialogResult = DialogResult.OK;   //设置窗体的对话框结果
                //向INI文件写入服务器IP地址
                WritePrivateProfileString("MyQQ", "ID", serID, PubClass.Get_windows() +
"\\Server.ini");
                WritePrivateProfileString("MyQQ", "Port", text_IP5.Text.Trim(), PubClass.Get_windows()
 + "\\Server.ini");//向INI文件写入端口号
                WritePrivateProfileString("MyQQ", "Name",
text_Name.Text.Trim(), PubClass.Get_windows() + "\\Server.ini");//向INI文件写入用户名称
                break;
        }
    }
    catch { }
}
[DllImport("kernel32")]                                            //定义读取INI文件的API函数
private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal,
int size, string filePath);
private void F_Logon_Load(object sender, EventArgs e)
{
    //如果程序启动目录中没有Server.ini文件
    if (System.IO.File.Exists(PubClass.Get_windows() + "\\Server.ini") == false)
    {
        F_SerSetup FrmSerSetup = new F_SerSetup();        //创建并引用注册窗体
        FrmSerSetup.Text = "用户注删";                            //设置注册窗体的名称
        if (FrmSerSetup.ShowDialog(this) == DialogResult.OK)//当注册窗体的对话框返回值为OK时
        {
            FrmSerSetup.Dispose();                                  //释放注册窗体的所有资源
        }
        else
        {
            FrmSerSetup.Dispose();
            DialogResult = DialogResult.Cancel;                   //将当前窗体的对话框返回值设为Cancel
        }
    }
    //如果Windows目录中有Server.ini文件
    if (System.IO.File.Exists(PubClass.Get_windows() + "\\Server.ini") == true)
    {
        Publec_Class.ServerIP = "";
        Publec_Class.ServerPort = "";
        //读取INI文件
        StringBuilder temp = new StringBuilder(255);
        //读取服务器的IP地址
        GetPrivateProfileString("MyQQ", "ID", "服务器地址读取错误。", temp, 255,
System.Environment.CurrentDirectory + "\\Server.ini");
        Publec_Class.ServerIP = temp.ToString();
        //读取端口号
        GetPrivateProfileString("MyQQ", "Port", "服务器端口号读取错误。", temp, 255,
System.Environment.CurrentDirectory + "\\Server.ini");
        Publec_Class.ServerPort = temp.ToString();
        //读取用户名称
        GetPrivateProfileString("MyQQ", "Name", "服务器端口号读取错误。", temp, 255,
System.Environment.CurrentDirectory + "\\Server.ini");
        Publec_Class.ClientName = temp.ToString();
    }
    else
    {
        DialogResult = DialogResult.Cancel;
    }
    udpSocket1.Active = true;                               //启动自定义的udpSocket1控件
}
private void button_QQLogon_Click(object sender, EventArgs e)
{
    //当读取到服务器的IP和端口号时
    if (Publec_Class.ServerPort != "" && Publec_Class.ServerIP != "")
    {
        RegisterMsg registermsg = new RegisterMsg();          //创建并引用QQClass类库中的RegisterMsg类
        registermsg.UserName = text_Name.Text;          //记录用户名
        registermsg.PassWord = text_PassWord.Text;           //记录密码
        //通过类库中的SerializeBinary方法，将registermsg序列化为二进制流
        byte[] registerData = new ClassSerializers().SerializeBinary(registermsg).ToArray();
        ClassMsg msg = new ClassMsg();                                   //引用类库中的ClassMsg类
        msg.sendKind = SendKind.SendCommand;               //设置为发送命令
        msg.msgCommand = MsgCommand.Logining;           //将消息命令设为用户登录
        msg.Data = registerData;                                   //将二进制流存入到类库中的二进制变量Data中
        udpSocket1.Send(IPAddress.Parse(Publec_Class.ServerIP), onvert.ToInt32(Publec_Class.ServerPort),
new ClassSerializers().SerializeBinary(msg).ToArray());            //用udpSocket1控件的Send方法将消息发给服务器
        Publec_Class.UserName = text_Name.Text;
    }
}
private void sockUDP1_DataArrival(byte[] Data, System.Net.IPAddress Ip, int Port)
{
    DataArrivaldelegate outdelegate = new DataArrivaldelegate(DataArrival);   //托管
    this.BeginInvoke(outdelegate, new object[] { Data, Ip, Port });                //异步执行托管
}
private void DataArrival(byte[] Data, System.Net.IPAddress Ip, int Port)
{
    try
    {
        ClassMsg msg = new ClassSerializers().DeSerializeBinary((new System.IO.MemoryStream(Data))) as ClassMsg;
        switch (msg.msgCommand)
        {
            case MsgCommand.Logined:                          //登录成功
                Publec_Class.UserID = msg.SID;          //存储当前登录用户在服务器端的ID
                DialogResult = DialogResult.OK;            //设置窗体的对话框结果
                break;
        }
    }
    catch { }
}
private void F_Client_Load(object sender, EventArgs e)
{
    F_Logon FrmLogon = new F_Logon();                          //创建并引用登录窗体
    if (FrmLogon.ShowDialog(this) == DialogResult.OK)      //当登窗体的对话框的返回值为OK时
    {
        FrmLogon.Dispose();                                       //释放登录窗体
        udpSocket1.Active = true;                                 //打开自定义控件udpSocket1
        label1.Text = Publec_Class.UserName;
        GetUserList();                                           //获取服务器端的所有用户信息
    }
    else
    {
        FrmLogon.Dispose();
        Close();                                                    //关闭当前窗体
    }
    Trans(label1, pictureBox1);                                      //使label1控件透明
    Trans(label2, pictureBox1);
}
private void GetUserList()
{
    ClassMsg msg = new ClassMsg();                                          //创建并引用ClassMsg类
    msg.sendKind = SendKind.SendCommand;                              //发送命令
    msg.msgCommand = MsgCommand.UserList;                         //消息命令是获取所有用户信息
    udpSocket1.Send(IPAddress.Parse(Publec_Class.ServerIP), Convert.ToInt32(Publec_Class.ServerPort),
new ClassSerializers().SerializeBinary(msg).ToArray());                   //发送给服务器端
}
private void DataArrival(byte[] Data, System.Net.IPAddress Ip, int Port) //当有数据到达后的处理进程
{
    try
    {
        ClassMsg msg = new ClassSerializers().DeSerializeBinary((new System.IO.MemoryStream(Data))) as
ClassMsg;
        switch (msg.msgCommand)
        {
            case MsgCommand.UserList:                   //获取用户列表
                GetUserList(Data, Ip, Port);
                break;
            case MsgCommand.SendToOne:                     //获取远程客户端所发送的消息
                GetMsg(Data, Ip, Port);
                break;
            case MsgCommand.UpdateState:
                UpdateState(Data, Ip, Port);                  //更新上线或下线的QQ用户
                break;
        }
    }
    catch { }
}
private void GetUserList(byte[] Data, System.Net.IPAddress Ip, int Port)
{
    ClassMsg msg = (ClassMsg)new ClassSerializers().DeSerializeBinary(new MemoryStream(Data));
    //获取所有用户信息
    users = (ClassUsers)new ClassSerializers().DeSerializeBinary(new MemoryStream(msg.Data));
    treeView1.Nodes.Clear();                                                             //清空所有节点 
    for (int i = 0; i < users.Count; i++)                                                               //遍历所有用户
    {
        ClassUserInfo userItem = users[i];
        TreeNode Node = new TreeNode();
        Node.Text = userItem.UserName;                                                         //获取当前用户的名称
        Node.Tag = userItem;
        if (userItem.State == Convert.ToString((int)MsgCommand.Logined))             //当用户为上线状态
        {
            Node.ImageIndex = 1;                                                           //设置上线用户的图片
            Node.SelectedImageIndex = 1;
        }
        else
        {
            Node.ImageIndex = 0;                                                           //设置下线用户的图片
            Node.SelectedImageIndex = 0;
        }
        treeView1.Nodes.Add(Node);
    }
}
private void GetMsg(byte[] Data, System.Net.IPAddress Ip, int Port)
{
    ClassMsg msg = (ClassMsg)new ClassSerializers().DeSerializeBinary(new MemoryStream(Data));
    string sid = msg.SID;                                                                   //发送方用户ID
    string msgid = msg.msgID;                                                           //消息标识，GUID
    switch (msg.sendKind)
    {
        case SendKind.SendMsg:                                                       //发送的消息
            {
                switch (msg.sendState)                                                 //消息发送的状态
                {
                    case SendState.Single:                                              //容量小的单消息
                        {
                            String rtf = Encoding.Unicode.GetString(msg.Data);//获取发送信息
                            Form msgform = null;                            //定义一个窗体
                            for (int i = 0; i < treeView1.Nodes.Count; i++)//在treeView1控件中遍历节点
                            {
                                //如果当前节点的Tag值为发送信息的用户ID
                                if ((treeView1.Nodes[i].Tag as ClassUserInfo).UserID == sid)
                                {
                                    msgform = FindForm(treeView1.Nodes[i]);//查找相关窗体信息
                                    if (msgform != null)
                                    {
                                        //将光标定位到RichTextBox控件的起始位
                                        (msgform as F_Chat).Rich_Out.SelectionStart = 0;
                                        //将发送方的名称添加到RichTextBox控件中
                                        (msgform as F_Chat).Rich_Out.AppendText
 (treeView1.Nodes[i].Tag as ClassUserInfo).UserName);
//将发送时间添加到RichTextBox控件中
(msgform as F_Chat).Rich_Out.AppendText("  " +
ateTime.Now.ToString());
(msgform as F_Chat).Rich_Out.AppendText("\r\n");//回车
(msgform as F_Chat).Rich_Out.SelectedRtf = rtf;//添加发送信息
msgform.Text = "与" + (treeView1.Nodes[i].Tag as
lassUserInfo).UserName + "对话";//设置窗体名称
msgform.Activate();          //激活窗体
                                   }
                                   else
{
    msgform = new F_Chat();       //创建并引用消信发送窗体
    msgform.Tag = treeView1.Nodes[i];//设置远程客户端的ID号
                                     //获取udpSocket1控件的所有信息
    (msgform as F_Chat).udpsocket = udpSocket1;
    FormList.add(msgform);//将当前窗体信息添加到窗体列表中
    (msgform as F_Chat).FormList = FormList;
    (msgform as F_Chat).Rich_Out.SelectionStart = 0;
    (msgform as F_Chat).Rich_Out.AppendText((treeView1.Nodes[i]
.Tag as ClassUserInfo).UserName);
    (msgform as F_Chat).Rich_Out.AppendText("  " +
DateTime.Now.ToString());
    (msgform as F_Chat).Rich_Out.AppendText("\r\n");
    (msgform as F_Chat).Rich_Out.SelectedRtf = rtf;
    msgform.Text = "与" + (treeView1.Nodes[i].Tag as
ClassUserInfo).UserName + "对话";
    msgform.Show();             //显示发送消息窗体
}
                              }
                          }
                          break;
                      }
                  case SendState.Start:                                                   //大容量文件接收的开始消息
                      {
    FileStream fs = null;
    try
    {
        string FileName = "c:\\" + msgid + ".msg";      //设置文件接收路径
        fs = File.Create(FileName);                            //创建文件
        fs.Write(msg.Data, 0, msg.Data.Length);        //向文件中写入接收信息
    }
    finally
    {
        fs.Close();                                           //关闭文件
    }
    break;
}
                   case SendState.Sending:                                             //大容量消息发送中
                       {
    FileStream fs = null;
    try
    {
        string FileName = "c:\\" + msgid + ".msg";     //设置文件接收路径
        fs = File.OpenWrite(FileName);                    //打开当前要写入的文件
        fs.Seek(0, SeekOrigin.End);                  //将该流的当前位值设为0
        fs.Write(msg.Data, 0, msg.Data.Length);        //向文件中写入当前接收的信息
    }
    finally
    {
        fs.Close();
    }
    break;
}
                   case SendState.End:                                                  //大容量消息发送结束
                       {
    FileStream fs = null;
    try
    {
        string FileName = "c:\\" + msgid + ".msg";//文件所在路径
        fs = File.OpenRead(FileName);                   //打开现有文件，以便进行读取
                                                        //将读取的信息存入到二进制流中
        byte[] fsdata = new byte[Convert.ToInt32(fs.Length)];
        //fs.Seek(0, SeekOrigin.Begin);
        //将读取的流写入到缓冲区中
        fs.Read(fsdata, 0, Convert.ToInt32(fs.Length));
        String rtf = Encoding.Unicode.GetString(fsdata);   //将流转换成字符串
        Form msgform = null;
        for (int i = 0; i < treeView1.Nodes.Count; i++)
        {
            if ((treeView1.Nodes[i].Tag as ClassUserInfo).UserID == sid)
            {
                msgform = FindForm(treeView1.Nodes[i]);
                if (msgform != null)
                {
                    (msgform as F_Chat).Rich_Out.SelectionStart = 0;
                    (msgform as F_Chat).Rich_Out.AppendText((
treeView1.Nodes[i].Tag as ClassUserInfo).UserName);
                    (msgform as F_Chat).Rich_Out.AppendText(" 

" + DateTime.Now.ToString());
                    (msgform as F_Chat).Rich_Out.AppendText
("\r\n");
                    (msgform as F_Chat).Rich_Out.SelectedRtf =
rtf;
                    msgform.Text = "与" + (treeView1.Nodes[i].Tag as
ClassUserInfo).UserName + "对话";
                    msgform.Activate();
                }
                else
                {
                    msgform = new F_Chat();
                    msgform.Tag = treeView1.Nodes[i];
                    (msgform as F_Chat).udpsocket = udpSocket1;
                    FormList.add(msgform);
                    (msgform as F_Chat).FormList = FormList;
                    (msgform as F_Chat).Rich_Out.SelectionStart = 0;
                    (msgform as F_Chat).Rich_Out.AppendText((
treeView1.Nodes[i].Tag as ClassUserInfo).UserName);
                    (msgform as F_Chat).Rich_Out.AppendText("  " +
DateTime.Now.ToString());
                    (msgform as F_Chat).Rich_Out.AppendText("\r\n");
                    (msgform as F_Chat).Rich_Out.SelectedRtf = rtf;
                    msgform.Text = "与" + (treeView1.Nodes[i].Tag as
ClassUserInfo).UserName + "对话";
                    msgform.Show();
                }
            }
        }
    }
    finally
    {
        fs.Close();
    }
    break;
}
                }
                break;
            }
        case SendKind.SendFile:
            {
    break;
}
    }
}
private void button_Send_Click(object sender, EventArgs e)
{
    IPAddress ip = IPAddress.Parse(Publec_Class.ServerIP);                 //服务器端的IP地址
    string port = Publec_Class.ServerPort;                                           //端口号
    string revid = ((this.Tag as TreeNode).Tag as ClassUserInfo).UserID;      //接收ID号
    string sid = Publec_Class.UserID;                                                  //发送ID
    string msgid = Guid.NewGuid().ToString();                                    //设置全局惟一标识
    byte[] data = Encoding.Unicode.GetBytes(rich_Input.Rtf);                //将当前要发送的信息转换成二进制流
    ClassMsg msg = new ClassMsg();
    msg.sendKind = SendKind.SendMsg;                                             //发送的消息
    msg.msgCommand = MsgCommand.SendToOne;                           //发送的是单用户信息
    msg.SID = sid;                                                              //发送ID
    msg.RID = revid;                                                           //接收ID
    msg.Data = data;                                                                   //发送的信息
    msg.msgID = msgid;
    if (data.Length <= 1024)                                                        //如果发送信息的长度小于等于1024
    {
        msg.sendState = SendState.Single;
        //将信息直接发送给远程客户端
        udpsocket.Send(ip, Convert.ToInt32(port), new ClassSerializers().SerializeBinary(msg).ToArray());
    }
    else
    {
        ClassMsg start = new ClassMsg();
        start.sendKind = SendKind.SendMsg;
        start.sendState = SendState.Start;                                            //文件发送开始命令
        start.msgCommand = MsgCommand.SendToOne;                    //发送单用户命令
        start.SID = sid;
        start.RID = revid;
        start.Data = Encoding.Unicode.GetBytes("");
        start.msgID = msgid;
        udpsocket.Send(ip, Convert.ToInt32(port), new ClassSerializers().SerializeBinary(start).ToArray());
        MemoryStream stream = new MemoryStream(data);                //将二进制流存储到内存流中
        int sendlen = 1024;                                                        //设置文件每块发送的长度
        long sunlen = (stream.Length);                                               //整个文件的大小
        int offset = 0;                                                          //设置文件发送的起始位置
        while (sunlen > 0)                                                         //分流发送
        {
            sendlen = 1024;
            if (sunlen <= sendlen)
                sendlen = Convert.ToInt32(sunlen);
            byte[] msgdata = new byte[sendlen];                                //创建一个1024大小的二进制流
            stream.Read(msgdata, offset, sendlen);                      //读取要发送的字节块
            msg.sendState = SendState.Sending;                                //发送状态为文件发送中
            msg.Data = msgdata;
            //发送当前块的信息
            udpsocket.Send(ip, Convert.ToInt32(port), new
ClassSerializers().SerializeBinary(msg).ToArray());
            sunlen = sunlen - sendlen;                                               //记录下一块的起始位置
        }
        ClassMsg end = new ClassMsg();
        end.sendKind = SendKind.SendMsg;
        end.sendState = SendState.End;                                              //文件发送结束命令
        end.msgCommand = MsgCommand.SendToOne;
        end.SID = sid;
        end.RID = revid;
        end.Data = Encoding.Unicode.GetBytes("");
        end.msgID = msgid;
        //发送信息，通知文件发送结束
        udpsocket.Send(ip, Convert.ToInt32(port), new ClassSerializers().SerializeBinary(end).ToArray());
    }
    Rich_Out.SelectionStart = 0;                                                  //将文本的起始点设为0
    Rich_Out.AppendText(Publec_Class.UserName);                      //将当前用户名添加到文本框中
    Rich_Out.AppendText("  " + DateTime.Now.ToString());                 //将当前发送的时间添加到文本框中
    Rich_Out.AppendText("\r\n");                                                 //换行回车
    Rich_Out.SelectedRtf = rich_Input.Rtf;                                                 //将发送信息添加到接收文本框中
    rich_Input.Clear();                                                          //清空发送文本框
}
private void tool_Video_Click(object sender, EventArgs e)
{
    //创建并引用cVideo类
    viodeo = new cVideo(pictureBox1.Handle, pictureBox1.Width, pictureBox1.Height);
    viodeo.StartWebCam();                                                   //打开视频设备
    ClassMsg msg = new ClassMsg();                                                 //创建并引用ClassMsg类
    msg.sendKind = SendKind.SendCommand;
    msg.msgCommand = MsgCommand.VideoOpen;                      //打开视频
    msg.Data = Encoding.Unicode.GetBytes("");                                          //发送一个空消息
    IPAddress ip = IPAddress.Parse(((this.Tag as TreeNode).Tag as ClassUserInfo).UserIP);//IP地址
    string port = "11005";                                                            //端口号
    //将消息发送给远程计算机
    udpsocket.Send(ip, Convert.ToInt32(port), new ClassSerializers().SerializeBinary(msg).ToArray());
    Voiding = true;
    SendViod = false;
    timer1.Enabled = true;                                                            //运行timer1控件的计时器
    panel2.Visible = true;                                                              //显示panel2控件
}
private void timer1_Tick(object sender, EventArgs e)
{
    if (Voiding || !SendViod || pictureBox1.Image != null)
    {
        SendViod = true;                                                    //视频开启或关闭的标识
                                                                            //开始发送
        IPAddress ip = IPAddress.Parse(((this.Tag as TreeNode).Tag as ClassUserInfo).UserIP);
        string port = "11005";
        string revid = ((this.Tag as TreeNode).Tag as ClassUserInfo).UserID;//远程用户ID
        string sid = Publec_Class.UserID;                                         //当前用户ID
        ClassMsg msg = new ClassMsg();                                        //创建并引用ClassMsg类
        msg.sendKind = SendKind.SendCommand;                            //消息发送的类型
        msg.msgCommand = MsgCommand.Videoing;                       //发送视频中
        msg.sendState = SendState.Sending;                                      //文件正在发送中
        msg.SID = sid;                                                             //获取当前用户的ID
        msg.RID = revid;                                                  //获取远程用户的ID
        msg.Data = Encoding.Unicode.GetBytes("");                          //定义一个空的二进制流
        ClassMsg start = new ClassMsg();
        start.sendKind = SendKind.SendCommand;
        start.msgCommand = MsgCommand.Videoing;
        start.sendState = SendState.Start;
        start.SID = sid;
        start.RID = revid;
        start.Data = Encoding.Unicode.GetBytes("");
        //将消息发送给远程客户端
        udpsocket.Send(ip, Convert.ToInt32(port), new ClassSerializers().SerializeBinary(start).ToArray());
        //将pictureBox1控件中的图片存入到指定路径中
        viodeo.GrabImage(pictureBox1.Handle, "C:\\TempVoid.Bmp");
        FileStream stream = File.OpenRead("C:\\TempVoid.Bmp");              //打开已有的文件，以便读取
                                                                            //下面是对stream中的文件进行分流发送
        int sendlen = 1024;                                                       //设置读取块的大小
        long sunlen = (stream.Length);                                              //获取文件的总大小
        int offset = 0;                                                        //定义读取文件的起始位置
        while (sunlen > 0)
        {
            if (sunlen <= sendlen)                                                   //当文件大小小于等于1024时
                sendlen = Convert.ToInt32(sunlen);                         //将长整型转换成整型
            byte[] msgdata = new byte[sendlen];                       //将文件转换成二进制流
            stream.Read(msgdata, offset, sendlen);                           //从流中读取字节块，并写入到缓存区中
            msg.sendState = SendState.Sending;                        //文件正在发送命令
                                                                      //将读取的内存流写入到类库中所定义的二进制变量中，以便向远程客户端发送
            msg.Data = msgdata;
            //向远程客户端发送信息
            udpsocket.Send(ip, Convert.ToInt32(port), new ClassSerializers().SerializeBinary(msg).ToArray());
            sunlen = sunlen - sendlen;                                              //从流的长度中减去已发送的字节块
        }
        ClassMsg end = new ClassMsg();                                         //创建并引用ClassMsg类
        end.sendKind = SendKind.SendCommand;                      //发送的消息类型
        end.sendState = SendState.End;                                            //文件发送结束命令
        end.msgCommand = MsgCommand.Videoing;                        //正在视频中
        end.SID = sid;                                                              //向类中存储当前用户的ID
        end.RID = revid;                                                   //向类中存储远程用户的ID
        end.Data = Encoding.Unicode.GetBytes("");                           //向类中存储一个空的二进制流
        stream.Close();                                                             //关闭当前流
                                                                                    //向远程客户端发送消息
        udpsocket.Send(ip, Convert.ToInt32(port), new ClassSerializers().SerializeBinary(end).ToArray());
        SendViod = false;
    }
}
private void DataArrival(byte[] Data, System.Net.IPAddress Ip, int Port) //当有数据到达后的处理进程
{
    try
    {
        ClassMsg msg = new ClassSerializers().DeSerializeBinary((new System.IO.MemoryStream(Data))) as
ClassMsg;
        switch (msg.msgCommand)
        {
            case MsgCommand.VideoOpen:    //打开视频
                Voiding = true;
                SendViod = false;
                timer1.Enabled = true;
                panel2.Visible = true;
                break;
            case MsgCommand.Videoing:                                                 //正在视频中
                GetVoid(Data, Ip, Port);
                break;
            case MsgCommand.VideoClose:                                              //关闭视频
                Voiding = false;
                break;
        }
    }
    catch { }
}
private void GetVoid(byte[] Data, System.Net.IPAddress Ip, int Port)
{
    ClassMsg msg = (ClassMsg)new ClassSerializers().DeSerializeBinary(new MemoryStream(Data));
    string sid = msg.SID;                                                                   //发送方用户ID
    string msgid = msg.msgID;                                                           //消息标识，GUID
    switch (msg.msgCommand)
    {
        case MsgCommand.Videoing:                                                        //正在视频中
            {
                switch (msg.sendState)                                                 //消息发送的状态
                {
                    case SendState.Start:                                               //开始发送文件
                        {
                            FileStream fs = null;
                            try
                            {
                                string FileName = "c:\\Void.bmp";         //设置文件接收路径
                                fs = File.Create(FileName);                  //创建文件
                                fs.Write(msg.Data, 0, msg.Data.Length);     //向文件中写入接收信息
                            }
                            finally
                            {
                                fs.Close();//关闭文件
                            }
                            break;
                        }
                    case SendState.Sending:                                         //正在发送文件
                        {
                            FileStream fs = null;
                            try
                            {
                                string FileName = "c:\\Void.bmp";         //设置文件接收路径
                                fs = File.OpenWrite(FileName);            //打开当前要写入的文件
                                fs.Seek(0, SeekOrigin.End);                //将该流的当前位值设为0
                                fs.Write(msg.Data, 0, msg.Data.Length);     //向文件中写入当前接收信息
                            }
                            finally
                            {
                                fs.Close();
                            }
                            break;
                        }
                    case SendState.End:                                   //文件发送结束
                        {
                            pictureBox2.Load("c:\\Void.bmp");
                            break;
                        }
                }
            }
            break;
    }
}
private void Tool_Open(object sender, EventArgs e)
{
    if (((ToolStripMenuItem)sender).Text == "开始服务")
    {
        ((ToolStripMenuItem)sender).Text = "结束服务";              //设置菜单项的文本信息
        udpSocket1.Active = true;                                        //打开监听
    }
    else
    {
        ((ToolStripMenuItem)sender).Text = "开始服务";
        udpSocket1.Active = false;                                       //关闭监听
    }
}
private void DataArrival(byte[] Data, System.Net.IPAddress Ip, int Port)
{
    try
    {
        ClassMsg msg = new ClassSerializers().DeSerializeBinary((new System.IO.MemoryStream(Data))) as
ClassMsg;                                                                     //获取当前所接收的消息
        switch (msg.sendKind)
        {
            case SendKind.SendCommand:                                 //命令
                {
                    switch (msg.msgCommand)
                    {
                        case MsgCommand.Registering:           //注册用户
                            RegisterUser(msg, Ip, Port);
                            break;
                        case MsgCommand.Logining:              //登录用户
                            UserLogin(msg, Ip, Port, 1);
                            break;
                        case MsgCommand.UserList:        //用户列表
                            SendUserList(msg, Ip, Port);
                            break;
                        case MsgCommand.SendToOne:          //发送消息给单用户
                            SendUserMsg(msg, Ip, Port);
                            break;
                        case MsgCommand.Close:                   //下线
                            UpdateUserState(msg, Ip, Port);
                            break;
                    }
                    break;
                }
            case SendKind.SendMsg:                                         //消息
                {
                    switch (msg.msgCommand)
                    {
                        case MsgCommand.SendToOne:          //发送消息给单用户
                            SendUserMsg(msg, Ip, Port);
                            break;
                    }
                    break;
                }
            case SendKind.SendFile:                                    //文件
                break;
        }
    }
    catch (Exception e)
    {
        MessageBox.Show(e.Message);
    }
}
private void RegisterUser(ClassMsg msg, System.Net.IPAddress Ip, int Port)
{
    msg = InsertUser(msg, Ip, Port);                                       //将注册信息添加到数据库中
    UpdateUserList(msg, Ip, Port);                                        //更新用户列表
}
private ClassMsg InsertUser(ClassMsg msg, System.Net.IPAddress Ip, int Port)
{
    RegisterMsg registermsg = (RegisterMsg)new ClassSerializers().DeSerializeBinary(new
MemoryStream(msg.Data));
    ClassOptionData OptionData = new ClassOptionData();
    MsgCommand Sate = msg.msgCommand;
    String UserName = registermsg.UserName;                                    //注册用户的名称
    String PassWord = registermsg.PassWord;                               //注册用户的密码
    String vIP = Ip.ToString();                                              //注册用户的IP地址
    //向数据表中添加注册信息
    OptionData.ExSQL("insert into tb_CurreneyUser (IP,Port,Name,PassWord,Sign) values ('" + vIP + "'," +
Port.ToString() + ",'" + UserName + "','" + PassWord + "'," + Convert.ToString((int)(MsgCommand.Registered))
 + ")");
    SqlDataReader DataReader = OptionData.ExSQLReDr("Select * From tb_CurreneyUser");
    UpdateUser();                                                         //更新用户列表
    OptionData.Dispose();
    msg.msgCommand = MsgCommand.Registered;                    //用户注册结束命令
    SendMsgToOne(Ip, Port, msg);                                         //将注册命令返回给注册用户
    return msg;
}
private void UpdateUserList(ClassMsg msg, System.Net.IPAddress Ip, int Port)
{
    ClassUsers Users = new ClassUsers();
    ClassOptionData OptionData = new ClassOptionData();
    //查找所有注册的用户信息
    SqlDataReader DataReader = OptionData.ExSQLReDr("Select * From tb_CurreneyUser");
    while (DataReader.Read())                                                      //遍历所有用户
    {
        ClassUserInfo UserItem = new ClassUserInfo();                       //创建并引用ClassUserInfo类
        UserItem.UserID = Convert.ToString(DataReader.GetInt32(0)); //记录用户编号
        UserItem.UserIP = DataReader.GetString(1);                            //记录用户的IP地址
        UserItem.UserPort = Convert.ToString(DataReader.GetInt32(2));      //记录端口号
        UserItem.UserName = DataReader.GetString(3);                       //记录用户名称
        UserItem.State = Convert.ToString(DataReader.GetInt32(5));           //记录当前状态
        Users.add(UserItem);                                                      //将单用户信息添加到用户列表中
    }
    msg.Data = new ClassSerializers().SerializeBinary(Users).ToArray(); //将用户列表写入到二进制流中
    //查找当前已上线的用户
    DataReader = OptionData.ExSQLReDr("Select * From tb_CurreneyUser Where Sign = " +
                MsgCommand.Logined);
    while (DataReader.Read())                                                      //向所有上线用户发送用户列表
    {
        udpSocket1.Send(IPAddress.Parse(DataReader.GetString(1)), DataReader.GetInt32(2), new
            ClassSerializers().SerializeBinary(msg).ToArray());
    }
    OptionData.Dispose();
}
private void UserLogin(ClassMsg msg, System.Net.IPAddress Ip, int Port, int State)
{
    RegisterMsg registermsg = (RegisterMsg)new ClassSerializers().DeSerializeBinary(new
MemoryStream(msg.Data));
    ClassOptionData OptionData = new ClassOptionData();           //创建并引用ClassOptionData
    MsgCommand msgState = msg.msgCommand;                    //获取接收消息的命令
    String UserName = registermsg.UserName;                                   //登录用户名称
    String PassWord = registermsg.PassWord;                             //用户密码
    String vIP = Ip.ToString();                                            //用户IP地址
    SqlDataReader DataReader = OptionData.ExSQLReDr("Select * From tb_CurreneyUser Where Name = 
"+ "'"+UserName+"'"+" and PassWord = "+"'"+PassWord+"'");          //在数据库中通过用户名和密码进行查找

    DataReader.Read();                                                       //读取查找到的记录
    string ID = Convert.ToString(DataReader.GetInt32(0));           //获取第一条记录中的ID字段值
    if (DataReader.HasRows)                                              //当DataReader中有记录信息时
    {
        //修改当前记录的标识为上线状态
        OptionData.ExSQL("Update tb_CurreneyUser Set Sign = " + Convert.ToString
((int)(MsgCommand.Logined)) + ",IP = " + "'" + vIP + "',Port = " + "'" + Port.ToString() +
"'" + " Where ID = " + ID);
        msg.msgCommand = MsgCommand.Logined;                //设置为上线命令
        msg.SID = ID;                                                 //用户ID值
        SendMsgToOne(Ip, Port, msg);                                       //将消息返回给发送用户
        UpdateUserState(msg, Ip, Port);                                        //更新用户的在线状态
    }
    OptionData.Dispose();
    UpdateUser();                                                         //更新用户列表
}