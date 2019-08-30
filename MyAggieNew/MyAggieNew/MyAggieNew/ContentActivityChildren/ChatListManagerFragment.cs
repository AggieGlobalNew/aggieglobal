using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using System;
using System.Linq;
using System.Collections.Generic;
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Locations;
using Android;
using Android.Content.PM;
using Android.Support.V7.Widget;
using RestSharp;
using System.Threading;

namespace MyAggieNew
{
    public class ChatListManagerFragment : Android.Support.V4.App.Fragment
    {
        public ChatListManagerFragment() { }

        public static Android.Support.V4.App.Fragment newInstance(Context context)
        {
            ChatListManagerFragment busrouteFragment = new ChatListManagerFragment();
            return busrouteFragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ViewGroup root = (ViewGroup)inflater.Inflate(Resource.Layout.fragment_content_chatlist, null);

            this.Activity.RunOnUiThread(() =>
            {
                Android.App.AlertDialog.Builder alertDiag = new Android.App.AlertDialog.Builder(this.Activity);
                alertDiag.SetTitle(Resource.String.DialogHeaderError);
                alertDiag.SetMessage(string.Format("Firebase id 0be8c237-cca6-4895-8f9c-9bd026023709 is not registered with your device id {0}", Android.Provider.Settings.Secure.GetString(Android.App.Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId)));
                alertDiag.SetIcon(Resource.Drawable.alert);
                alertDiag.SetPositiveButton(Resource.String.DialogButtonOk, (senderAlert, args) =>
                {
                    MyFarmDashboardFragment objFragment = new MyFarmDashboardFragment();
                    Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                    tx.Replace(Resource.Id.m_main, objFragment, Constants.myfarmdash);
                    tx.Commit();
                });
                Dialog diag = alertDiag.Create();
                diag.Show();
                diag.SetCanceledOnTouchOutside(false);
            });

            return root;
        }
    }
}