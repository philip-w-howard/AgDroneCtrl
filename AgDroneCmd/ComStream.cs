using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;

namespace AgDroneCtrl
{
    public class ComStream
    {
        public ComStream()
        {
            const String SOCKET_ADDR = "192.168.2.6";       // WiFi mode
            //const String SOCKET_ADDR = "192.168.42.1";     // AP mode
            const int SOCKET_PORT = 2003;

            System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
            clientSocket.Connect(SOCKET_ADDR, SOCKET_PORT);

            m_socket = clientSocket.GetStream();

            m_socket_buffer = new byte[1024 * 1024];
            m_line_index = 0;
            m_line_extent = 0;
        }

        public String ReadLine()
        {
            String line = "";
            do
            {
                do
                {
                    while (m_line_index == m_line_extent)
                    {
                        m_line_extent = m_socket.Read(m_socket_buffer, 0, m_socket_buffer.Length);
                        m_line_index = 0;
                    }

                    line += Convert.ToChar(m_socket_buffer[m_line_index]);
                    m_line_index++;
                } while (m_socket_buffer[m_line_index] != '\n' && m_line_index < m_line_extent);
            } while (m_socket_buffer[m_line_index] != '\n');

            m_line_index++;

            Console.WriteLine("Read {0}", line.Trim());

            return line.Trim();
        }

        public void WriteString(string msg)
        {
            byte[] outString;

            outString = System.Text.Encoding.ASCII.GetBytes(msg);

            m_socket.Write(outString, 0, outString.Length);
            m_socket.Flush();
        }

        protected NetworkStream m_socket;
        protected byte[] m_socket_buffer;
        protected int m_line_index;
        protected int m_line_extent;
    }
}
