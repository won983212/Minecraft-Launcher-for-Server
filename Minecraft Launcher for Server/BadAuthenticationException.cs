using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft_Launcher_for_Server
{
    public class BadAuthenticationException : Exception
    {
        public string ErrorType { get; private set; }
        public string ErrorMessage { get; private set; }

        public BadAuthenticationException(string error, string message)
            : base(error + ": " + message)
        {
            ErrorType = error;
            ErrorMessage = Filtering(message);
        }

        private static string Filtering(string message)
        {
            if (message.StartsWith("Invalid credentials. Account"))
            {
                return "계정이 이전되었습니다. 아이디 대신에 이메일을 사용해서 로그인해주세요.";
            }
            else if (message.StartsWith("Invalid credentials. Invalid"))
            {
                return "이메일 또는 비밀번호가 틀렸습니다.";
            }
            else if (message.StartsWith("Invalid credentials."))
            {
                return "로그인 시도를 너무 많이 했습니다.";
            }
            else if (message.StartsWith("Invalid token."))
            {
                return "로그인이 만료되었습니다.";
            }
            else
            {
                return message;
            }
        }
    }
}
