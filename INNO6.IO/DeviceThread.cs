using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace INNO6.IO
{
    public delegate void ExecuteFunction(object deviceHandler);

    public class DevicePollingBase
    {
        protected int _eventRegenPreiod = 100;
        protected DeviceHandler _deviceHandler;
        public event ExecuteFunction DevicePollingExecute;
        protected bool _isThreadRun = false;

        public virtual void ThreadFunction()
        {

        }

        protected void PollingExecute()
        {
            if (DevicePollingExecute != null)
                DevicePollingExecute(_deviceHandler);
        }

        protected void ThreadFunction(object state)
        {
            if (DevicePollingExecute != null)
                DevicePollingExecute(_deviceHandler);
        }
    }

    public class DeviceTimer : DevicePollingBase
    {
        private Timer _timer = null;

        public DeviceTimer(DeviceHandler deviceHandler, ExecuteFunction executeFunction)
        {
            _deviceHandler = deviceHandler;
            _timer = new Timer(ThreadFunction, deviceHandler, 0, _eventRegenPreiod);
            DevicePollingExecute += executeFunction;
        }

        public override void ThreadFunction()
        {
            PollingExecute();
        }

    }
    public class DeviceThread : DevicePollingBase
    {
        private Thread _thread = null;

        public DeviceThread(DeviceHandler deviceHandler, ExecuteFunction executeFunction)
        {
            _thread = new Thread(new ThreadStart(ThreadFunction));
            _thread.IsBackground = true;
            _thread.Priority = ThreadPriority.Highest;
            _deviceHandler = deviceHandler;
            DevicePollingExecute += executeFunction;
        }

        public void ThreadStart()
        {
            if(!_thread.IsAlive || _thread.ThreadState != ThreadState.Running)
            {
                _isThreadRun = true;
                _thread.Priority = ThreadPriority.Normal;
                _thread.Start();
            }
        }

        public override void ThreadFunction()
        {
            while(_isThreadRun)
            {
                PollingExecute();
                Thread.Sleep(_eventRegenPreiod);
            }
            
        }

        public bool IsThreadAlive()
        {
            return _thread.IsAlive;
        }

        public void ThreadAbort()
        {
            _thread.Abort();
        }
    }
}
