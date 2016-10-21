using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Endzone.uSplit.GoogleApi;
using Endzone.uSplit.Models;

namespace Endzone.uSplit.Pipeline
{
    public class VariationReportingHttpResponseFilter : MemoryStream
    {
        private readonly Stream outputStream;
        private readonly VariedContent content;
        private readonly Func<bool> scriptsWritten;
        private bool wroteScripts;

        public VariationReportingHttpResponseFilter(Stream outputStream, VariedContent content, Func<bool> scriptsWrittenBefore)
        {
            this.outputStream = outputStream;
            this.content = content;
            scriptsWritten = () => scriptsWrittenBefore() || wroteScripts;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (scriptsWritten())
            {
                //we already wrote the scripts, just pass the response
                outputStream.Write(buffer, offset, count);
            }     
            else
            {
                var fragment = ScriptsHelper.ReportVariations(content.AppliedVariations);

                // get the transmitted html
                var html = Encoding.UTF8.GetString(buffer);

                // append scripts after head if present
                var transformed = Regex.Replace(html, @"<head>", $"<head>\n{fragment}", RegexOptions.IgnoreCase);

                // did we just write the scripts?
                wroteScripts = !string.Equals(html, transformed);

                // write the data to stream 
                var outdata = Encoding.UTF8.GetBytes(transformed);
                outputStream.Write(outdata, 0, outdata.GetLength(0));
            }
        }

        public override void Close()
        {
            outputStream.Close();
            base.Close();
        }
    }
}