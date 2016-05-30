using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Endzone.uSplit.Models;

namespace Endzone.uSplit.Pipeline
{
    public class InjectVariationReport : MemoryStream
    {
        private readonly Stream outputStream;
        private readonly Func<string, string> filter;
        public InjectVariationReport(Stream outputStream, VariedContent content)
        {
            this.outputStream = outputStream;
            var js = "<script src=\"//www.google-analytics.com/cx/api.js\"></script>\n" +
                     "<script>\n" +
                     $"cxApi.setChosenVariation({content.VariationId},'{content.Experiment.Id}');\n" +
                     "</script>\n";
            filter = s => Regex.Replace(s, @"<head>", $"<head>\n{js}", RegexOptions.IgnoreCase);
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