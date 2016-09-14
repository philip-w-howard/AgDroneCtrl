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
        public Command(string cmd, ComStream agdrone)
        {
            m_cmd = cmd;
            m_agdrone = agdrone;
            m_Thread = new Thread(new ThreadStart(Process));
            m_Thread.Start();

            m_start_time = DateTime.Now;
            m_expected_duration = 2;    // default 2 second duration

            // Give the thread a chance to start up
            for (int ii=0; ii<1000; ii++)
            {
                if (m_Thread.IsAlive) break;
                Thread.Sleep(1);
            }

        }

        public virtual void Abort()
        {
            if (m_Thread != null) m_Thread.Abort();
            m_Thread.Join();
        }

        abstract protected void Process();

        public bool IsFinished()
        {
            return !m_Thread.IsAlive;
        }

        public bool MayBeHung()
        {
            TimeSpan duration = DateTime.Now.Subtract(m_start_time);

            return duration.TotalSeconds > m_expected_duration;
        }

        protected Thread m_Thread;
        protected string m_cmd;
        protected DateTime m_start_time;
        protected double m_expected_duration;   // in seconds
        protected ComStream m_agdrone;
    }
}
