using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSC.Threading.Tasks
{
    public class TaskRunResult<T>
    {
        public TaskRunResult(T r, Exception r_Ex = null, bool canceled = false)
        {
            Result = r;
            Exception = r_Ex;
            Canceled = canceled;
        }

        public Exception Exception { get; private set; }

        public bool Canceled { get; private set; }

        public T Result { get; private set; }

        public bool IsSuccess { get { return (Exception == null && !Canceled); } }
    }
}
