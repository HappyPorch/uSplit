namespace Endzone.uSplit
{
    /// <summary>
    /// Provide an implementation of this interface to register your own segmentation provider
    /// </summary>
    public interface ISegmentationProvider
    {
        string Name { get; }
        /// <summary>
        /// A short key (e.g. "ABC") to identify this provider. Will end up in experiment name!
        /// </summary>
        string ProviderKey { get; }
        bool VisitorInSegment(string segmentKey);
        string SegmentName(string segmentKey);
        string AngularViewPath { get; }
    }
}