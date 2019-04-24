using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntivirusLibrary.Events
{
    public class FileCheckEventArgs: EventArgs
    {
        public bool State { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="state">True для увеличения, false для уменьшения</param>
        public FileCheckEventArgs(bool state)
        {
            State = state
        }
    }
}
