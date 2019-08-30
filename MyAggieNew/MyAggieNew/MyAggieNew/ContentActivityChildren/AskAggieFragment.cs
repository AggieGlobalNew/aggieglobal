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

namespace MyAggieNew
{
    public class AskAggieFragment : Android.Support.V4.App.Fragment
    {
        TextView text_datenow, text_datetomorrow;
        Button btn_myaggiewelcome, btn_addactivity, btn_viewactivity;
        private SupportFragment mCurrentFragment = new SupportFragment();

        public AskAggieFragment() { }

        public static Android.Support.V4.App.Fragment newInstance(Context context)
        {
            AskAggieFragment busrouteFragment = new AskAggieFragment();
            return busrouteFragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ViewGroup root = (ViewGroup)inflater.Inflate(Resource.Layout.fragment_content_askaggie, null);
            return root;
        }
    }
}