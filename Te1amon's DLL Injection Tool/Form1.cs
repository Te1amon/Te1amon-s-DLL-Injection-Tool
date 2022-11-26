using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Te1amon_s_DLL_Injection_Tool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string dllpathfunny = string.Empty;


        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool WaitNamedPipe(string pipe, int timeout = 10);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int CloseHandle(IntPtr hObject);

        // Token: 0x06000013 RID: 19
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttribute, IntPtr dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        // Token: 0x06000014 RID: 20
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        // Token: 0x06000015 RID: 21
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        // Token: 0x06000017 RID: 23
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, int bInheritHandle, uint dwProcessId);

        // Token: 0x06000018 RID: 24
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flAllocationType, uint flProtect);

        // Token: 0x06000019 RID: 25
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] buffer, uint size, int lpNumberOfBytesWritten);

        // Token: 0x04000007 RID: 7
        private static readonly IntPtr INTPTR_ZERO = (IntPtr)0;
        private bool bInject(uint pToBeInjected, string sDllPath)
        {
            IntPtr intPtr = OpenProcess(1082U, 1, pToBeInjected);
            bool flag = intPtr == INTPTR_ZERO;
            bool result;
            if (flag)
            {
                result = false;
            }
            else
            {
                IntPtr procAddress = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
                bool flag2 = procAddress == INTPTR_ZERO;
                if (flag2)
                {
                    result = false;
                }
                else
                {
                    IntPtr intPtr2 = VirtualAllocEx(intPtr, (IntPtr)0, (IntPtr)sDllPath.Length, 12288U, 64U);
                    bool flag3 = intPtr2 == INTPTR_ZERO;
                    if (flag3)
                    {
                        result = false;
                    }
                    else
                    {
                        byte[] bytes = Encoding.ASCII.GetBytes(sDllPath);
                        bool flag4 = WriteProcessMemory(intPtr, intPtr2, bytes, (uint)bytes.Length, 0) == 0;
                        if (flag4)
                        {
                            result = false;
                        }
                        else
                        {
                            bool flag5 = CreateRemoteThread(intPtr, (IntPtr)0, INTPTR_ZERO, procAddress, intPtr2, 0U, (IntPtr)0) == INTPTR_ZERO;
                            if (flag5)
                            {
                                result = false;
                            }
                            else
                            {
                                CloseHandle(intPtr);
                                result = true;
                            }
                        }
                    }
                }
            }
            return result;
        }

        public enum DllInjectionResult
        {
            // Token: 0x04000003 RID: 3
            DllNotFound,
            // Token: 0x04000004 RID: 4
            GameProcessNotFound,
            // Token: 0x04000005 RID: 5
            InjectionFailed,
            // Token: 0x04000006 RID: 6
            Success
        }

        public static DllInjector GetInstance
        {
            get
            {
                bool flag = DllInjector._instance == null;
                if (flag)
                {
                    DllInjector._instance = new DllInjector();
                }
                return DllInjector._instance;
            }
        }


        public class DllInjector
        {

            // Token: 0x04000008 RID: 8
            public static DllInjector _instance;

        }

        public DllInjectionResult Injectmabob(string sProcName, string sDllPath)
        {
            bool flag = !File.Exists(sDllPath);
            DllInjectionResult result;
            if (flag)
            {
                result = DllInjectionResult.DllNotFound;
            }
            else
            {
                uint num = 0U;
                Process[] processes = Process.GetProcesses();
                for (int i = 0; i < processes.Length; i++)
                {
                    bool flag2 = !(processes[i].ProcessName != sProcName);
                    if (flag2)
                    {
                        num = (uint)processes[i].Id;
                        break;
                    }
                }
                bool flag3 = num == 0U;
                if (flag3)
                {
                    result = DllInjectionResult.GameProcessNotFound;
                }
                else
                {
                    bool flag4 = !bInject(num, sDllPath);
                    if (flag4)
                    {
                        result = DllInjectionResult.InjectionFailed;
                        MessageBox.Show("An error has occured while injecting.", "Te1amon's DLL Injection Tool");
                    }
                    else
                    {
                        result = DllInjectionResult.Success;
                    }
                }
            }
            return result;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                openFileDialog1.Title = "Noobsploit";
                openFileDialog1.Filter = "Dynamic Libraries (*.dll)|*.dll";
                string thingmabob = openFileDialog1.FileName;
                dllpathfunny = thingmabob;
                button1.Text = "Select DLL (Selected)";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dllpathfunny == string.Empty)
            {
                MessageBox.Show("Choose a DLL using the Select DLL button", "Te1amon's DLL Injection Tool");
            }
            else
            {
                Injectmabob(textBox1.Text, dllpathfunny);
            }
        }
    }
}
