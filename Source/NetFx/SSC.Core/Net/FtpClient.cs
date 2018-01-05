#if !NETFX_CORE
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SSC.Net
{
    public class FtpClient
    {
        public event EventHandler<System.Net.FtpWebRequest> OnConfigureFtpWebRequest;

        public string Username { get; set; }

        public string Password { get; set; }

        public async Task<List<FtpListRecord>> ListAsync(string path)
        {
            var clientFtp = _ClientFtpCreate(path);
            clientFtp.Method = System.Net.WebRequestMethods.Ftp.ListDirectoryDetails;
            var ftpList1 = await clientFtp.GetResponseAsync() as System.Net.FtpWebResponse;
            var regexLineTyped = new System.Text.RegularExpressions.Regex(@"(?<date>.+((?i)am|pm))\s*?(<(?<type>\w+)>)\s*(?<name>.+)");
            var regexLineFile = new System.Text.RegularExpressions.Regex(@"(?<date>.+((?i)am|pm))\s*(?<size>\d+)\s*(?<name>.+)");
            var r = new List<FtpListRecord>();
            using (var ftpList1Sr = new System.IO.StreamReader(ftpList1.GetResponseStream()))
            {
                var ftpList1Response = await ftpList1Sr.ReadToEndAsync();
                using (var stringReader = new System.IO.StringReader(ftpList1Response))
                {
                    while (true)
                    {
                        var line = stringReader.ReadLine();
                        if (line == null)
                            break;
                        var match1 = regexLineTyped.Match(line);
                        if (match1.Success)
                        {
                            r.Add(new FtpListRecord
                            {
                                Date = DateTime.Parse(match1.Groups["date"].Value.Trim()),
                                Name = match1.Groups["name"].Value.Trim(),
                                IsDir = match1.Groups["type"].Value.Trim().ToLower() == "dir",
                            });
                        }
                        else
                        {
                            var match2 = regexLineFile.Match(line);
                            if (match2.Success)
                            {
                                r.Add(new FtpListRecord
                                {
                                    Date = DateTime.Parse(match2.Groups["date"].Value.Trim()),
                                    Name = match2.Groups["name"].Value.Trim(),
                                    Size = long.Parse(match2.Groups["size"].Value.Trim()),
                                    IsDir = false,
                                });
                            }
                        }
                    }
                }
            }
            return r;
        }

        private System.Net.FtpWebRequest _ClientFtpCreate(string path)
        {
            var r = System.Net.FtpWebRequest.Create(path) as System.Net.FtpWebRequest;
            if (!string.IsNullOrWhiteSpace(Username))
                r.Credentials = new System.Net.NetworkCredential(Username, Password);
            OnConfigureFtpWebRequest?.Invoke(this, r);
            return r;
        }

        public async Task<Stream> FileOpenAsync(string path)
        {
            var clientFtp = _ClientFtpCreate(path);
            clientFtp.Method = System.Net.WebRequestMethods.Ftp.DownloadFile;
            var ftpResp = await clientFtp.GetResponseAsync() as System.Net.FtpWebResponse;
            return ftpResp.GetResponseStream();
        }
    }
}
#endif