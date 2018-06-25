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
                cs.image = Capture(@"C:\Users\fatesaikou\work\MinibotInterfaceDaemon\tmp.png");
                return cs;
            }
            else
            {
                throw new System.ArgumentException("No Such Action!!", "action");
            }
        }

        static private byte[] Capture(string full_file_name)
        {
            RunCmd(@"C:\Users\fatesaikou\work\MinibotInterfaceDaemon\camera_scripts\capture.py", full_file_name);
            return System.IO.File.ReadAllBytes(full_file_name);
        }

        static private void RunCmd(string cmd, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = @"C:\Users\fatesaikou\AppData\Local\Programs\Python\Python36-32/python.exe";
            start.Arguments = string.Format("{0} {1}", cmd, args);
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