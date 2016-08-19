using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.IO;

namespace AgDroneCtrl
{
    class GetTLogsCmd : Command
    {
        public GetTLogsCmd(string cmd, NetworkStream socket)
            : base(cmd, socket)
        {
            m_expected_duration = 60;
            m_expectedSize = 1;
        }
        public override void Abort()
        {
            if (m_log_entry != null) m_log_entry.Abort();
            base.Abort();
        }

        protected override void Process()
        {
            const String SOCKET_ADDR = "192.168.2.6";       // WiFi mode
            //const String SOCKET_ADDR = "192.168.42.1";     // AP mode
            const int SOCKET_PORT = 2004;

            String line = "";
            char[] DELIMS = { ' ', '\n', '\r' };
            byte[] outString;

            outString = System.Text.Encoding.ASCII.GetBytes("gettlogs\n");

            m_socket.Write(outString, 0, outString.Length);
            m_socket.Flush();
            
            line = ReadLine();
            String[] line_words = line.Split(DELIMS);

            while (line_words.Length < 2 ||  !line_words[0].Equals("filesize"))
            {
                Console.WriteLine("Read: {0}", line);
                line = ReadLine();
                line_words = line.Split(DELIMS);
            }

            Console.WriteLine("File size is {0}", line_words[1]);

            System.Net.Sockets.TcpClient dataSocket = new System.Net.Sockets.TcpClient();
            dataSocket.Connect(SOCKET_ADDR, SOCKET_PORT);

            string path = String.Format("pixhawk_{0}.bin", m_log_entry.GetEntry(0).timestamp);
            Console.WriteLine("Creating file {0}", path);
            FileStream fs = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None);
            BinaryWriter file = new BinaryWriter(fs);

            long fileSize = 0;
            int size = 10;
            byte[] buffer = new byte[1024 * 1024];
            
            try
            {
                //while (dataSocket.GetStream().CanRead && size > 0)
                while (size > 0)
                {
                    size = dataSocket.GetStream().Read(buffer, 0, buffer.Length);
                    if (size > 0)
                    {
                        fileSize += size;
                        file.Write(buffer, 0, size);
                        Console.Write("Received {0} bytes {1:F2}% of {2} bytes    \r", 
                            fileSize, 100.0*fileSize/m_expectedSize, m_expectedSize);
                    }
                }
            }
            catch (IOException e)
            {
                // How to decide if I got the whole thing or if something went wrong?
                Console.WriteLine("Received exception: {0}", e);
            }

            file.Close();
            Console.WriteLine("");
            Console.WriteLine("Wrote {0} bytes", fileSize);
        }


    }
}
