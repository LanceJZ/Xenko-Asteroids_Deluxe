using Android.App;
using Android.OS;
using Android.Content.PM;

using SiliconStudio.Xenko.Engine;
using SiliconStudio.Xenko.Starter;

namespace Asteroids_Deluxe
{
    [Activity(MainLauncher = true, 
              Icon = "@drawable/icon", 
              ScreenOrientation = ScreenOrientation.Portrait,
              ConfigurationChanges = ConfigChanges.UiMode | ConfigChanges.Orientation | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize)]
    public class Asteroids_DeluxeActivity : AndroidXenkoActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Game = new AsteroidsGame();
            Game.Run(GameContext);
        }

        protected override void OnDestroy()
        {
            Game.Dispose();

            base.OnDestroy();
        }
    }
}
