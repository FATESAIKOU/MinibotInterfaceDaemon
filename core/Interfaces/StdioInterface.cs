using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DaemonCore;
using Newtonsoft.Json;

namespace DaemonInterface
{
    static public class Stdio
    {
        static public void StartHandle(Route router, StreamReader src, StreamWriter dest)
        {
            int status;
            string cmd;
            string target, action;
            object[] args;

            while (true)
            {
                if (dest != null)
                    dest.Write("[Command]: ");

                // Get command
                cmd = src.ReadLine();
                if (cmd == "Exit")
                    break;

                // Parse command and check
                (target, action, args, status) = ParseCmd(cmd);
                if (status != 0)
                {
                    Console.WriteLine("Invalid command format.");
                    continue;
                }

                // Check command contain
                if (dest != null)
                    dest.WriteLine("[GetCommand]: " + target + " " + action + " " + JsonConvert.SerializeObject(args, Formatting.Indented));

                // Check command type (Macro or Other)
                if (target == "Exec")
                {
                    ExecFile(router, action, args);
                }
                else
                {
                    string robot_status = router.DoRoute(target, action, args).DumpJson();

                    if (dest != null)
                        dest.WriteLine("[Status]: " + robot_status);
                }
            }
        }

        static private (string, string, object[], int) ParseCmd(string cmd)
        {
            string target, action;
            List<object> args = new List<object>();

            string pat1 = @"\[.*\]|[^ ]+";
            string pat2 = @"[+-]?[0-9]+([.]?[0-9]+)?";
            MatchCollection m, m2;

            // Match cmd with pat1
            m = Regex.Matches(cmd, pat1);
            if ( m.Count < 2)
                return (null, null, null, 1);

            // Extract target & action
            target = m[0].Groups[0].Value;
            action = m[1].Groups[0].Value;

            // Extract args
            try
            {
                for (int i = 2; i < m.Count; i ++)
                {
                    if (m[i].Groups[0].Value[0] == '[')
                    {
                        List<double> tmp_double = new List<double>();
                        m2 = Regex.Matches(m[i].Groups[0].Value, pat2);

                        if ( m2.Count != 6 )
                            return (null, null, null, 1);

                        foreach (Match me in m2)
                        {
                            tmp_double.Add(Convert.ToDouble(me.Groups[0].Value));
                        }

                        args.Add(tmp_double.ToArray());
                    }
                    else
                    {
                        int v;
                        if ( Int32.TryParse(m[i].Groups[0].Value, out v) )
                            args.Add( v );
                        else
                            args.Add( m[i].Groups[0].Value );
                    }
                }
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
                return (null, null, null, 1);
            }

            // Return
            return (target, action, args.ToArray(), 0);
        }

        static private void ExecFile(Route router, string file_path, object[] args)
        {
            StreamReader src = new StreamReader(file_path);;

            int limit = 1;
            if (args.Length > 0) limit = (int)args[0];

            for (int i = 0; i < limit; i ++)
            {
                Console.WriteLine("[EXEC_FILE][Round-(" + (i+1).ToString() + "/" + limit + ")][" + file_path + "]");

                StartHandle(router, src, null);
                src.BaseStream.Seek(0, SeekOrigin.Begin);

                Console.WriteLine("[EXEC_FILE][Status] " + router.DoRoute("Robot", "GetStatus", null).DoMap().state);
            }

            src.Close();
        }
    }
}