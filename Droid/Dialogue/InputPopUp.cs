
using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace Drop.Droid
{
	public class InputPopUp : DialogFragment
	{
		Action<string> callback;
		string title;

		public static InputPopUp newInstance(string title, Action<string> callback)
		{
			InputPopUp inputDialog = new InputPopUp();

			inputDialog.callback = callback;
			inputDialog.title = title;
				
			return inputDialog;
		}

		public InputPopUp()
		{
			// Required empty public constructor
		}
		public override Dialog OnCreateDialog(Bundle savedInstanceState)
		{
			var dialog = base.OnCreateDialog(savedInstanceState);
			dialog.SetTitle(title);
			return dialog;
		}
		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			LinearLayout.LayoutParams params1 = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);

			LinearLayout linLayoutV = new LinearLayout(this.Activity);
			linLayoutV.Orientation = Orientation.Vertical;

			EditText input = new EditText(this.Activity);
			linLayoutV.AddView(input);

			Button okButton = new Button(this.Activity);
			okButton.Click += (sender, e) => {
				callback(input.Text);
				Dismiss();
			};

			params1.Gravity = GravityFlags.CenterHorizontal;
			okButton.LayoutParameters = params1;
			okButton.Text = "Done";

			linLayoutV.AddView(okButton);
			return linLayoutV;

		}
	}
}
