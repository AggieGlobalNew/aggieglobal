using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using SupportFragment = Android.Support.V4.App.Fragment;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using System.Collections.Generic;
using Android.Text;

namespace MyAggieNew
{
    public class AppInfoFragment : Android.Support.V4.App.Fragment
    {
        Button btn_backtodashboard;
        TextView txt_welcome_note_0;
        private SupportFragment mCurrentFragment = new SupportFragment();

        public AppInfoFragment() { }

        public static Android.Support.V4.App.Fragment newInstance(Context context)
        {
            AppInfoFragment busrouteFragment = new AppInfoFragment();
            return busrouteFragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ViewGroup root = (ViewGroup)inflater.Inflate(Resource.Layout.fragment_content_appinfo, null);
            txt_welcome_note_0 = root.FindViewById<TextView>(Resource.Id.txt_welcome_note_0);
            txt_welcome_note_0.SetText(Resource.String.welcome_points1);
            //txt_welcome_note_0.Text = Html.FromHtml(GetString(Resource.String.welcome_points0)).ToString();
            btn_backtodashboard = root.FindViewById<Button>(Resource.Id.btn_backtodashboard);
            btn_backtodashboard.Click += (sndr, argus) => btn_backtodashboard_Click(sndr, argus, this.Activity);
            return root;
        }

        private void btn_backtodashboard_Click(object sender, EventArgs e, Activity currentActivity)
        {
            try
            {
                btn_backtodashboard.Click -= (sndr, argus) => btn_backtodashboard_Click(sndr, argus, currentActivity);

                DashboardFragment dashboardFragment = new DashboardFragment();
                Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                tx.Replace(Resource.Id.m_main, dashboardFragment, Constants.dashboard);
                mCurrentFragment = dashboardFragment;
                tx.Commit();
            }
            catch (Exception ex)
            {
                currentActivity.RunOnUiThread(() =>
                {
                    Android.App.AlertDialog.Builder alertDiag = new Android.App.AlertDialog.Builder(currentActivity);
                    alertDiag.SetTitle(Resource.String.DialogHeaderError);
                    alertDiag.SetMessage(ex.Message);
                    alertDiag.SetIcon(Resource.Drawable.alert);
                    alertDiag.SetPositiveButton(Resource.String.DialogButtonOk, (senderAlert, args) =>
                    {
                        btn_backtodashboard.Click += (sndr, argus) => btn_backtodashboard_Click(sndr, argus, currentActivity);
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }
    }
}