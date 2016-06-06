using System;
using Google.Apis.Analytics.v3.Data;

namespace Endzone.uSplit
{
    public static class LicenseHelper
    {
        internal const int FreeTrialExperimentDurationInDays = 3;

        public static bool HasValidLicense()
        {
            //we do not have a means to license this
            return false;
        }

        public static bool IsCoveredInFreeTrial(Experiment experiment)
        {
            return !experiment.StartTime.HasValue || experiment.StartTime > DateTime.Now.AddDays(-FreeTrialExperimentDurationInDays);
        }
    }
}
