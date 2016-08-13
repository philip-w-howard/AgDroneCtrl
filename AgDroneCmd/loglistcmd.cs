using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;

namespace AgDroneCtrl
{
    public class LogListCmd : Command
    {
        public LogListCmd(string cmd, NetworkStream socket)
            : base(cmd, socket)
        {
            m_socket = socket;
        }

        protected override void Process()
        {
            String line = "";

            byte[] outStream = System.Text.Encoding.ASCII.GetBytes("loglist\n");
            m_socket.Write(outStream, 0, outStream.Length);
            m_socket.Flush();
            
            line = ReadLine();

            while (!line.Equals("LogEntryDone"))
            {
                if (line.Length > 0)
                Console.WriteLine(line);
                line = ReadLine();
            }
        }

        protected NetworkStream m_socket;
    }
}
