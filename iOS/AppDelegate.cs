using Foundation;
using UIKit;

using Facebook.CoreKit;
using Google.Maps;
using Xamarin.InAppPurchase;
using System.Diagnostics;

namespace Drop.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations

		public static LocationHelper MyLocationHelper = new LocationHelper();

		#region Public Properties
		public static InAppPurchaseManager PurchaseManager = new InAppPurchaseManager();
		#endregion


		public override UIWindow Window
		{
			get;
			set;
		}

		public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
		{
			// Override point for customization after application launch.
			// If not required for your application you can safely delete this method

			MapServices.ProvideAPIKey(Constants.GOOGLE_MAP_API_KEY);

			Facebook.CoreKit.Settings.AppID = Constants.FACEBOOK_APP_ID;
			Facebook.CoreKit.Settings.DisplayName = Constants.FACEBOOK_DISPLAY_NAME;

			// In App Purchase Shared Secret Key
			string value = Xamarin.InAppPurchase.Utilities.Security.Unify(
				new string[] { "a93112ea75",
					"e54da8803",
					"482be33",
					"32e1a9" },
				new int[] { 0, 1, 2, 3 });

			// Initialize the In App Purchase Manager

#if SIMULATED
			PurchaseManager.SimulateiTunesAppStore = true;
#else
			PurchaseManager.SimulateiTunesAppStore = false;
#endif
			PurchaseManager.PublicKey = value;

			//PurchaseManager.ApplicationUserName = "Digicache";

			// Warn user that the store is not available
			if (PurchaseManager.CanMakePayments)
			{
				//Alert 
				Debug.WriteLine("Xamarin.InAppBilling: User can make payments to iTunes App Store.");
			}
			else
			{
				//Display Alert Dialog Box
				using (var alert = new UIAlertView("Xamarin.InAppBilling", "Sorry but you cannot make purchases from the In App Billing store. Please try again later.", null, "OK", null))
				{
					alert.Show();
				}

			}

			// Warn user if the Purchase Manager is unable to connect to
			// the network.
			PurchaseManager.NoInternetConnectionAvailable += () =>
			{
				//Display Alert Dialog Box
				using (var alert = new UIAlertView("Xamarin.InAppBilling", "No open internet connection is available.", null, "OK", null))
				{
					alert.Show();
				}
			};

			// Show any invalid product queries
			PurchaseManager.ReceivedInvalidProducts += (productIDs) =>
			{
				// Display any invalid product IDs to the console
				Debug.WriteLine("The following IDs were rejected by the iTunes App Store:");
				foreach (string ID in productIDs)
				{
					Debug.WriteLine(ID);
				}
				Debug.WriteLine(" ");
			};

			// Report the results of the user restoring previous purchases
			PurchaseManager.InAppPurchasesRestored += (count) =>
			{
				// Anything restored?
				if (count == 0)
				{
					// No, inform user
					using (var alert = new UIAlertView("Xamarin.InAppPurchase", "No products were available to be restored from the iTunes App Store.", null, "OK", null))
					{
						alert.Show();
					}
				}
				else
				{
					// Yes, inform user
					using (var alert = new UIAlertView("Xamarin.InAppPurchase", string.Format("{0} {1} restored from the iTunes App Store.", count, (count > 1) ? "products were" : "product was"), null, "OK", null))
					{
						alert.Show();
					}
				}
			};

			// Report miscellanous processing errors
			PurchaseManager.InAppPurchaseProcessingError += (message) =>
			{
				//Display Alert Dialog Box
				using (var alert = new UIAlertView("Xamarin.InAppPurchase", message, null, "OK", null))
				{
					alert.Show();
				}
			};

			// Report any issues with persistence
			PurchaseManager.InAppProductPersistenceError += (message) =>
			{
				using (var alert = new UIAlertView("Xamarin.InAppPurchase", message, null, "OK", null))
				{
					alert.Show();
				}
			};

			// Setup automatic purchase persistance and load any previous purchases
			PurchaseManager.AutomaticPersistenceType = InAppPurchasePersistenceType.LocalFile;
			PurchaseManager.PersistenceFilename = "AtomicData";
			PurchaseManager.ShuffleProductsOnPersistence = false;
			PurchaseManager.RestoreProducts();

#if SIMULATED
			// Ask the iTunes App Store to return information about available In App Products for sale
			PurchaseManager.QueryInventory (new string[] { 
				"DropDistantCache",
				"NoExpiryDrop",
				"OpenDistantCache"
			});

			// Setup the list of simulated purchases to restore when doing a simulated restore of pruchases
			// from the iTunes App Store
			PurchaseManager.SimulatedRestoredPurchaseProducts = "OpenDistantCache";
#else
			// Ask the iTunes App Store to return information about available In App Products for sale
			PurchaseManager.QueryInventory(new string[] {
				"DropDistantCache",
				"NoExpiryDrop",
				"OpenDistantCache"
			});
#endif



			return true;
		}

		public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
		{
			return ApplicationDelegate.SharedInstance.OpenUrl(application, url, sourceApplication, annotation);
		}
		public override void OnResignActivation(UIApplication application)
		{
			// Invoked when the application is about to move from active to inactive state.
			// This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
			// or when the user quits the application and it begins the transition to the background state.
			// Games should use this method to pause the game.
		}

		public override void DidEnterBackground(UIApplication application)
		{
			// Use this method to release shared resources, save user data, invalidate timers and store the application state.
			// If your application supports background exection this method is called instead of WillTerminate when the user quits.
		}

		public override void WillEnterForeground(UIApplication application)
		{
			// Called as part of the transiton from background to active state.
			// Here you can undo many of the changes made on entering the background.
		}

		public override void OnActivated(UIApplication application)
		{
			// Restart any tasks that were paused (or not yet started) while the application was inactive. 
			// If the application was previously in the background, optionally refresh the user interface.
		}

		public override void WillTerminate(UIApplication application)
		{
			// Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
		}
	}
}

