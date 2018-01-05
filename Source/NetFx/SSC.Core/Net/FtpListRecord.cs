using System;
using System.Collections.Generic;
using System.Text;

namespace SSC.Net
{
    public class FtpListRecord
    {
        public bool IsDir { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public long Size { get; set; }
    }
}
