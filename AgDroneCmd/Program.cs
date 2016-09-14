using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using System.Net.Sockets;
using System.IO;
using System.Net;
using AgDroneCtrl;

namespace AgDroneCmd
{
    class Program
    {
        static void Main(string[] args)
        {
            char[] DELIMS = { ' ', '\n', '\r' };
            String command = "";
            Command cmd = null;

            ComStream agdrone = new ComStream();

            Console.Write("Cmd> ");
            command = Console.ReadLine();
            while (command != null && !command.Equals("quit", StringComparison.OrdinalIgnoreCase))
            {
                String[] command_words = command.Split(DELIMS);
                if (command_words.Length > 0)
                {
                    if (command_words[0].Equals("loglist", StringComparison.OrdinalIgnoreCase))
                    {
                        if (cmd != null) cmd.Abort();
                        cmd = new LogListCmd(command, agdrone);
                    }
                    else if (command_words[0].Equals("getlog", StringComparison.OrdinalIgnoreCase))
                    {
                        if (cmd != null) cmd.Abort();
                        cmd = new DataFlashCmd(command, agdrone);
                    }
                    else if (command_words[0].Equals("gettlogs", StringComparison.OrdinalIgnoreCase))
                    {
                        if (cmd != null) cmd.Abort();
                        cmd = new GetTLogsCmd(command, agdrone);
                    }
                    else 
                    {
                        Console.WriteLine("Unrecognized command");
                    }
                }
                
                while (!cmd.IsFinished() && !cmd.MayBeHung())
                {
                    Thread.Sleep(100);
                }

                Console.Write("Cmd> ");
                command = Console.ReadLine();
            }

            if (cmd != null) cmd.Abort();
        }
    }
}
