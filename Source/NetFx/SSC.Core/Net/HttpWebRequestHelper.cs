using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSC.Net
{
    public class HttpWebRequestHelper
    {
        public event EventHandler Download_ProgressChanged;

        public int? Download_Length { get; set; }
        public int Download_Done { get; set; }

        /// <summary>
        /// Return calculated progress of download using length and done
        /// </summary>
        public double? Download_ProgressInPercent
        {
            get { return (Download_Length == null) ? null : (double?)((double)Download_Done / (double)Download_Length.Value) * 100d; }
        }

        public async Task<T> Download<T>(string inUrl, int retryTimesOnException = 1, int timeout_Header_InSconds = 3, int timeout_Content_InSconds = 10, Action<Exception> exHandler = null, System.Threading.CancellationToken? cToken = null, bool cacheEnabled = false, Action<List<string>> logHandler = null)
        {
            object r = null;
            int retryTime_Counter;
            retryTime_Counter = 1;
            List<string> logs = null;
            if (logHandler != null)
                logs = new List<string>();
            while (retryTime_Counter <= retryTimesOnException)
            {
                try
                {
                    if (cToken != null && cToken.Value.IsCancellationRequested)
                        break;
                    if (logs != null)
                        logs.Add($"Try - Start: {retryTime_Counter}");
                    Download_Done = 0;
                    Download_Length = null;
                    try
                    {
                        System.Net.HttpWebRequest req_Header = System.Net.WebRequest.Create(inUrl) as System.Net.HttpWebRequest;
                        req_Header.Method = "HEAD";
                        var req_Header_GetResponseAsync_Task = req_Header.GetResponseAsync();
                        if (await Task.WhenAny(req_Header_GetResponseAsync_Task, Task.Delay(TimeSpan.FromSeconds(timeout_Header_InSconds))) == req_Header_GetResponseAsync_Task)
                        {
                            using (System.Net.WebResponse resp_Header = req_Header_GetResponseAsync_Task.Result)
                            {
                                int resp_Header_ContentLength;
                                if (resp_Header.Headers.AllKeys.Count(a1 => a1 == "Content-Length") > 0)
                                    if (int.TryParse(resp_Header.Headers["Content-Length"], out resp_Header_ContentLength))
                                    {
                                        Download_Length = resp_Header_ContentLength;
                                        if (logs != null)
                                            logs.Add($"Download_Length - Try 1: {Download_Length}");
                                    }
                            }
                        }
                        else
                        {
                            throw new TimeoutException($"Time out (HEADER) on URL: {inUrl}");
                        }
                    }
                    catch (Exception ex1)
                    {
                        if (logs != null)
                            logs.Add($"DownloadLength - Try 1 Ex: {ex1.ToString()}");
                    }
                    //
                    if (cToken != null && cToken.Value.IsCancellationRequested)
                        break;
                    //
                    System.Net.HttpWebRequest hWReq = System.Net.WebRequest.Create(inUrl) as System.Net.HttpWebRequest;
                    hWReq.Method = "GET";
                    hWReq.Accept = null;
                    //if (!cacheEnabled)
                    //{
                    //    hWReq.Headers["Cache-Control"] = "no-cache";
                    //    hWReq.Headers["Pragma"] = "no-cache";
                    //}
                    //
                    System.Net.WebResponse hWResp = null;
                    System.Net.WebRequest hwReq_Typed = hWReq as System.Net.WebRequest;
                    var hWResp_GetResponseAsync_Task = hwReq_Typed.GetResponseAsync();
                    if (await Task.WhenAny(hWResp_GetResponseAsync_Task, Task.Delay(TimeSpan.FromSeconds(timeout_Content_InSconds))) == hWResp_GetResponseAsync_Task)
                    {
                        if (cToken != null && cToken.Value.IsCancellationRequested)
                            break;
                        //
                        hWResp = hWResp_GetResponseAsync_Task.Result;
                        // Check for content length in GET header (if no success using HEAD method)
                        if (Download_Length == null)
                        {
                            int resp_Header_ContentLength;
                            if (hWResp.Headers.AllKeys.Count(a1 => a1 == "Content-Length") > 0)
                                if (int.TryParse(hWResp.Headers["Content-Length"], out resp_Header_ContentLength))
                                {
                                    Download_Length = resp_Header_ContentLength;
                                    if (logs != null)
                                        logs.Add($"Download_Length - Try 2: {Download_Length}");
                                }
                        }
                        // Read the response into a Stream object.
                        byte[] data_Native;
                        using (System.IO.MemoryStream ms1 = new System.IO.MemoryStream())
                        {
                            using (System.IO.Stream responseStream = hWResp.GetResponseStream())
                            {
                                int bufferSize = 8192;
                                byte[] buffer = new byte[bufferSize];
                                int bufferUsed;
                                do
                                {
                                    if (cToken != null && cToken.Value.IsCancellationRequested)
                                        break;
                                    //
                                    Task<int> responseStream_ReadAsync_Task;
                                    if (cToken != null)
                                        responseStream_ReadAsync_Task = responseStream.ReadAsync(buffer, 0, bufferSize, cToken.Value);
                                    else
                                        responseStream_ReadAsync_Task = responseStream.ReadAsync(buffer, 0, bufferSize);
                                    //
                                    if (await Task.WhenAny(responseStream_ReadAsync_Task, Task.Delay(TimeSpan.FromSeconds(timeout_Content_InSconds))) == responseStream_ReadAsync_Task)
                                    {
                                        bufferUsed = responseStream_ReadAsync_Task.Result;
                                        if (bufferUsed > 0)
                                        {
                                            ms1.Write(buffer, 0, bufferUsed);
                                            Download_Done += bufferUsed;
                                            if (Download_ProgressChanged != null)
                                                Download_ProgressChanged(this, EventArgs.Empty);
                                        }
                                    }
                                    else
                                        throw new TimeoutException($"Time out (GET - Read) on URL: {inUrl}");
                                } while (bufferUsed > 0);
                            }
                            //
                            if (cToken != null && cToken.Value.IsCancellationRequested)
                                break;
                            //
                            data_Native = ms1.ToArray();
                            ms1.Position = 0;
                            //
                            if (typeof(T) == typeof(string))
                            {
                                string data;
                                using (var reader = new System.IO.StreamReader(ms1))
                                {
                                    data = reader.ReadToEnd();
                                }
                                r = data;
                            }
                            else if (typeof(T) == typeof(byte[]))
                            {
                                r = data_Native;
                            }
                            else if (typeof(T) == typeof(System.IO.MemoryStream))
                            {
                                System.IO.MemoryStream ms2 = new System.IO.MemoryStream(data_Native);
                                r = ms2;
                            }
                        }
                    }
                    else
                    {
                        throw new TimeoutException($"Time out (GET) on URL: {inUrl}");
                    }
                }
                catch (Exception ex)
                {
                    if (logs != null)
                        logs.Add($"Try {retryTime_Counter} Ex: {ex.ToString()}");
                    //
                    r = null;
                    if (exHandler != null)
                        exHandler(ex);
                    //
                    var we = ex.InnerException as System.Net.WebException;
                    if (we != null)
                    {
                        //var resp = we.Response as System.Net.HttpWebResponse;
                        //if (resp != null)
                        //{
                        //    var code = resp.StatusCode;
                        //}
                        Debug.WriteLine("RespCallback Exception raised! Message:{0}" + we.Message);
                        Debug.WriteLine("Status:{0}", we.Status);
                    }
                    else
                        Debug.WriteLine("Unknown Exception: {0}", ex.ToString());
                }
                //
                if (cToken != null && cToken.Value.IsCancellationRequested)
                    break;
                //
                if (r != null)
                    break;
                retryTime_Counter++;
            }
            //
            if (cToken != null)
                cToken.Value.ThrowIfCancellationRequested();
            //
            if (logHandler != null)
                logHandler(logs);
            //
            return (T)r;
        }
    }
}
