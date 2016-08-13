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
            m_expected_duration = 10;
            m_entries = new List<LogEntry>();
        }

        protected override void Process()
        {
            String line = "";
            char[] DELIMS = { ' ', '\n', '\r' };
            byte[] outString;

            String[] command_words = m_cmd.Split(DELIMS);
            if (command_words.Length > 1)
                outString = System.Text.Encoding.ASCII.GetBytes("loglist" + command_words[1] + "\n");
            else
                outString = System.Text.Encoding.ASCII.GetBytes("loglist\n");

            m_socket.Write(outString, 0, outString.Length);
            m_socket.Flush();
            
            line = ReadLine();

            while (!line.Equals("LogEntryDone"))
            {
                if (line.Length > 0)
                Console.WriteLine(line);

                String[] words = line.Split();
                int entry;
                long size;
                long timestamp;

                if (words.Length > 3)
                {
                    LogEntry log_entry = new LogEntry();
                    log_entry.entry = int.Parse(words[1]);
                    log_entry.size = long.Parse(words[2]);
                    log_entry.timestamp = long.Parse(words[3]);
                    
                    m_entries.Add(log_entry);
                }
                line = ReadLine();
            }
        }

        public int NumEntries()
        {
            return m_entries.Count;
        }

        public LogEntry GetEntry(int index)
        {
            if (index >= m_entries.Count) return null;
            return m_entries[index];
        }

        public class LogEntry
        {
            public int entry;
            public long size;
            public long timestamp;
        }

        protected List<LogEntry> m_entries;
    }
}
