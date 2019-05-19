using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace AntivirusLibrary
{
    public class FileValidater
    {
        public static bool VerifyAuthenticodeSignature(string path)
        {
            string fullPath = Path.GetFullPath(path);

            if (!File.Exists(fullPath))
                return false;

            using (var ps = PowerShell.Create())
            {
                ps.AddCommand("Get-AuthenticodeSignature", true);
                ps.AddParameter("LiteralPath", fullPath);
                var results = ps.Invoke();

                var signature = (Signature)results.Single().BaseObject;
                return (signature.Status == SignatureStatus.Valid);
            }
        }
    }
}
