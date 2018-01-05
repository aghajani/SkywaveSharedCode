using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSC.Net
{
    public static class Extensions
    {

        public static Task<System.Net.WebResponse> GetResponseAsync(this System.Net.WebRequest webReq)
        {
            TaskCompletionSource<System.Net.WebResponse> taskCR = new TaskCompletionSource<System.Net.WebResponse>();
            //
            webReq.BeginGetResponse((asyncR) =>
            {
                try
                {
                    taskCR.TrySetResult(webReq.EndGetResponse(asyncR));
                }
                catch (Exception ex1)
                {
                    taskCR.TrySetException(ex1);
                }
            }, null);
            //
            return taskCR.Task;
        }

    }
}
