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
    public class DataFlashCmd : Command
    {
        public DataFlashCmd(string cmd, ComStream agdrone)
            : base(cmd, agdrone)
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
            char[] DELIMS = { ' ', '\n', '\r' };

            String[] command_words = m_cmd.Split(DELIMS);
            if (command_words.Length <= 1)
            {
                Console.WriteLine("Must specify a log id");
                return;
            }

            Console.WriteLine("Sending logdata command: logdata {0}", command_words[1]);
            m_agdrone.WriteString("logdata " + command_words[1] + "\n");

            var getFile = new GetFile("", m_agdrone);
            while (!getFile.IsFinished())
            {
                Thread.Sleep(100);
            }

        }

        protected bool GetLogEntry(String id)
        {
            String entry_cmd = String.Format("loglist {0}", id);
            m_log_entry = new LogListCmd(entry_cmd, m_agdrone);

            while (!m_log_entry.IsFinished() && !m_log_entry.MayBeHung())
            {
                Thread.Sleep(100);
            }

            if (m_log_entry.IsFinished() && m_log_entry.NumEntries() > 0)
            {
                m_expectedSize = m_log_entry.GetEntry(0).size;
                if (m_expectedSize < 1) m_expectedSize = 1;
                return true;
            }

            return false;
        }

        protected LogListCmd m_log_entry;
        protected long m_expectedSize;
    }
}

