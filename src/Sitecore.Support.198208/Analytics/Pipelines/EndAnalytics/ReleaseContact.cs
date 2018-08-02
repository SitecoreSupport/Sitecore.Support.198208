namespace Sitecore.Support.Analytics.Pipelines.EndAnalytics
{
    using Sitecore.Analytics;
    using Sitecore.Analytics.Tracking;
    using Sitecore.Configuration;
    using Sitecore.Diagnostics;
    using Sitecore.Pipelines;
    using Sitecore.Support.Analytics.Extensions;
    using System.Reflection;

    public class ReleaseContact
    {
        public void Process(PipelineArgs args)
        {
            if (Tracker.Current == null)
            {
                Log.Debug("Tracker is not initialized. ReleaseContact processor is skipped");
                return;
            }
            Session session = Tracker.Current.Session;
            Assert.IsNotNull(session, "Tracker.Current.Session");

            bool transferInProgress =
              (bool)session.GetType()
                .GetProperty("TransferInProcess", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty)
                .GetValue(session);

            if (transferInProgress)
            {
                Log.Debug("Contact is being transferred. ReleaseContact processor is skipped");
                return;
            }

            if (session.Contact == null)
            {
                Log.Debug("Contact is null. ReleaseContact processor is skipped");
                return;
            }

            ContactManager contactManager = Factory.CreateObject("tracking/contactManager", true) as ContactManager;
            Assert.IsNotNull(contactManager, "tracking/contactManager");
            if (session.Settings.IsTransient)
            {
                Log.Debug("Session is in TRANSIENT MODE. ReleaseContact processor is skipped");
                return;
            }

            if (session.IsReadOnly)
            {
              return;
            }

            #region Added code
            session = (session.GetOriginalSession() ?? session);
            #endregion

            contactManager.SaveAndReleaseContact(session.Contact);
            session.Contact = null;
        }
    }
}