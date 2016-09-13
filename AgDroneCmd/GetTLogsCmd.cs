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
        public GetTLogsCmd(string cmd, NetworkStream socket)
            : base(cmd, socket)
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
            byte[] outString;

            outString = System.Text.Encoding.ASCII.GetBytes("gettlogs\n");

            m_socket.Write(outString, 0, outString.Length);
            m_socket.Flush();
            
            line = ReadLine();
            String[] line_words = line.Split(DELIMS);

            int numFiles = 0;
            int numFailures = 0;
            while (line_words.Length < 2 ||  !line_words[0].Equals("tlogsdone"))
            {
                if (line_words[0].Equals("sendingfile"))
                {
                    var getFile = new GetFile("", m_socket);
                    while (!getFile.IsFinished())
                    {
                        Thread.Sleep(100);
                    }
                    if (getFile.Successful)
                        numFiles++;
                    else
                        numFailures++;
                }
                line = ReadLine();
                line_words = line.Split(DELIMS);
            }

            Console.WriteLine("Successfully received {0} files with {1} failures", numFiles, numFailures);
        }
    }
}
