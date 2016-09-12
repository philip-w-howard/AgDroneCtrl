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
        const String SOCKET_ADDR = "192.168.2.6";       // WiFi mode
        //const String SOCKET_ADDR = "192.168.42.1";     // AP mode
        const int SOCKET_PORT = 2003;

        static void Main(string[] args)
        {
            char[] DELIMS = { ' ', '\n', '\r' };
            String command = "";
            Command cmd = null;

            System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
            clientSocket.Connect(SOCKET_ADDR, SOCKET_PORT);

            NetworkStream serverStream = clientSocket.GetStream();

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
                        cmd = new LogListCmd(command, serverStream);
                    }
                    else if (command_words[0].Equals("getlog", StringComparison.OrdinalIgnoreCase))
                    {
                        if (cmd != null) cmd.Abort();
                        cmd = new DataFlashCmd(command, serverStream);
                    }
                    else if (command_words[0].Equals("gettlogs", StringComparison.OrdinalIgnoreCase))
                    {
                        if (cmd != null) cmd.Abort();
                        cmd = new GetTLogsCmd(command, serverStream);
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
