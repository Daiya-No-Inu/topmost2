using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
//using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TopMost2
{
    internal class ConfigData
    {
        public string[] TopWindows { get; set; }
    }

    public class Config
    {
        private static string CONFIG_PATH =$"./config.cfg";

        private ConfigData configData=new ConfigData();

        public Config() 
        {
            LoadConfig();
            ApplyConfig();
        }

        private void LoadConfig()
        {
            if(File.Exists(CONFIG_PATH))
            {
                string s=File.ReadAllText(CONFIG_PATH);
                configData.TopWindows=s.Split(';');
            }
            else
            {
                File.Create(CONFIG_PATH).Dispose();
                configData.TopWindows=Array.Empty<string>();
            }
        }

        private void ApplyConfig()
        {
            ApplyTopWindows();
        }

        private void ApplyTopWindows()
        {

            Process[] procs = Process.GetProcesses();
            IntPtr hWnd;
            List<(string, IntPtr)> it = new List<(string, IntPtr)>();
            for (int i = 0; i < procs.Length; i++)
            {
                if ((hWnd = procs[i].MainWindowHandle) != IntPtr.Zero && !API.IsWindowsCore(hWnd))
                {
                    it.Add((API.GetWindowTitle(hWnd), hWnd));
                }
            }
            foreach (string w in configData.TopWindows)
            {
                foreach ((string, IntPtr) i in it)
                {
                    if (Regex.IsMatch(i.Item1, w))
                    {
                        API.SetTopMost(i.Item2, true);
                    }
                }
            }
        }

        //private void LoadConfig()
        //{
        //    try
        //    {
        //        if (File.Exists(CONFIG_PATH))
        //        {
        //            string json = File.ReadAllText(CONFIG_PATH);
        //            ConfigData data = JsonSerializer.Deserialize<ConfigData>(json);
        //            if (data != null)
        //                configData = data;
        //        }
        //        else
        //        {
        //            // 如果文件不存在，就生成一个默认的配置文件
        //            SaveDefaultConfig();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"加载配置失败: {ex.Message}");
        //        SaveDefaultConfig();
        //    }
        //}

        //private void SaveDefaultConfig()
        //{
        //    configData = new ConfigData();
        //    configData.TopWindows = new string[0];
        //    if (File.Exists(CONFIG_PATH))
        //    {
        //        File.Delete(CONFIG_PATH);
        //    }
        //    var options = new JsonSerializerOptions
        //    {
        //        WriteIndented = true,
        //    };
        //    string json=JsonSerializer.Serialize(configData,options);
        //    File.WriteAllText(CONFIG_PATH, json);
        //}
    }
}
