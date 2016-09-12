using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Security.Cryptography;

namespace AgDroneCtrl
{
    public class GetFile : Command
    {
        public GetFile(string cmd, NetworkStream socket)
            : base(cmd, socket)
        {
            m_expected_duration = 60;
            m_expectedSize = -1;
            m_fileName = null;
        }

        public override void Abort()
        {
            base.Abort();
        }

        protected override void Process()
        {
            const String SOCKET_ADDR = "192.168.2.6";       // WiFi mode
            //const String SOCKET_ADDR = "192.168.42.1";     // AP mode
            const int SOCKET_PORT = 2004;

            String line = "";
            char[] DELIMS = { ' ', '\n', '\r' };
            String[] line_words = {""};

            Console.WriteLine("Attempting to get a file");

            while (line_words.Length < 2 ||  m_expectedSize < 0 || m_fileName == null)
            {
                line = ReadLine();
                Console.WriteLine("Read: {0}", line);
                line_words = line.Split(DELIMS);
                if (line_words[0].Equals("filesize"))
                {
                    m_expectedSize = Convert.ToInt32(line_words[1]);
                }
                else if (line_words[0].Equals("filename"))
                {
                    m_fileName = line_words[1];
                }
            }

            Console.WriteLine("Getting file {0} or size {1}", m_fileName, m_expectedSize);

            System.Net.Sockets.TcpClient dataSocket = new System.Net.Sockets.TcpClient();
            dataSocket.Connect(SOCKET_ADDR, SOCKET_PORT);

            FileStream fs = File.Open(m_fileName, FileMode.Create, FileAccess.Write, FileShare.None);
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

            var sum = MD5.Create();
            var stream = File.OpenRead(m_fileName);
            byte[] fileMD5Sum = sum.ComputeHash(stream);
            string fileMD5String = System.Text.Encoding.UTF8.GetString(fileMD5Sum, 0, fileMD5Sum.Length);
            Console.WriteLine("MD5 sum: {0}", fileMD5String);

            while (line_words.Length < 2 || !line_words[0].Equals("md5sum"))
            {
                line = ReadLine();
                Console.WriteLine("Read: {0}", line);
                line_words = line.Split(DELIMS);
            }

            if (line_words[1].Equals(fileMD5String))
            {
                Console.WriteLine("File transfered successfullly");
            }
            else
            {
                Console.WriteLine("MD5 sums did not match");
            }
        }

        protected long m_expectedSize;
        protected String m_fileName;

    }
}
