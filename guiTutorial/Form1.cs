using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;

namespace guiTutorial
{

    public partial class Form1 : Form
    {

        [System.Runtime.InteropServices.DllImport("User32", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, IntPtr lParam);

        [System.Runtime.InteropServices.DllImport("User32.dll")]

        private static extern IntPtr FindWindowEx(IntPtr hWnd1, int hWnd2, string lp1, string lp2);

        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc callback, IntPtr hInstance, uint threadId);
        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);
        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);
        [DllImport("kernel32.dll")] static extern IntPtr LoadLibrary(string lpFileName);
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        const int WH_KEYBOARD_LL = 13;
        const int WM_KEYDOWN = 0x100;
        private LowLevelKeyboardProc _proc = hookProc;
        private static IntPtr hhook = IntPtr.Zero;
        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;      // 좌클릭 누름
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;        // 좌클릭 뗌
        const uint MOUSEEVENTF_RBUTTONDOWN = 0x0008;    // 오른쪽 마우스 버튼 눌림
        const uint MOUSEEVENTF_RBUTTONUP = 0x00010;      // 오른쪽 마우스 버튼 떼어짐
        public static Boolean leftPower = false;

        String AppPlayerName = "Minecraft 1.16.5 - 멀티플레이 (제삼자 서버)";

        private System.Windows.Forms.Timer scanloopTimer;

        public static System.Windows.Forms.Timer FixToolTimer = new System.Windows.Forms.Timer();

        private static System.Windows.Forms.Timer FastClickTimer;

        public static int count = 0;

        public static int FragmentCount = 0;
        // 돌 조각으로 변환시킬 카운트

        public String fixCommnad;

        public int ClickSpeed;

        public static Boolean FastClickPower = false;

        public static Boolean FragmentAutoPower = false;

        public int MiningLimit = 200;

        public void SetHook() {
            IntPtr hInstance = LoadLibrary("User32");
            hhook = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, hInstance, 0);
        }
        public static void UnHook() { 
            UnhookWindowsHookEx(hhook); 
        }

      
        
        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            UnHook();
        }
        
    
        
        
        

        public Form1()
        {
            InitializeComponent();
            this.Text = "마인크래프트 매크로";
        }
        bool isRunning = true;

        private void button1_Click(object sender, EventArgs e)
        {
            fixCommnad = fixCommand_Text.Text;
            MiningLimit = Int32.Parse(MiningLimit_Text.Text);
            MessageBox.Show("설정이 적용되었습니다.");
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void scanLoop(object sender, EventArgs e)
        {


            IntPtr findwindow = FindWindow(null, AppPlayerName);

            IntPtr fwEx = FindWindowEx(findwindow, 0, "GLFW30", "Minecraft 1.16.5 - 멀티플레이 (제삼자 서버)");
            //1: findwindow 2: 0 3: 클래스명 4: 캡션명

            if (findwindow != IntPtr.Zero)
            {

                //플레이어를 찾았을 경우
                Debug.WriteLine("설정한 앱을 찾았습니다.");

                try
                {

                    //찾은 플레이어를 바탕으로 Graphics 정보를 가져옵니다.
                    Graphics Graphicsdata = Graphics.FromHwnd(findwindow);

                    //찾은 플레이어 창 크기 및 위치를 가져옵니다. 
                    Rectangle rect = Rectangle.Round(Graphicsdata.VisibleClipBounds);

                    //플레이어 창 크기 만큼의 비트맵을 선언해줍니다.
                    Bitmap bmp = new Bitmap(rect.Width, rect.Height);

                    //비트맵을 바탕으로 그래픽스 함수로 선언해줍니다.
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        //찾은 플레이어의 크기만큼 화면을 캡쳐합니다.
                        IntPtr hdc = g.GetHdc();
                        PrintWindow(fwEx, hdc, 0x2);
                        g.ReleaseHdc(hdc);
                    }

                    // pictureBox1 이미지를 표시해줍니다.
                    pictureBox1.Image = bmp;
                }
                catch (Exception)
                {
                    Debug.WriteLine("오류");
                }
            }
            else
            {
                //마크를 못찾을경우
                Debug.WriteLine("설정한 앱을 찾지 못했습니다.");
            }
        }

        private async void Run()
        {
            InputSimulator s = new InputSimulator();
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            count = 0;
            Debug.WriteLine("수리가필요함");
            s.Keyboard.KeyDown(VirtualKeyCode.RETURN);
            Thread.Sleep(200);
            Debug.WriteLine("엔터누름");
            SendKeys.Send(fixCommnad);
            Thread.Sleep(200);
            s.Keyboard.KeyDown(VirtualKeyCode.RETURN);
            //SendKeys.Send("{ENTER}");
            Thread.Sleep(200);
            Debug.WriteLine("돌조각 자동생성: " + FragmentAutoPower);
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);

            if (FragmentAutoPower == true)
            {
                Thread.Sleep(100);
                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                s.Keyboard.KeyDown(VirtualKeyCode.VK_9);
                Thread.Sleep(200);
                mouse_event(MOUSEEVENTF_RBUTTONDOWN, 0, 0, 0, 0);
                mouse_event(MOUSEEVENTF_RBUTTONUP, 0, 0, 0, 0);
                Thread.Sleep(200);
                mouse_event(MOUSEEVENTF_RBUTTONDOWN, 0, 0, 0, 0);
                mouse_event(MOUSEEVENTF_RBUTTONUP, 0, 0, 0, 0);
                Thread.Sleep(200);
                mouse_event(MOUSEEVENTF_RBUTTONDOWN, 0, 0, 0, 0);
                mouse_event(MOUSEEVENTF_RBUTTONUP, 0, 0, 0, 0);
                Thread.Sleep(200);
                mouse_event(MOUSEEVENTF_RBUTTONDOWN, 0, 0, 0, 0);
                mouse_event(MOUSEEVENTF_RBUTTONUP, 0, 0, 0, 0);
                Thread.Sleep(200);
                mouse_event(MOUSEEVENTF_RBUTTONDOWN, 0, 0, 0, 0);
                mouse_event(MOUSEEVENTF_RBUTTONUP, 0, 0, 0, 0);
                Thread.Sleep(200);
                mouse_event(MOUSEEVENTF_RBUTTONDOWN, 0, 0, 0, 0);
                mouse_event(MOUSEEVENTF_RBUTTONUP, 0, 0, 0, 0);
                Thread.Sleep(200);
                mouse_event(MOUSEEVENTF_RBUTTONDOWN, 0, 0, 0, 0);
                mouse_event(MOUSEEVENTF_RBUTTONUP, 0, 0, 0, 0);
                Thread.Sleep(200);
                //조약돌을 돌조각으로 바꿔주기 위하여 우클 연타

                s.Keyboard.KeyDown(VirtualKeyCode.VK_1);

                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            }
        }

        private void FixTool_Tick(object sender, EventArgs e)
        {
            Debug.WriteLine("타이머 진입");
                count = count + 1;
            if (FragmentAutoPower == true)
            {
                FragmentCount = FragmentCount + 1;
            }
            else
            {
                FragmentCount = 0;
            }
            Debug.WriteLine("카운트: " + count.ToString());
            if (count == MiningLimit) {
                Run();
            }

            if(leftPower == false)
            {
                FixToolTimer.Stop();
                count = 0;
                FragmentCount = 0;
            }
        }

        private void FastClick_Tick(object sender, EventArgs e)
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            if(FastClickPower == false)
            {
                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                FastClickTimer.Stop();
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("이 프로그램은 마인크래프트에서");
            sb.AppendLine("자동으로 광질을 하거나,");
            sb.AppendLine("좌클릭을 빠르게 하고 싶을때");
            sb.AppendLine("사용하는 프로그램 입니다.");

            FragmentCheckBox.Checked = false;

            label3.Text = sb.ToString();


            SetHook();

            scanloopTimer = new System.Windows.Forms.Timer();
            scanloopTimer.Interval = 50;
            scanloopTimer.Tick += new EventHandler(scanLoop);

            FixToolTimer.Interval = 1000; //주기 설정
            FixToolTimer.Tick += new EventHandler(FixTool_Tick); //주기마다 실행되는 이벤트 등록

            FastClickTimer = new System.Windows.Forms.Timer();
            FastClickTimer.Interval = 10;
            FastClickTimer.Tick += new EventHandler(FastClick_Tick);

            IntPtr hWnd = FindWindow(null, AppPlayerName);
            if(!hWnd.Equals(IntPtr.Zero))
            {
                Debug.WriteLine("윈도우 핸들 : " + hWnd);
            }
        }

        public static IntPtr hookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                if (vkCode.ToString() == "119")
                {
                    try
                    {
                        if (leftPower == false)
                        {
                            leftPower = true;
                            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                            FixToolTimer.Start();
                        }
                        else
                        {
                            leftPower = false;
                            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                        }
                    }
                    catch (Exception)
                    {
                        return (IntPtr)MessageBox.Show("에러발생");
                    }
                    //Cursor.Position = new Point(242, 83);
                }

                if (vkCode.ToString() == "120")
                {
                    if(FastClickPower == false)
                    {
                        FastClickPower = true;
                        FastClickTimer.Start();
                    }
                    else
                    {
                        FastClickPower = false;
                    }
                }

                    //MessageBox.Show(vkCode.ToString()); 
                    return CallNextHookEx(hhook, code, (int)wParam, lParam);
            }
            else
                return CallNextHookEx(hhook, code, (int)wParam, lParam);
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void runNotice_Click(object sender, EventArgs e)
        {
            
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void scanBtn_Click(object sender, EventArgs e)
        {
            scanloopTimer.Start();

            IntPtr findwindow = FindWindow(null, AppPlayerName);

            IntPtr fwEx = FindWindowEx(findwindow, 0, "GLFW30", "Minecraft 1.16.5 - 멀티플레이 (제삼자 서버)");
            //1: findwindow 2: 0 3: 클래스명 4: 캡션명

            if (findwindow != IntPtr.Zero)
            {

                //플레이어를 찾았을 경우
                Debug.WriteLine("설정한 앱을 찾았습니다.");

                try
                {

                    //찾은 플레이어를 바탕으로 Graphics 정보를 가져옵니다.
                    Graphics Graphicsdata = Graphics.FromHwnd(findwindow);

                    //찾은 플레이어 창 크기 및 위치를 가져옵니다. 
                    Rectangle rect = Rectangle.Round(Graphicsdata.VisibleClipBounds);

                    //플레이어 창 크기 만큼의 비트맵을 선언해줍니다.
                    Bitmap bmp = new Bitmap(rect.Width, rect.Height);

                    //비트맵을 바탕으로 그래픽스 함수로 선언해줍니다.
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        //찾은 플레이어의 크기만큼 화면을 캡쳐합니다.
                        IntPtr hdc = g.GetHdc();
                        PrintWindow(fwEx, hdc, 0x2);
                        g.ReleaseHdc(hdc);
                    }

                    // pictureBox1 이미지를 표시해줍니다.
                    pictureBox1.Image = bmp;
                }
                catch (Exception)
                {
                    Debug.WriteLine("오류");
                }
            }
            else
            {
                //마크를 못찾을경우
                Debug.WriteLine("설정한 앱을 찾지 못했습니다.");
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(FragmentCheckBox.Checked == false)
            {
                FragmentAutoPower = false;
            }
            else
            {
                FragmentAutoPower = true;
            }

        }

        private void splitContainer1_Panel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void label4_Click_1(object sender, EventArgs e)
        {

        }
    }
}
