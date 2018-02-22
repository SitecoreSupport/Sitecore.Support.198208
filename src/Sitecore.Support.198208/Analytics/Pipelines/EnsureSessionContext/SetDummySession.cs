using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Abstractions;
using Sitecore.Analytics.Pipelines.InitializeTracker;
using Sitecore.Analytics.Tracking;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Support.Analytics.Extensions;

namespace Sitecore.Support.Analytics.Pipelines.EnsureSessionContext
{
  public class SetDummySession : InitializeTrackerProcessor
  {
    private readonly BaseLog _log;

    public int MaxPageIndexThreshold
    {
      get;
      set;
    }

    public SetDummySession() : this(ServiceLocator.ServiceProvider.GetRequiredService<BaseLog>())
    {
    }

    public SetDummySession(BaseLog log)
    {
      Assert.ArgumentNotNull(log, "log");
      this._log = log;
    }

    public override void Process(InitializeTrackerArgs args)
    {
      if (args.Session != null && args.Session.Interaction != null && args.Session.Interaction.PageCount >= this.MaxPageIndexThreshold)
      {
        if (!args.Session.CustomData.ContainsKey("MaxPageIndexThresholdWarningLogged"))
        {
          this._log.Warn(string.Format("Session has reached the max page threshold of {0}. If you see this message regularly, you should increase configuration parameter MaxPageIndexThreshold to avoid loss of valid data.", this.MaxPageIndexThreshold), this);
          args.Session.CustomData.Add("MaxPageIndexThresholdWarningLogged", true);
        }

        Session session = args.Session;
        MemoryStream memoryStream = new MemoryStream();
        BinaryFormatter expr_AF = new BinaryFormatter();
        expr_AF.Serialize(memoryStream, session);
        memoryStream.Position = 0L;
        Session session2 = (Session)expr_AF.Deserialize(memoryStream);
        session2.SetOriginalSession(session);
        args.Session = session2;
        memoryStream.Close();
        memoryStream.Dispose();
      }
    }
  }
}
