using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
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
            else if(message.StartsWith("Invalid token."))
            {
                return "로그인이 만료되었습니다.";
            }
            else
            {
                return message;
            }
        }
    }

    public class AuthResponse
    {
        public string Username { get; private set; }
        public string AccessToken { get; private set; }
        public string ClientToken { get; private set; }

        internal static AuthResponse CreateFromJson(JObject obj)
        {
            AuthResponse ret = new AuthResponse
            {
                Username = (string)obj["selectedProfile"]["name"],
                AccessToken = (string)obj["accessToken"],
                ClientToken = (string)obj["clientToken"]
            };
            return ret;
        }
    }

    public static class MinecraftAPI
    {
        public static AuthResponse DoAuth(string username, string password)
        {
            string data = "{ \"agent\": { \"name\": \"Minecraft\", \"version\": 1 }, \"username\": \"" + username + "\", \"password\": \"" + password + "\" }";
            return AuthResponse.CreateFromJson(SendPostRequest("https://authserver.mojang.com/authenticate", data));
        }

        public static AuthResponse RefreshAccessToken(string accessToken, string clientToken)
        {
            string data = "{ \"accessToken\": \"" + accessToken + "\", \"clientToken\": \"" + clientToken + "\" }";
            return AuthResponse.CreateFromJson(SendPostRequest("https://authserver.mojang.com/refresh", data));
        }

        public static bool ValidateAccessToken(string accessToken)
        {
            string data = "{ \"accessToken\": \"" + accessToken + "\" }";
            bool ret;

            try
            {
                SendPostRequest("https://authserver.mojang.com/validate", data);
                ret = true;
            }
            catch (BadAuthenticationException)
            {
                ret = false;
            }

            return ret;
        }

        public static void InvalidateAccessToken(string accessToken, string clientToken)
        {
            string data = "{ \"accessToken\": \"" + accessToken + "\", \"clientToken\": \"" + clientToken + "\" }";
            SendPostRequest("https://authserver.mojang.com/invalidate", data);
        }

        public static string GetHeadImageURL(string identifier, int width)
        {
            return string.Format("https://mc-heads.net/head/{0}/{1}", identifier, width);
        }

        public static Image GetHeadImage(string identifier, int width)
        {
            byte[] imageData = new WebClient().DownloadData(GetHeadImageURL(identifier, width));
            MemoryStream imgStream = new MemoryStream(imageData);
            return Image.FromStream(imgStream);
        }

        private static JObject SendPostRequest(string url, string data)
        {
            byte[] payload = Encoding.UTF8.GetBytes(data);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json; charset=UTF-8";
            request.Method = "POST";
            request.ContentLength = payload.Length;

            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(payload, 0, payload.Length);
            }

            JObject ret = null;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        ret = JObject.Parse(reader.ReadToEnd());
                    }
                }
            }
            catch (WebException e)
            {
                using (StreamReader reader = new StreamReader(e.Response.GetResponseStream(), Encoding.UTF8))
                {
                    ret = JObject.Parse(reader.ReadToEnd());
                    throw new BadAuthenticationException((string)ret["error"], (string)ret["errorMessage"]);
                }
            }

            return ret;
        }
    }
}
