using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;

namespace AgDroneCtrl
{
    public class DataFlashCmd : Command
    {
        public DataFlashCmd(string cmd, NetworkStream socket)
            : base(cmd, socket)
        {
            m_expected_duration = 10;
        }

        protected override void Process()
        {
            String line = "";
            char[] DELIMS = { ' ', '\n', '\r' };
            byte[] outString;

            String[] command_words = m_cmd.Split(DELIMS);
            if (command_words.Length <= 1)
            {
                Console.WriteLine("Must specify a log id");
                return;
            }

            if (!GetLogEntry(command_words[1]))
            {
                Console.WriteLine("Unable to fetch metadata");
                return;
            }

            outString = System.Text.Encoding.ASCII.GetBytes("logdata " + command_words[1] + "\n");

            m_socket.Write(outString, 0, outString.Length);
            m_socket.Flush();
            
            line = ReadLine();
            String[] line_words = line.Split(DELIMS);

            while (line_words.Length < 2 ||  !line.Equals("filesize"))
            {
                line = ReadLine();
                line_words = line.Split(DELIMS);
            }

            Console.WriteLine("File size is {0}", line_words[1]);
        }

        protected bool GetLogEntry(String id)
        {
            String entry_cmd = String.Format("loglist {0}", id);
            m_log_entry = new LogListCmd(entry_cmd, m_socket);

            while (!m_log_entry.IsFinished() && !m_log_entry.MayBeHung())
            {
                Thread.Sleep(100);
            }

            if (m_log_entry.IsFinished() && m_log_entry.NumEntries() > 0) return true;

            return false;
        }

        protected LogListCmd m_log_entry;
    }
}

