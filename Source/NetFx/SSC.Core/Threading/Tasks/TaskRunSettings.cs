using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSC.Threading.Tasks
{
    public class TaskRunSettings
    {
        private int _TryCount = 5;

        public int TryCount
        {
            get { return _TryCount; }
            set { _TryCount = value; }
        }

        private int _DelayBetweenTriesInSeconds = 2;

        public int DelayBetweenTriesInSeconds
        {
            get { return _DelayBetweenTriesInSeconds; }
            set { _DelayBetweenTriesInSeconds = value; }
        }

        private TimeSpan? _TimeOut = null;

        public TimeSpan? TimeOut
        {
            get { return _TimeOut; }
            set { _TimeOut = value; }
        }


        private bool _ThrowExceptionOnFailure = false;
        /// <summary>
        /// ...
        /// </summary>
        public bool ThrowExceptionOnFailure
        {
            get { return _ThrowExceptionOnFailure; }
            set { _ThrowExceptionOnFailure = value; }
        }
    }
}
