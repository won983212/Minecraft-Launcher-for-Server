using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft_Launcher_for_Server
{
    public enum RetrieveState
    {
        Processing, Loaded, Error
    }

    public class ConnectionState
    {
        public RetrieveState State { get; set; }
        public string ErrorMessage { get; set; } = null;
    }
}
