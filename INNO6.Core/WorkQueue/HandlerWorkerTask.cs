using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INNO6.Core.Threading
{
    public delegate void WorkEventHandler(object sender, object param);

    public class HandlerWorkerTask : WorkItem 
    {
        private object _sender;
        private object _workItem;
        private WorkEventHandler _handler;

        public HandlerWorkerTask(object sender, object workItem, WorkEventHandler handler)
        {
            _sender = sender;
            _workItem = workItem;
            _handler = handler;
        }

        public override void Perform()
        {
            if (_handler != null)
                _handler(_sender, _workItem);
        } 
    }
}
