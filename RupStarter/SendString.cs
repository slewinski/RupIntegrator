using System;
using System.Collections.Generic;
using System.Text;using System.Runtime.InteropServices;

namespace RupStarter
{
     public class SendString
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool PostMessage(IntPtr hWnd, int Msg, IntPtr wParam,
          IntPtr lParam);
        const int WM_CHAR = 0x0102;


        private void button1_Click(object sender, EventArgs e)
        {
            IntPtr window = FindWindow(null, "Form2"); //Find the handler of Form2
            if (window != IntPtr.Zero)//If found
            {
                {
             /*   IntPtr tb = FindWindowEx(window, IntPtr.Zero, "WindowsForms10.EDIT.app.0.161e476_r29_ad17", null); //Find the handler of TextBox
                foreach (char c in textBox1.Text.ToCharArray()) //WindowsForms10.EDIT.app.0.161e476_r29_ad16    zlecemnuodawca 15 - Iban
                    WindowsForms10.EDIT.app.0.161e476_r29_ad1
                        WindowsForms10.EDIT.app.0.161e476_r29_ad1
                    PostMessage(tb, WM_CHAR, new IntPtr(c), IntPtr.Zero); //Send the chars one by one
              * */
                }
            }
        }

    }
}



        