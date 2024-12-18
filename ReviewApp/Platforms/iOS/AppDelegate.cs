using Foundation;
using UIKit;

namespace ReviewApp;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    public override bool FinishedLaunching(UIApplication app, NSDictionary options)
    {
        //Firebase.Core.App.Configure();
        return base.FinishedLaunching(app, options);
    }
}


