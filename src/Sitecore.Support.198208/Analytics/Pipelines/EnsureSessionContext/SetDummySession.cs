namespace Sitecore.Support.Analytics.Pipelines.EnsureSessionContext
{
    using Sitecore.Analytics.Pipelines.InitializeTracker;
    using Sitecore.Analytics.Tracking;

    public class SetDummySession : InitializeTrackerProcessor
    {
        public int MaxPageIndexThreshold
        {
            get;
            set;
        }

        public override void Process(InitializeTrackerArgs args)
        {
            if (args.Session != null && args.Session.Interaction != null && args.Session.Interaction.PageCount >= this.MaxPageIndexThreshold)
            {
                System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, args.Session);
                memoryStream.Position = 0L;
                args.Session = (Session)binaryFormatter.Deserialize(memoryStream);
                memoryStream.Close();
                memoryStream.Dispose();
            }
        }
    }
}