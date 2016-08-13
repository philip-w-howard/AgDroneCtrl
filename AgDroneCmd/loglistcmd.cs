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
            : base(cmd)
        {
            m_socket = socket;
        }

        protected override void Process()
        {
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes("loglist\n");
            m_socket.Write(outStream, 0, outStream.Length);
            m_socket.Flush();
            while (true)
            {
                Console.WriteLine("processing {0}", m_cmd);
                Thread.Sleep(1000);
            }
        }

        protected NetworkStream m_socket;
    }
}
