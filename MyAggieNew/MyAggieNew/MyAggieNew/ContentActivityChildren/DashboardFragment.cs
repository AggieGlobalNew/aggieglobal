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

namespace MyAggieNew
{
    public class DashboardFragment : Android.Support.V4.App.Fragment
    {
        TextView text_datenow, text_datetomorrow;
        GridView androidGridView;
        string[] gridViewString;
        string[] gridViewCodeString;
        int[] gridViewImageId;
        Button btn_myaggiewelcome, btn_addactivity, btn_viewactivity, btn_askaggie, btn_myfarmdash;
        private SupportFragment mCurrentFragment = new SupportFragment();

        DBaseOperations objdb;

        public DashboardFragment() { }

        public static Android.Support.V4.App.Fragment newInstance(Context context)
        {
            DashboardFragment busrouteFragment = new DashboardFragment();
            return busrouteFragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ViewGroup root = (ViewGroup)inflater.Inflate(Resource.Layout.fragment_content_dashboard, null);

            try
            {
                if (Build.VERSION.SdkInt >= Build.VERSION_CODES.M)
                {
                    if (Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.Camera) != (int)Permission.Granted
                        || Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.WriteExternalStorage) != (int)Permission.Granted
                        || Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.AccessFineLocation) != (int)Permission.Granted
                        || Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.AccessCoarseLocation) != (int)Permission.Granted
                        || Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.AccessWifiState) != (int)Permission.Granted
                        || Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.AccessNetworkState) != (int)Permission.Granted)
                    {
                        Android.Support.V4.App.ActivityCompat.RequestPermissions(this.Activity, new string[]
                        {
                            Manifest.Permission.Camera,
                            Manifest.Permission.WriteExternalStorage,
                            Manifest.Permission.ReadExternalStorage,
                            Manifest.Permission.AccessFineLocation,
                            Manifest.Permission.AccessCoarseLocation,
                            Manifest.Permission.AccessWifiState,
                            Manifest.Permission.AccessNetworkState
                        }, 54);
                        /*FragmentManager.FindFragmentById(Resource.Layout.fragment_content_addproduct).RequestPermissions(new string[]
                        {
                                Manifest.Permission.Camera, Manifest.Permission.WriteExternalStorage,
                                Manifest.Permission.ReadExternalStorage
                        }, 54);*/
                    }
                }
            }
            catch { }

            gridViewString = new string[] { "Welcome Note", "My Farm", "View Activities" };
            gridViewCodeString = new string[] { "WLC", "MYFRM", "VWACT" };
            gridViewImageId = new int[] { Resource.Drawable.welcome, Resource.Drawable.myfarmnew, Resource.Drawable.activity };

            _generic_grid_menu_helper adapterViewAndroid = new _generic_grid_menu_helper(this.Activity, gridViewString, gridViewCodeString, gridViewImageId);
            androidGridView = root.FindViewById<GridView>(Resource.Id.grid_view_dashitems);
            new System.Threading.Thread(new System.Threading.ThreadStart(() =>
            {
                if (this.Activity != null)
                {
                    this.Activity.RunOnUiThread(() =>
                    {
                        androidGridView.SetAdapter(adapterViewAndroid);
                    });
                }
            })).Start();
            androidGridView.ItemClick += (sndr, argus) => ItemSearch_clicked(sndr, argus, this.Activity);

            text_datenow = root.FindViewById<TextView>(Resource.Id.text_datenow);
            text_datenow.Text = DateTime.Now.ToString("dd-MM-yyyy");
            text_datetomorrow = root.FindViewById<TextView>(Resource.Id.text_datetomorrow);
            text_datetomorrow.Text = DateTime.Now.AddDays(1).ToString("dd-MM-yyyy");

            btn_myaggiewelcome = root.FindViewById<Button>(Resource.Id.btn_myaggiewelcome);
            btn_myaggiewelcome.Click += (sndr, argus) => btn_myaggiewelcome_Click(sndr, argus, this.Activity);

            btn_addactivity = root.FindViewById<Button>(Resource.Id.btn_addactivity);
            btn_addactivity.Click += (sndr, argus) => btn_addactivity_Click(sndr, argus, this.Activity);

            btn_viewactivity = root.FindViewById<Button>(Resource.Id.btn_viewactivity);
            btn_viewactivity.Click += (sndr, argus) => btn_viewactivity_Click(sndr, argus, this.Activity);

            btn_askaggie = root.FindViewById<Button>(Resource.Id.btn_askaggie);
            btn_askaggie.Click += (sndr, argus) => btn_askAnAggie_Click(sndr, argus, this.Activity);

            btn_myfarmdash = root.FindViewById<Button>(Resource.Id.btn_myfarmdash);
            btn_myfarmdash.Click += (sndr, argus) => NavigateToMyFarm(sndr, argus, this.Activity);

            try
            {
                /*ISharedPreferences prefs = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(this.Activity);
                string mStringLoginInfo = prefs.GetString(SessionConstants.LoginInfo, default(string));
                string mStringSessionToken = prefs.GetString(SessionConstants.SessionToken, default(string));*/

                string mStringLoginInfo = string.Empty;
                string mStringSessionToken = string.Empty;
                try
                {
                    objdb = new DBaseOperations();
                    var lstu = objdb.selectTable();
                    if (lstu != null && lstu.Count > default(int))
                    {
                        var uobj = lstu.FirstOrDefault();
                        if (uobj.Password == " ")
                        {
                            throw new Exception("Please login again");
                        }
                        mStringLoginInfo = uobj.EmailId;
                        mStringSessionToken = uobj.AuthToken;
                    }
                }
                catch { }

                if (string.IsNullOrEmpty(mStringSessionToken))
                {
                    throw new Exception("Token does not exists");
                }
            }
            catch (Exception ex)
            {
                this.Activity.RunOnUiThread(() =>
                {
                    Android.App.AlertDialog.Builder alertDiag = new Android.App.AlertDialog.Builder(this.Activity);
                    alertDiag.SetTitle(Resource.String.DialogHeaderError);
                    alertDiag.SetMessage(ex.Message);
                    alertDiag.SetIcon(Resource.Drawable.alert);
                    alertDiag.SetPositiveButton(Resource.String.DialogButtonOk, (senderAlert, args) =>
                    {
                        Intent intent = new Intent(this.Activity, typeof(MainActivity));
                        StartActivity(intent);
                        this.Activity.Finish();
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }

            try
            {
                this.GetCityByGeoLocation(this.Activity);
            }
            catch { }

            return root;
        }

        private void ItemSearch_clicked(object sender, AdapterView.ItemClickEventArgs e, Activity currentActivity)
        {
            try
            {
                androidGridView.ItemClick -= (sndr, argus) => ItemSearch_clicked(sndr, argus, currentActivity);

                switch (gridViewCodeString[e.Position])
                {
                    case "WLC":
                        {
                            AppInfoFragment appInfoFragment = new AppInfoFragment();
                            Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                            tx.Replace(Resource.Id.m_main, appInfoFragment, Constants.appinfo);
                            mCurrentFragment = appInfoFragment;
                            tx.Commit();
                            break;
                        }
                    case "MYFRM":
                        {
                            MyFarmDashboardFragment obj = new MyFarmDashboardFragment();
                            Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                            tx.Replace(Resource.Id.m_main, obj, Constants.myfarmdash);
                            mCurrentFragment = obj;
                            tx.Commit();
                            break;
                        }
                    case "VWACT":
                        {
                            ActivityViewerFragment activityViewerFragment = new ActivityViewerFragment();
                            Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                            tx.Replace(Resource.Id.m_main, activityViewerFragment, Constants.activityviewer);
                            mCurrentFragment = activityViewerFragment;
                            tx.Commit();
                            break;
                        }
                }
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
                        androidGridView.ItemClick += (sndr, argus) => ItemSearch_clicked(sndr, argus, currentActivity);
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }

        private void NavigateToMyFarm(object sender, EventArgs e, Activity currentActivity)
        {
            try
            {
                btn_myfarmdash.Click -= (sndr, argus) => NavigateToMyFarm(sndr, argus, currentActivity);

                MyFarmDashboardFragment obj = new MyFarmDashboardFragment();
                Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                tx.Replace(Resource.Id.m_main, obj, Constants.myfarmdash);
                mCurrentFragment = obj;
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

                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }

        private void btn_askAnAggie_Click(object sender, EventArgs e, Activity currentActivity)
        {
            try
            {
                btn_askaggie.Click -= (sndr, argus) => btn_askAnAggie_Click(sndr, argus, currentActivity);

                AskAggieFragment askAggieFragment = new AskAggieFragment();
                Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                tx.Replace(Resource.Id.m_main, askAggieFragment, Constants.addactivity);
                mCurrentFragment = askAggieFragment;
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
                        btn_askaggie.Click += (sndr, argus) => btn_askAnAggie_Click(sndr, argus, currentActivity);
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }

        private void btn_addactivity_Click(object sender, EventArgs e, Activity currentActivity)
        {
            try
            {
                btn_addactivity.Click -= (sndr, argus) => btn_addactivity_Click(sndr, argus, currentActivity);

                AddActivityFragment addActivityFragment = new AddActivityFragment();
                Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                tx.Replace(Resource.Id.m_main, addActivityFragment, Constants.addactivity);
                mCurrentFragment = addActivityFragment;
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
                        btn_addactivity.Click += (sndr, argus) => btn_addactivity_Click(sndr, argus, currentActivity);
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }

        private void btn_viewactivity_Click(object sender, EventArgs e, Activity currentActivity)
        {
            try
            {
                btn_viewactivity.Click -= (sndr, argus) => btn_viewactivity_Click(sndr, argus, currentActivity);

                ActivityViewerFragment activityViewerFragment = new ActivityViewerFragment();
                Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                tx.Replace(Resource.Id.m_main, activityViewerFragment, Constants.activityviewer);
                mCurrentFragment = activityViewerFragment;
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
                        btn_viewactivity.Click += (sndr, argus) => btn_viewactivity_Click(sndr, argus, currentActivity);
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }

        private void btn_myaggiewelcome_Click(object sender, EventArgs e, Activity currentActivity)
        {
            try
            {
                btn_myaggiewelcome.Click -= (sndr, argus) => btn_myaggiewelcome_Click(sndr, argus, currentActivity);

                AppInfoFragment appInfoFragment = new AppInfoFragment();
                Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                //tx.Add(Resource.Id.m_main, appInfoFragment);
                tx.Replace(Resource.Id.m_main, appInfoFragment, Constants.appinfo);
                mCurrentFragment = appInfoFragment;
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
                        btn_myaggiewelcome.Click += (sndr, argus) => btn_myaggiewelcome_Click(sndr, argus, currentActivity);
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }

        private void GetCityByGeoLocation(Context cntxt)
        {
            try
            {
                Geocoder coder = new Geocoder(cntxt, Java.Util.Locale.English);
                var results = coder.GetFromLocation(-17.713371, 178.065033, 10);
                var addr = results.FirstOrDefault();
                if (addr != null)
                {
                    string city = !string.IsNullOrEmpty(addr.Locality) ? addr.Locality : addr.FeatureName;
                }
            }
            catch { }
        }

        private void GetWeather()
        {
            //https://weather.cit.api.here.com/weather/1.0/report.json?product=observation&latitude=12.912&longitude=77.642&oneobservation=true&app_id=30QDUhRcq7xBEAMhfgV9&app_code=x6OatHkDnZ8ns10T-dJR1g
        }

        private void LatLongFinder()
        {

        }
    }
}