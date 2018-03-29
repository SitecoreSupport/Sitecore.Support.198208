namespace Sitecore.Support.Analytics.Pipelines.EnsureSessionContext
{
    using Sitecore.Analytics.Pipelines.InitializeTracker;
    using Sitecore.Analytics.Tracking;
    using Sitecore.Diagnostics;
    using Sitecore.Support.Analytics.Extensions;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

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
                #region Added Code
                if (!args.Session.CustomData.ContainsKey("MaxPageIndexThresholdWarningLogged"))
                {
                    Log.Warn(string.Format("Session has reached the max page threshold of {0}. If you see this message regularly, you should increase configuration parameter MaxPageIndexThreshold to avoid loss of valid data.", this.MaxPageIndexThreshold), this);
                    args.Session.CustomData.Add("MaxPageIndexThresholdWarningLogged", true);
                }

                Session session = args.Session;
                #endregion

                MemoryStream memoryStream = new MemoryStream();
                BinaryFormatter binaryFormatter = new BinaryFormatter();

                #region Modified code
                binaryFormatter.Serialize(memoryStream, session);
                #endregion

                memoryStream.Position = 0L;

                #region Added Code
                Session session2 = (Session)binaryFormatter.Deserialize(memoryStream);
                session2.SetOriginalSession(session);
                args.Session = session2;
                #endregion

                memoryStream.Close();
                memoryStream.Dispose();                
            }
        }
    }
}