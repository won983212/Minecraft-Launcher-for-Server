using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft_Launcher_for_Server.Updater
{
    public enum RetrieveState
    {
        Processing, Loaded, Error
    }

    public class ServerStatus
    {
        private MemoryStream ms = new MemoryStream();

        // minecraft server status
        public string Motd { get; private set; }
        public int PlayersOnline { get; private set; }
        public int PlayersMax { get; private set; }
        public int Protocol { get; private set; }

        // api server status
        public string PatchVersion { get; private set; }
        public string ClientVersion { get; private set; }

        public static bool IsActiveAPIServer()
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URLs.InfoFile);
                req.Timeout = 3000;
                req.AllowAutoRedirect = false;
                req.Method = "HEAD";
                req.GetResponse().Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task RetrieveAPIServerVersion()
        {
            using (WebClient client = new WebClient())
            {
                string data = await client.DownloadStringTaskAsync(URLs.InfoFile);
                JObject obj = JObject.Parse(data);
                PatchVersion = (string)obj["patchVersion"];
                ClientVersion = (string)obj["clientVersion"];
            }
        }

        public async Task RetrieveServerStatus()
        {
            Tuple<string, int> data = await Task.Factory.StartNew(RetrieveServerStatusSync);

            if (data.Item2 == 0x19)
                throw new InvalidDataException(data.Item1);

            JObject json = JObject.Parse(data.Item1);
            Motd = (string)json["description"]["text"];
            PlayersOnline = (int)json["players"]["online"];
            PlayersMax = (int)json["players"]["max"];
            Protocol = (int)json["version"]["protocol"];

            Logger.Debug("[ServerStatus] Server status has retrieved");
        }

        private Tuple<string, int> RetrieveServerStatusSync()
        {
            string serverIP = Properties.Settings.Default.MinecraftServerIP;

            TcpClient client = new TcpClient();
            client.Connect(serverIP, 25565);

            Logger.Debug("[ServerStatus] Connected to " + serverIP);
            BufferedStream stream = new BufferedStream(client.GetStream());

            BinaryWriter bw = new BinaryWriter(stream);
            WriteVarInt(ms, -1);
            WriteString(ms, serverIP);
            WriteUnsignedShort(ms, 25565);
            WriteVarInt(ms, 1);
            Flush(bw, 0);
            Flush(bw, 0);
            Logger.Debug("[ServerStatus] Sent handshake packet.");

            BinaryReader br = new BinaryReader(stream);
            int len = ReadVarInt(br); // content-length
            int id = ReadVarInt(br); // id

            Logger.Debug("[ServerStatus] Packet " + id + " is receved. (" + len + ")");
            string data = ReadString(br);

            client.Close();
            return new Tuple<string, int>(data, id);
        }

        private void Flush(BinaryWriter bw, int id)
        {
            byte[] data = ms.ToArray();
            ms.SetLength(0);

            int idLen = WriteVarInt(ms, id);
            byte[] idData = ms.ToArray();
            ms.SetLength(0);

            WriteVarInt(bw.BaseStream, data.Length + idLen);
            bw.Write(idData, 0, idData.Length);
            bw.Write(data, 0, data.Length);
            bw.Flush();
        }

        private int ReadVarInt(BinaryReader br)
        {
            var value = 0;
            var size = 0;
            int b;
            while ((((b = br.ReadByte()) & 0x80) == 0x80))
            {
                value |= (b & 0x7F) << (size++ * 7);
                if (size > 5)
                    throw new IOException("This VarInt is too big!");
            }
            return value | ((b & 0x7F) << (size * 7));
        }

        private string ReadString(BinaryReader br)
        {
            int size = ReadVarInt(br);
            byte[] data = br.ReadBytes(size);
            return Encoding.UTF8.GetString(data);
        }

        private void WriteString(Stream stream, string value)
        {
            byte[] data = Encoding.UTF8.GetBytes(value);
            WriteVarInt(stream, data.Length);
            stream.Write(data, 0, data.Length);
        }

        private int WriteVarInt(Stream stream, int value)
        {
            int size = 1;
            while((value & -128) != 0)
            {
                stream.WriteByte((byte)(value & 127 | 128));
                value = (byte)(((uint)value) >> 7);
                size++;
            }
            stream.WriteByte((byte) value);
            return size;
        }

        private void WriteUnsignedShort(Stream stream, int value)
        {
            stream.WriteByte((byte)((value >> 8) & 0xff));
            stream.WriteByte((byte)(value & 0xff));
        }
    }
}
