﻿using System;
using System.IO;
using System.Net;
using System.Text;

namespace osu_download
{
    class Program
    {
        static void Main(string[] args)
        {
            string Author = "asd";
            string ProgramTitle = "osu! 镜像下载客户端";
            string CurDLClientVer = "b20180108.2";
            string InstallPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\osu!";
            Console.Title = ProgramTitle;
            Console.WriteLine(string.Format("欢迎使用由 {0} 提供的 {1}！", Author, ProgramTitle));
            Console.WriteLine("[广告/反馈] QQ群：132783429");
            Console.WriteLine(string.Format("当前下载客户端版本为：{0}，绿色版客户端默认会安装到 {1}。", CurDLClientVer, InstallPath));
            recheck:
            Console.WriteLine("输入数字然后按下回车(Enter)以选择分支：");
            Console.WriteLine("0 [Latest(最新版)]，1 [Fallback(回退版)]，2 [Beta(测试版)]，3 [CuttingEdge(前沿版)]");
            byte ver;
            while (byte.TryParse(Console.ReadKey(true).KeyChar.ToString(), out ver) != true)
            {
                Console.WriteLine("输入数字然后按下回车(Enter)以选择分支：");
                Console.WriteLine("0 [Latest(最新版)]，1 [Fallback(回退版)]，2 [Beta(测试版)]，3 [CuttingEdge(前沿版)]");
            }
            if (ver > 3)
            {
                goto recheck;
            }
            try
            {
                Console.WriteLine("正在检查选定的分支...如果检查时间过久，可能是因为正在镜像该分支。");
                HttpWebRequest request = WebRequest.Create(string.Format("https://www.userpage.me/osu-update.php?s={0}",ver)) as HttpWebRequest;
                request.Method = "GET";
                request.Timeout = 120000;
                HttpWebResponse webresponse = request.GetResponse() as HttpWebResponse;
                string response = new StreamReader(webresponse.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                webresponse.Close();
                if (string.IsNullOrEmpty(response))
                {
                    throw new Exception("无法获取数据！");
                }
                Console.WriteLine("检查完毕！");
                string mirror = null;
                string[] arrresponse = response.Split(Environment.NewLine.ToCharArray());
                foreach (string tmp in arrresponse)
                {
                    if (tmp.StartsWith("Mirror:"))
                    {
                        mirror = tmp.Replace("Mirror:", "");
                        continue;
                    }
                    if (mirror != null && tmp.StartsWith("File:"))
                    {
                        string uri = tmp.Replace("File:", "");
                        string[] filearr = uri.Split('/');
                        if (filearr.Length < 2)
                        {
                            throw new Exception("数据不正确！");
                        }
                        if (!Directory.Exists(InstallPath))
                        {
                            Console.WriteLine("已创建安装目录。");
                            Directory.CreateDirectory(InstallPath);
                        }
                        string filepath = InstallPath + @"\" + filearr[1];
                        if (File.Exists(filepath))
                        {
                            Console.WriteLine(string.Format("文件已存在：{0}", filearr[1]));
                        }
                        else
                        {
                            Console.WriteLine(string.Format("正在下载：{0}...", filearr[1]));
                            WebClient wc = new WebClient();
                            wc.DownloadFile(mirror + uri, filepath);
                            Console.WriteLine(string.Format("下载完成：{0}", filearr[1]));
                        }
                    }
                }
                Console.WriteLine("全部文件已下载完成！");
            } catch (Exception e)
            {
                Console.WriteLine("下载失败！" + e.Message);
            }
            Console.WriteLine("请按任意键继续...");
            Console.ReadKey(true);
        }
    }
}
