using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft_Launcher_for_Server
{
    // TODO 향후 log / debug를 따로 콘솔창 만들어서 사용
    public static class Logger
    {
        public static void Log(object data)
        {
            Console.WriteLine(data.ToString());
        }

        public static void Debug(object data)
        {
#if DEBUG
            Console.WriteLine(data.ToString());
#endif
        }
    }
}
