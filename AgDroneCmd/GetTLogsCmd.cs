using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;

namespace AgDroneCtrl
{
    class GetTLogsCmd : Command
    {
        public GetTLogsCmd(string cmd, ComStream agdrone)
            : base(cmd, agdrone)
        {
            m_expected_duration = 60;
        }
        public override void Abort()
        {
            base.Abort();
        }

        protected override void Process()
        {
            String line = "";
            char[] DELIMS = { ' ', '\n', '\r' };

            m_agdrone.WriteString("gettlogs\n");
            
            line = m_agdrone.ReadLine();
            String[] line_words = line.Split(DELIMS);

            int numFiles = 0;
            int numFailures = 0;
            while (true)
            {
                if (line_words.Length >= 1)
                {
                    if (line_words[0].Equals("tlogsdone")) 
                        break;
                    else if (line_words[0].Equals("sendingfile"))
                    {
                        var getFile = new GetFile("", m_agdrone);
                        while (!getFile.IsFinished())
                        {
                            Thread.Sleep(100);
                        }
                        if (getFile.Successful)
                            numFiles++;
                        else
                            numFailures++;
                    }
                }
                line = m_agdrone.ReadLine();
                line_words = line.Split(DELIMS);
            }

            Console.WriteLine("Successfully received {0} files with {1} failures", numFiles, numFailures);
        }
    }
}
