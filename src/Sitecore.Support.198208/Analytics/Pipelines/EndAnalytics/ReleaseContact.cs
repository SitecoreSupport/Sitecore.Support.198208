using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Abstractions;
using Sitecore.Analytics;
using Sitecore.Analytics.Tracking;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;
using Sitecore.Support.Analytics.Extensions;

namespace Sitecore.Support.Analytics.Pipelines.EndAnalytics
{
  public class ReleaseContact
  {
    private readonly BaseFactory _factory;

    private readonly BaseLog _log;

    public ReleaseContact() : this(ServiceLocator.ServiceProvider.GetRequiredService<BaseFactory>(), ServiceLocator.ServiceProvider.GetRequiredService<BaseLog>())
    {
    }

    internal ReleaseContact(BaseFactory factory, BaseLog log)
    {
      Assert.ArgumentNotNull(factory, "factory");
      Assert.ArgumentNotNull(log, "log");
      this._factory = factory;
      this._log = log;
    }

    public void Process(PipelineArgs args)
    {
      if (Tracker.Current == null)
      {
        this._log.Debug("Tracker is not initialized. ReleaseContact processor is skipped");
        return;
      }

      Session session = Tracker.Current.Session;
      Assert.IsNotNull(session, "Tracker.Current.Session");

      bool transferInProgress =
        (bool) session.GetType()
          .GetProperty("TransferInProcess", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty)
          .GetValue(session);

      if (transferInProgress)
      {
        this._log.Debug("Contact is being transferred. ReleaseContact processor is skipped");
        return;
      }

      if (session.Contact == null)
      {
        this._log.Debug("Contact is null. ReleaseContact processor is skipped");
        return;
      }

      if (session.Settings.IsTransient)
      {
        this._log.Debug("Session is in TRANSIENT MODE. ReleaseContact processor is skipped");
        return;
      }

      if (session.IsReadOnly)
      {
        return;
      }

      ContactManager expr_9D = this._factory.CreateObject("tracking/contactManager", true) as ContactManager;
      Assert.IsNotNull(expr_9D, "tracking/contactManager");
      session = (session.GetOriginalSession() ?? session);
      expr_9D.SaveAndReleaseContact(session.Contact);
      session.Contact = null;
    }
  }
}
