using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace Drop.Droid
{
	public class PurchasePopUp : DialogFragment
	{
		Constants.PURCHASE_TYPE mType;
		Action PurchaseCallBack;

		public static PurchasePopUp newInstance(Constants.PURCHASE_TYPE type, Action callBack = null)
		{
			PurchasePopUp pPopUp = new PurchasePopUp();

			pPopUp.mType = type;
			pPopUp.PurchaseCallBack = callBack;

			return pPopUp;
		}

		public PurchasePopUp()
		{
			// Required empty public constructor
		}
		public override Dialog OnCreateDialog(Bundle savedInstanceState)
		{
			var dialog = base.OnCreateDialog(savedInstanceState);
			dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
			return dialog;
		}
		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var infoView = LayoutInflater.From(this.Activity).Inflate(Resource.Layout.item_PurchasePopUp, null);

			infoView.FindViewById<TextView>(Resource.Id.lblTitle).Text = Constants.PURCHASE_TITLE[(int)mType];
			infoView.FindViewById<TextView>(Resource.Id.lblDescription1).Text = Constants.PURCHASE_DESCRIPTION1[(int)mType];
			infoView.FindViewById<TextView>(Resource.Id.lblDescription2).Text = Constants.PURCHASE_DESCRIPTION2[(int)mType];
			infoView.FindViewById<TextView>(Resource.Id.lblBtnTitle).Text = Constants.PURCHASE_BUTTON[(int)mType];
			
			infoView.FindViewById<LinearLayout>(Resource.Id.ActionClose).Click += (sender, e) => Dismiss();
			infoView.FindViewById<LinearLayout>(Resource.Id.ActionPurchase).Click += (sender, e) => ActionPurchase();

			return infoView;
		}

		void ActionPurchase()
		{
			PurchaseCallBack();
			Dismiss();
		}
	}
}
