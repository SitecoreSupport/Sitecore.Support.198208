using Sitecore.Analytics.Tracking;
using Sitecore.Diagnostics;

namespace Sitecore.Support.Analytics.Extensions
{
  internal static class SessionExtensions
  {
    internal const string OriginalSessionKey = "SessionExtensions.OriginalSessionKey";

    public static Session GetOriginalSession(this Session session)
    {
      Assert.ArgumentNotNull(session, "session");
      if (session.CustomData.ContainsKey("SessionExtensions.OriginalSessionKey"))
      {
        var holder = session.CustomData[OriginalSessionKey] as SessionHolder;
        return holder != null ? holder.Session : null;
      }

      return null;
    }

    public static void SetOriginalSession(this Session session, Session originalSession)
    {
      Assert.ArgumentNotNull(session, "session");
      session.CustomData[OriginalSessionKey] = new SessionHolder(originalSession);
    }

    internal class SessionHolder
    {
      public SessionHolder(Session session)
      {
        Session = session;
      }

      public Session Session { get; private set; }
    }
  }
}