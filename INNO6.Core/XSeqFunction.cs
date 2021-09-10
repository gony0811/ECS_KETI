using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INNO6.Core
{
    public class XSeqFunction
    {
        private int m_SeqNo = 0;
        private int m_ReturnSeqNo = 0;
        private int m_AlarmId = 0;
        private string m_SeqFunName;
        protected UInt32 m_StartTicks = XConvert.GetTickCount();
        protected List<UInt32> m_ExtraStartTicks = new List<UInt32>();

        private DateTime m_StartTime;

        public int SeqNo
        {
            get { return m_SeqNo; }
            set { m_SeqNo = value; }
        }

        public int ReturnSeqNo
        {
            get { return m_ReturnSeqNo; }
            set { m_ReturnSeqNo = value; }
        }

        public int AlarmId
        {
            get { return m_AlarmId; }
            set { m_AlarmId = value; }
        }

        public string SeqFunName
        {
            get { return m_SeqFunName; }
            set { m_SeqFunName = value; }
        }

        public DateTime StartTime
        {
            get { return m_StartTime; }
            set { m_StartTime = value; }
        }

        public UInt32 StartTicks
        {
            get { return m_StartTicks; }
            set { m_StartTicks = value; }
        }

        public List<UInt32> StartTicksExtra
        {
            get { return m_ExtraStartTicks; }
            set { m_ExtraStartTicks = value; }
        }

        public XSeqFunction()
        {
            InitSeq();
        }

        public virtual void InitSeq()
        {
            this.SeqNo = 0;
        }

        public virtual int Do()
        {
            int result = -1;
            int nSeqNo = this.SeqNo;

            switch (nSeqNo)
            {
                case 0:
                    break;
            }

            this.SeqNo = nSeqNo;

            return result;
        }

        //public double GetElapsedTicks()
        //{
        //    TimeSpan diff = DateTime.Now - StartTime;
        //    return diff.TotalMilliseconds;
        //}

        public UInt32 GetElapsedTicks()
        {
            return XConvert.GetTickCount() - m_StartTicks;
        }

        public void SetExtraStartTicks(int count)
        {
            for (int i = 0; i < count; i++)
            {
                m_ExtraStartTicks.Add(XConvert.GetTickCount());
            }
        }

        public UInt32 GetElapsedTicks(int id)
        {
            return XConvert.GetTickCount() - m_ExtraStartTicks[id];
        }
    }
}
