using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace AgDroneCtrl
{
    public abstract class Command
    {
        public Command(string cmd)
        {
            m_cmd = cmd;
            m_Thread = new Thread( new ThreadStart(Process));
            m_Thread.Start();

            // Give the thread a chance to start up
            for (int ii=0; ii<1000; ii++)
            {
                if (m_Thread.IsAlive) break;
                Thread.Sleep(1);
            }
        }

        public void Abort()
        {
            if (m_Thread != null) m_Thread.Abort();
            m_Thread.Join();
        }

        abstract protected void Process();

        protected Thread m_Thread;
        protected string m_cmd;
    }
}
