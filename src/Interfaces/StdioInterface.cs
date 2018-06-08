using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DaemonCore;
using Newtonsoft.Json;

namespace DaemonInterface
{
    static public class Stdio
    {
        static public void StartHandle(Route router)
        {
            Console.WriteLine("Start of Stdio Handler.");

            int status;
            string cmd;
            string target, action;
            object[] args;

            while (true)
            {
                Console.Write("[Command]: ");
                cmd = Console.ReadLine();

                if (cmd == "Exit")
                    break;

                (target, action, args, status) = ParseCmd(cmd);
                if (status != 0)
                {
                    Console.WriteLine("Invalid command format.");
                    continue;
                }

                Console.WriteLine("[GetCommand]: " + target + " " + action + " " + JsonConvert.SerializeObject(args, Formatting.Indented));
                Console.WriteLine("[Status]: " + router.DoRoute(target, action, args).DumpJson());
            }

            Console.WriteLine("End of Stdio Handler.");
        }

        static private (string, string, object[], int) ParseCmd(string cmd)
        {
            string target, action;
            List<object> args = new List<object>();

            string pat1 = @"\[.*\]|\w+";
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
                        args.Add( Convert.ToInt32(m[i].Groups[0].Value) );
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
    }
}