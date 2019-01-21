using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using NEXCOMROBOT;
using NEXCOMROBOT.MCAT;

namespace LogicController
{
    static public class EzCameraController
    {
        static public CameraStatus Do(string action, object[] args)
        {
            if (action == "Capture")
            {
                CameraStatus cs = new CameraStatus();
                int camera_id = (int)args[0];
                cs.image = Capture(camera_id, @"C:\Users\miniBot\Desktop\MinibotInterfaceDaemon\tmp.jpeg");
                return cs;
            }
            else
            {
                throw new System.ArgumentException("No Such Action!!", "action");
            }
        }

        static private byte[] Capture(int camera_id,string full_file_name)
        {
            RunCmd(@"C:\Users\miniBot\Desktop\MinibotInterfaceDaemon\camera_scripts\capture.py", camera_id, full_file_name);
            return System.IO.File.ReadAllBytes(full_file_name);
        }

        static private void RunCmd(string cmd, int camera_id, string full_file_name)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = @"C:\Users\minibot\AppData\Local\Programs\Python\Python36-32/python.exe";
            start.Arguments = string.Format("{0} {1} {2}", cmd, camera_id, full_file_name);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            using(Process process = Process.Start(start))
            {
                using(StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.Write(result);
                }
            }
        }
    }
}