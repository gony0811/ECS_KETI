using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace INNO6.Core
{
    public class XSequence //: Object
    {
        private Thread thread = null;
        private List<XSeqFunction> m_SeqFunctions = new List<XSeqFunction>();
        protected int m_ScanTime = 500;
        private bool m_SeqRunFlag = true;

        public List<XSeqFunction> SeqFunctions
        {
            get { return m_SeqFunctions; }
        }

        public int ScanTime
        {
            get { return m_ScanTime; }
            set { m_ScanTime = value; }
        }

        public Thread ThreadInfo
        {
            get { return thread; }
            set { thread = value; }
        }

        public XSequence()
        {
            this.thread = new Thread(new ThreadStart(this.threadProc));
            this.thread.IsBackground = true;
        }

        public XSequence(int scanTime)
            : this()
        {
            m_ScanTime = scanTime;
        }

        public void RegisterSequence(XSeqFunction seq)
        {
            SeqFunctions.Add(seq);
        }

        protected virtual void RegisterSequences()
        {
        }

        public void threadProc()
        {
            while (m_SeqRunFlag)
            {
                Sequence();
                Thread.Sleep(m_ScanTime);

            }
        }

        protected virtual void Sequence()
        {
            Thread.Sleep(m_ScanTime);
        }

        public Boolean IsAlive()
        {
            return this.thread.IsAlive;
        }

        public Boolean IsStarted()
        {
            int i = (int)(this.thread.ThreadState & ThreadState.Unstarted);
               
            return (i == 0) ? true : false;
        }

        public Boolean IsRunnig()
        {
            return (this.thread.ThreadState == ThreadState.Running);
        }

        public Boolean IsSuspended()
        {

            int i = (int)(this.thread.ThreadState & ThreadState.Suspended);

            return (i != 0) ? true : false;
        }

        public Boolean IsAborted()
        {
            int i = (int)(this.thread.ThreadState & ThreadState.Aborted);

            return (i != 0) ? true : false;
        }

        public void Join()
        {
            Resume();

            if (IsAlive())
            {
                m_SeqRunFlag = false;
                this.thread.Join();
            }

        }

        public void Start()
        {
            if (false == IsStarted())
            {
                this.thread.Start();
            }
            else
            {
                Resume();
            }
        }

        public void Abort()
        {
            Resume();

            if (IsAlive()) this.thread.Abort();
        }

        public void Pause()
        {
            if (true == IsStarted() &&
                false == IsSuspended())
            {
                this.thread.Abort();
            }
        }

        public void Resume()
        {
            if (true == IsStarted() &&
                true == IsSuspended())
            {
                //this.thread.Resume();
            }
        }
    }
}
