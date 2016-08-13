using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;

namespace AgDroneCtrl
{
    public abstract class Command
    {
        public Command(string cmd, NetworkStream socket)
        {
            m_cmd = cmd;
            m_socket = socket;
            m_Thread = new Thread( new ThreadStart(Process));
            m_Thread.Start();

            // Give the thread a chance to start up
            for (int ii=0; ii<1000; ii++)
            {
                if (m_Thread.IsAlive) break;
                Thread.Sleep(1);
            }

            m_socket_buffer = new byte[1024 * 1024];
            m_line_index = 0;
            m_line_extent = 0;
        }

        public void Abort()
        {
            if (m_Thread != null) m_Thread.Abort();
            m_Thread.Join();
        }

        abstract protected void Process();

        protected String ReadLine()
        {
            int size;
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

            return line.Trim();
        }

        protected Thread m_Thread;
        protected string m_cmd;
        protected NetworkStream m_socket;
        protected byte[] m_socket_buffer;
        protected int m_line_index;
        protected int m_line_extent;
    }
}
