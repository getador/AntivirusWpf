using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntivirusLibrary
{
    public class EvrizmSignature
    {
        public static readonly List<string> signatures = new List<string>()
        {
            "CreateRemoteThread",
            "RijndaelManaged",
            "NtUnmapViewOfSection",
            "JOIN",
            "SetWindowsHookEx",
            "GetForegroundWindow",
            "MD5CryptoServiceProvider",
            "GetAsyncKeyState",
            "GetWindowText",
            "PRIVMSG"
        };
    }
}
