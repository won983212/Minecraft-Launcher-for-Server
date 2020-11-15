using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ResourceServerUtils
{
    class Program
    {
        void ShowHelp()
        {
            Console.WriteLine("remap <src> <dst> - src폴더안에 리소스파일들의 hash를 업데이트한다. dst폴더에 변경된 리소스 파일과 hash정보가 저장된다.");
        }

        void Remapping(string src, string dst)
        {

        }

        void Run(string[] args)
        {
            if (args.Length > 1)
            {
                if (args[1] == "remap")
                {
                    if (args.Length == 4)
                    {
                        Remapping(args[2], args[3]);
                        return;
                    }
                }
            }
            ShowHelp();
            /*using (var sha1 = new SHA1CryptoServiceProvider())
            {
                byte[] data = sha1.ComputeHash(new FileStream(@"C:\Users\psvm\Desktop\file", FileMode.Open));
                Console.WriteLine(BitConverter.ToString(data));
            }*/
        }

        static void Main(string[] args)
        {
            //new Program().Run(args);
            new Program().Run(new string[] { "", "remap", @"C:\Users\psvm\Desktop\node\server\sourcefiles", @"C:\Users\psvm\Desktop\node\server\resources" });
        }
    }
}
