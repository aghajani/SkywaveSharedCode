using SSC.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SSC.Net
{
    public class WebApiClient
    {
        public Func<HttpClient> HttpClientCreate { get; set; }

        string _ServiceURL = "";

        public WebApiClient(string serviceUrl = "")
        {
            if (!_ServiceURL.EndsWith("/"))
                _ServiceURL = _ServiceURL + "/";
            _ServiceURL = serviceUrl;
        }

        //public object Transport_Api(string apiName)
        //{
        //    return new
        //    {
        //        url = Url_Api(apiName),
        //        headers = new
        //        {
        //            Authorization = Request_AuthorizationHeaderValue.ToString()
        //        },
        //    };
        //}

        public string Url_Api(string apiName, bool includeHostAddress = true)
        {
            string r = "";
            if (includeHostAddress)
                r += _ServiceURL;
            r += $"api/{apiName}";
            return r;
        }

        private AuthenticationHeaderValue _Request_AuthorizationHeaderValue = null;
        /// <summary>
        /// ...
        /// </summary>
        public AuthenticationHeaderValue Request_AuthorizationHeaderValue
        {
            get { return _Request_AuthorizationHeaderValue; }
            set { _Request_AuthorizationHeaderValue = value; }
        }
        public string Request_AuthorizationHeaderValue_Text
        { get { return (Request_AuthorizationHeaderValue == null) ? "" : Request_AuthorizationHeaderValue.ToString(); } }


        public async Task<TaskRunResult<byte[]>> InvokeApiDownload(string apiAddress)
        {
            return await _Download(Url_Api(apiAddress, false));
        }
        public async Task<TaskRunResult<TResult>> InvokeApi<TResult>(string apiName)
        {
            return await Post<TResult>(Url_Api(apiName, false));
        }
        public async Task<TaskRunResult<bool>> InvokeApi<TInput>(string apiName, TInput arg)
        {
            return await Post<TInput>(Url_Api(apiName, false), arg);
        }
        public async Task<TaskRunResult<TResult>> InvokeApi<TResult, TInput>(string apiName, TInput arg)
        {
            return await Post<TResult, TInput>(Url_Api(apiName, false), arg);
        }
        public async Task<TaskRunResult<string>> InvokeApi_Native<TInput>(string apiName, TInput arg)
        {
            return await _Post<string, TInput>(Url_Api(apiName, false), arg, true, true, true);
        }

        public async Task<TaskRunResult<TResult>> PostHttpContent<TResult>(string path, HttpContent content)
        {
            return await _Post<TResult, HttpContent>(path, content, true);
        }
        public async Task<TaskRunResult<bool>> PostHttpContent(string path, HttpContent content)
        {
            var r_Post = await _Post<bool, HttpContent>(path, content, false);
            TaskRunResult<bool> r = new TaskRunResult<bool>(r_Post.IsSuccess, r_Post.Exception, r_Post.Canceled);
            return r;
        }
        public async Task<TaskRunResult<bool>> Post<TInput>(string path, TInput arg)
        {
            var r_Post = await _Post<bool, TInput>(path, arg, false);
            TaskRunResult<bool> r = new TaskRunResult<bool>(r_Post.IsSuccess, r_Post.Exception, r_Post.Canceled);
            return r;
        }
        public async Task<TaskRunResult<TResult>> Post<TResult>(string path)
        {
            return await _Post<TResult, bool?>(path, null, true, hasArg: false);
        }
        public async Task<TaskRunResult<TResult>> Post<TResult, TInput>(string path, TInput arg)
        {
            return await _Post<TResult, TInput>(path, arg, true);
        }

        public async Task<TaskRunResult<TResult>> Get<TResult>(string path)
        {
            return await _Run<TResult, bool?>(RunModes.Get, path, null, true, hasArg: false);
        }

        private async Task<TaskRunResult<TResult>> _Post<TResult, TInput>(string path, TInput arg, bool hasReturn, bool hasArg = true, bool returnContentAsString = false)
        { return await _Run<TResult, TInput>(RunModes.Post, path, arg, hasReturn, hasArg, returnContentAsString); }

        private enum RunModes
        {
            Post,
            Get,
            Put,
            Delete,
        }
        private async Task<TaskRunResult<TResult>> _Run<TResult, TInput>(RunModes mode, string path, TInput arg, bool hasReturn, bool hasArg = true, bool returnContentAsString = false)
        {
            TaskRunResult<TResult> r;
            try
            {
                using (var client = HttpClientCreate?.Invoke() ?? new HttpClient())
                {
                    if (!string.IsNullOrWhiteSpace(_ServiceURL))
                        client.BaseAddress = new Uri(_ServiceURL);
                    client.DefaultRequestHeaders.AcceptEncoding.Clear();
                    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (Request_AuthorizationHeaderValue != null)
                        client.DefaultRequestHeaders.Authorization = Request_AuthorizationHeaderValue;

                    // New code:
                    HttpResponseMessage response;
                    switch (mode)
                    {
                        case RunModes.Post:
                            if (hasArg)
                            {
                                if (typeof(TInput) == typeof(HttpContent))
                                    response = await client.PostAsync(path, arg as HttpContent);
                                else
                                    response = await client.PostAsync(path, new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(arg), Encoding.UTF8, "application/json"));// .PostAsJsonAsync<TInput>(path, arg);
                            }
                            else
                                response = await client.PostAsync(path, null);
                            break;
                        case RunModes.Get:
                            response = await client.GetAsync(path);
                            break;
                        case RunModes.Put:
                            if (hasArg)
                            {
                                if (typeof(TInput) == typeof(HttpContent))
                                    response = await client.PutAsync(path, arg as HttpContent);
                                else
                                    response = await client.PutAsync(path, new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(arg), Encoding.UTF8, "application/json"));// .PostAsJsonAsync<TInput>(path, arg);
                            }
                            else
                                response = await client.PutAsync(path, null);
                            break;
                        case RunModes.Delete:
                            response = await client.DeleteAsync(path);
                            break;
                        default:
                            throw new ArgumentException();
                    }

                    if (!response.IsSuccessStatusCode)
                    {
                        var ex1 = new System.Net.WebException(response.StatusCode.ToString());
                        ex1.Data.Add("content", await _Response_ReadContent(response));
                        r = new TaskRunResult<TResult>(default(TResult), ex1);
                    }
                    else
                    {
                        TResult r_InPost = default(TResult);
                        if (hasReturn)
                        {
                            if (returnContentAsString)
                                r_InPost = (TResult)((object)await _Response_ReadContent(response));
                            else
                                r_InPost = Newtonsoft.Json.JsonConvert.DeserializeObject<TResult>(await _Response_ReadContent(response));
                        }
                        r = new TaskRunResult<TResult>(r_InPost);
                    }
                }
            }
            catch (Exception ex1)
            {
                r = new TaskRunResult<TResult>(default(TResult), ex1);
            }
            return r;
        }

        private async Task<string> _Response_ReadContent(HttpResponseMessage response)
        {
            string r = "";
            bool isProccessed = false;
            if (response.Content.Headers.Contains("Content-Encoding"))
            {
                var contentEncoding = response.Content.Headers.GetValues("Content-Encoding").First().ToLower();
                if (contentEncoding == "deflate")
                {
                    using (var streamDecompress = new DeflateStream(await response.Content.ReadAsStreamAsync(), CompressionMode.Decompress))
                    {
                        using (var streamReader = new StreamReader(streamDecompress))
                        {
                            r = await streamReader.ReadToEndAsync();
                        }
                    }
                    isProccessed = true;
                }
                else if (contentEncoding == "gzip")
                {
                    using (var streamDecompress = new GZipStream(await response.Content.ReadAsStreamAsync(), CompressionMode.Decompress))
                    {
                        using (var streamReader = new StreamReader(streamDecompress))
                        {
                            r = await streamReader.ReadToEndAsync();
                        }
                    }
                    isProccessed = true;
                }
            }
            if (!isProccessed)
                r = await response.Content.ReadAsStringAsync();
            return r;
        }

        private async Task<TaskRunResult<byte[]>> _Download(string path)
        {
            TaskRunResult<byte[]> r;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrWhiteSpace(_ServiceURL))
                        client.BaseAddress = new Uri(_ServiceURL);
                    client.Timeout = TimeSpan.FromSeconds(15);
                    if (Request_AuthorizationHeaderValue != null)
                        client.DefaultRequestHeaders.Authorization = Request_AuthorizationHeaderValue;

                    // New code:
                    HttpResponseMessage response;
                    response = await client.GetAsync(path, HttpCompletionOption.ResponseHeadersRead);

                    if (!response.IsSuccessStatusCode)
                    {
                        var ex1 = new System.Net.WebException(response.StatusCode.ToString());
                        r = new TaskRunResult<byte[]>(null, ex1);
                    }
                    else
                    {
                        var responseSize = int.Parse(response.Headers.GetValues("ContentLength").First());
                        byte[] responseReadBuffer = new byte[4096];
                        List<byte> responseContent = new List<byte>();
                        using (var responseS1 = await response.Content.ReadAsStreamAsync())
                        {
                            int countRead = 0;
                            while (countRead < responseSize)
                            {
                                var taskRead = responseS1.ReadAsync(responseReadBuffer, 0, responseReadBuffer.Length);
                                var taskTimeout = Task.Delay(2000); //buffer size: 4096 => on a 32Kbit/s it will take 1 second
                                var taskDone = await Task.WhenAny(taskRead, taskTimeout);
                                if (taskDone == taskRead && taskRead.Result > 0)
                                {
                                    for (int i1 = 0; i1 < taskRead.Result; i1++)
                                        responseContent.Add(responseReadBuffer[i1]);
                                    countRead += taskRead.Result;
                                }
                                else
                                    throw new TimeoutException();
                            }
                        }
                        if (responseContent.Count <= 0)
                            throw new ArgumentException();
                        r = new TaskRunResult<byte[]>(responseContent.ToArray());
                    }
                }
            }
            catch (Exception ex1)
            {
                r = new TaskRunResult<byte[]>(null, ex1);
            }
            return r;
        }
    }
}
