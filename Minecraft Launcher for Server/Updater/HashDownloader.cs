﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Minecraft_Launcher_for_Server.Updater
{
    class FileObj
    {
        public string FilePath { get; set; }
        public string Hash { get; set; }
        public long Size { get; set; }

        public FileObj(JProperty p)
        {
            FilePath = p.Name;
            Hash = (string)p.Value["hash"];
            Size = (long)p.Value["size"];
        }

        public string GetActualPath(string parent, bool useHashPath)
        {
            return Path.Combine(parent, useHashPath ? (Hash.Substring(0, 2) + "/" + Hash) : FilePath);
        }
    }

    public class ProgressArgs
    {
        public double Progress { get; }
        public string Status { get; }

        public ProgressArgs(double progress, string status)
        {
            Progress = progress;
            Status = status;
        }
    }

    public class HashDownloader
    {
        // 저장 경로를 Hash 형태로 저장할 건지?
        public bool UseHashPath { get; set; } = false;

        // Hash 검사를 통해 업데이트가 필요한 파일만 다운로드할 건지?
        public bool DownloadOnlyNecessary { get; set; } = true;


        public event EventHandler<ProgressArgs> OnProgress;

        private CancellationTokenSource _tknSrc = new CancellationTokenSource();
        private string _savePath;
        private string _indexesURL;
        private string _resourceUrl;
        private int _count = 0;
        private int _total = 0;
        private volatile bool _isRunning = false;
        private volatile bool _isCanceling = false;

        public HashDownloader(string savePath, string indexesURL, string resourceUrl)
        {
            _savePath = savePath;
            _indexesURL = indexesURL;
            _resourceUrl = resourceUrl;
        }

        public void Cancel()
        {
            if (_isRunning)
            {
                _tknSrc.Cancel();
                _isCanceling = true;
                UpdateStatus((_count * 100.0 / _total), "취소하고 있습니다..");
            }
        }

        public Task DownloadTask()
        {
            if (!_isRunning)
            {
                _isRunning = true;
                return Task.Factory.StartNew(() => Download());
            }
            return null;
        }

        private void UpdateStatus(double progress, string status)
        {
            OnProgress?.Invoke(this, new ProgressArgs(progress, status));
        }

        private bool CheckHash(SHA1Managed sha1, string parent, FileObj file)
        {
            string path = file.GetActualPath(parent, UseHashPath);
            if (!File.Exists(path))
                return false;
            if (new FileInfo(path).Length != file.Size)
                return false;

            StringBuilder sb = new StringBuilder(40);
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                byte[] hash = sha1.ComputeHash(fs);
                foreach (byte b in hash)
                    sb.AppendFormat("{0:X2}", b);
            }
            return sb.ToString() == file.Hash;
        }

        private void Download()
        {
            UpdateStatus(0, "Index파일 다운로드중..");
            string indexData = new WebClient().DownloadString(_indexesURL);
            string parentFolder = _savePath;

            if (UseHashPath)
            {
                string indexesFolder = Path.Combine(_savePath, "indexes");
                parentFolder = Path.Combine(_savePath, "objects");

                if (!Directory.Exists(indexesFolder))
                    Directory.CreateDirectory(indexesFolder);

                File.WriteAllText(Path.Combine(indexesFolder, Path.GetFileName(_indexesURL)), indexData);
            }

            if (!Directory.Exists(parentFolder))
                Directory.CreateDirectory(parentFolder);

            JObject indexDataJson = JObject.Parse(indexData);
            SHA1Managed sha1 = new SHA1Managed();
            List<FileObj> files = new List<FileObj>();

            UpdateStatus(0, "다운로드해야 할 파일 검색중..");
            foreach (var token in indexDataJson["objects"])
            {
                JProperty p = token as JProperty;
                if (p != null)
                {
                    FileObj file = new FileObj(p);
                    if (DownloadOnlyNecessary && CheckHash(sha1, parentFolder, file))
                        Debug.WriteLine("Ignore " + file.FilePath);
                    else
                        files.Add(file);
                }
            }

            sha1.Dispose();
            Interlocked.Exchange(ref _total, files.Count);
            UpdateStatus(0, "다운로드중..");

            try
            {
                Parallel.ForEach(files,
                    new ParallelOptions { MaxDegreeOfParallelism = 10, CancellationToken = _tknSrc.Token },
                    (file) => DownloadFile(parentFolder, file));
            } 
            catch (OperationCanceledException e)
            {
                Console.WriteLine(e.Message);
                _isRunning = false;
                _isCanceling = false;
            }
        }

        private void DownloadFile(string path, FileObj file)
        {
            path = file.GetActualPath(path, UseHashPath);

            DirectoryInfo parent = Directory.GetParent(path);
            if (!parent.Exists)
                Directory.CreateDirectory(parent.FullName);

            string downloadUrl = Path.Combine(_resourceUrl, file.Hash.Substring(0, 2) + "/" + file.Hash);
            var sr = new BinaryReader(WebRequest.Create(downloadUrl).GetResponse().GetResponseStream());
            var sw = new BinaryWriter(new FileStream(path, FileMode.Create));

            byte[] buf = new byte[1024];
            int len;
            while ((len = sr.Read(buf, 0, buf.Length)) > 0)
                sw.Write(buf, 0, len);

            sr.Close();
            sw.Close();
            Interlocked.Increment(ref _count);

            UpdateStatus((_count * 100.0 / _total), _isCanceling ? "취소하고 있습니다.." : "다운로드중..");
            if(_count == _total)
            {
                _isRunning = false;
            }
        }
    }
}