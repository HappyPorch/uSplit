using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Endzone.uSplit.Models;

namespace Endzone.uSplit.Pipeline
{
    public class InjectExpiredLicenseWarning : MemoryStream
    {
        private readonly Stream outputStream;
        private readonly Func<string, string> filter;
        public InjectExpiredLicenseWarning(Stream outputStream)
        {
            this.outputStream = outputStream;
            var warning = $"<!-- uSplit ERROR: running A/B experiments for more than {LicenseHelper.FreeTrialExperimentDurationInDays} days is prohibited on a free trial -->";
            filter = s => Regex.Replace(s, @"<body>", $"<body>\n{warning}", RegexOptions.IgnoreCase);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            // capture the data and convert to string 
            var s = Encoding.UTF8.GetString(buffer);

            // filter the string
            s = filter(s);

            // write the data to stream 
            var outdata = Encoding.UTF8.GetBytes(s);
            outputStream.Write(outdata, 0, outdata.GetLength(0));
        }

        public override void Close()
        {
            outputStream.Close();
            base.Close();
        }
    }
}