using System;
using System.Web;
using Endzone.uSplit.Models;

namespace Endzone.uSplit.Pipeline
{
    public class BeforeExperimentExecutedArgs : EventArgs {
        public BeforeExperimentExecutedArgs(Experiment experiment, HttpContextBase httpContext)
        {
            Experiment = experiment;
            HttpContext = httpContext;
        }

        public Experiment Experiment { get; set; }
        public HttpContextBase HttpContext { get; set; }
        public bool SkipExperiment { get; set; }
    }
}