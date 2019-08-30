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

namespace MyAggieNew
{
    [Activity(Label = "@string/app_name", MainLauncher = false, Icon = "@drawable/appicon", Theme = "@style/AppTheme")]
    public class MainContentActivity : AppCompatActivity
    {
        private SupportToolbar mToolbar;
        private ActionBarDrawerToggle mDrawerToggle;
        private DrawerLayout mDrawerLayout;
        private ListView mLeftDrawer;

        private DashboardFragment dashboardFragment;
        private MyFarmDashboardFragment myFarmDashboardFragment;
        private MyProfileFragment myProfileFragment;
        private AddActivityFragment addActivityFragment;
        private ActivityViewerFragment activityViewerFragment;
        private AskAggieFragment askAggieFragment;
        private SettingsFragment settingsFragment;
        private ContactFragment contactFragment;
        private ChatbotFragment chatbotFragment;
        private ChatListManagerFragment chatListManagerFragment;

        private Stack<SupportFragment> mStackFragments;

        private ArrayAdapter mLeftAdapter;
        private List<string> mLeftDataSet;

        DBaseOperations objdb;

        Refractored.Controls.CircleImageView circular_imageViewLogo;
        ImageView imageViewLogo;
        TextView textView;
        string _profilepicbase64 = string.Empty;

        public override void OnBackPressed()
        {
            this.RunOnUiThread(() =>
            {
                Android.App.AlertDialog.Builder alertDiag = new Android.App.AlertDialog.Builder(this);
                alertDiag.SetTitle(Resource.String.DialogHeaderGeneric);
                alertDiag.SetMessage(Resource.String.appLogoutMessage);
                alertDiag.SetIcon(Resource.Drawable.alert);
                alertDiag.SetPositiveButton(Resource.String.DialogButtonYes, (senderAlert, args) =>
                {
                    try
                    {
                        objdb = new DBaseOperations();
                        var lstu = objdb.selectTable();
                        if (lstu != null && lstu.Count > default(int))
                        {
                            var uobj = lstu.FirstOrDefault();
                            objdb.updateTable(new UserLoginInfo() { Id = uobj.Id, EmailId = uobj.EmailId, GoodName = uobj.GoodName, Password = " ", IsAdmin = uobj.IsAdmin, AuthToken = uobj.AuthToken, ProfilePicture = uobj.ProfilePicture });
                        }
                    }
                    catch { }

                    Intent intent = new Intent(this, typeof(MainActivity));
                    StartActivity(intent);
                    this.Finish();
                });
                alertDiag.SetNegativeButton(Resource.String.DialogButtonNo, (senderAlert, args) =>
                {

                });
                Dialog diag = alertDiag.Create();
                diag.Show();
                diag.SetCanceledOnTouchOutside(false);
            });
        }

        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                base.OnCreate(bundle);
                SetContentView(Resource.Layout.activity_maincontent);

                mToolbar = FindViewById<SupportToolbar>(Resource.Id.m_toolbar);
                mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.m_drawer_layout);
                mLeftDrawer = FindViewById<ListView>(Resource.Id.m_left_drawer);

                dashboardFragment = new DashboardFragment();
                myFarmDashboardFragment = new MyFarmDashboardFragment();
                myProfileFragment = new MyProfileFragment();
                addActivityFragment = new AddActivityFragment();
                activityViewerFragment = new ActivityViewerFragment();
                askAggieFragment = new AskAggieFragment();
                settingsFragment = new SettingsFragment();
                contactFragment = new ContactFragment();
                chatbotFragment = new ChatbotFragment();
                chatListManagerFragment = new ChatListManagerFragment();

                mStackFragments = new Stack<SupportFragment>();

                mLeftDrawer.Tag = 0;

                SetSupportActionBar(mToolbar);

                mLeftDataSet = new List<string>();
                mLeftDataSet.Add("My Dashboard");
                mLeftDataSet.Add("My Farm");
                mLeftDataSet.Add("My Profile");
                mLeftDataSet.Add("Add Activitiy");
                mLeftDataSet.Add("View Activities");
                /*mLeftDataSet.Add("Ask an Aggie");*/
                /*mLeftDataSet.Add("Settings");*/

                var objdbTemp = new DBaseOperations();
                var lstux = objdbTemp.selectTable();
                if (lstux != null && lstux.Count > default(int))
                {
                    var uobj = lstux.FirstOrDefault();
                    if (uobj != null && uobj.IsAdmin > default(int))
                    {
                        mLeftDataSet.Add("Chat Management");
                    }
                    else
                    {
                        mLeftDataSet.Add("Ask an Aggie");
                    }
                }
                else
                {
                    mLeftDataSet.Add("Ask an Aggie");
                }

                mLeftDataSet.Add("Contact Us");
                mLeftAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, mLeftDataSet);
                mLeftDrawer.Adapter = mLeftAdapter;
                mLeftDrawer.ItemClick += MenuListView_ItemClick;

                mDrawerToggle = new ActionBarDrawerToggle(this, mDrawerLayout, Resource.String.openDrawer, Resource.String.closeDrawer);

                mDrawerLayout.SetDrawerListener(mDrawerToggle);
                SupportActionBar.SetHomeButtonEnabled(true);
                SupportActionBar.SetDisplayShowTitleEnabled(true);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                mDrawerToggle.SyncState();

                if (bundle != null)
                {
                    if (bundle.GetString("DrawerState") == "Opened")
                    {
                        SupportActionBar.SetTitle(Resource.String.openDrawer);
                    }
                    else
                    {
                        SupportActionBar.SetTitle(Resource.String.closeDrawer);
                    }
                }
                else
                {
                    SupportActionBar.SetTitle(Resource.String.closeDrawer);
                }

                try
                {
                    string mStringLoginInfo = string.Empty;
                    string mStringSessionToken = string.Empty;
                    string mStringGoodName = string.Empty;
                    circular_imageViewLogo = this.FindViewById<Refractored.Controls.CircleImageView>(Resource.Id.circular_imageViewLogo);
                    imageViewLogo = this.FindViewById<ImageView>(Resource.Id.imageViewLogo);
                    textView = this.FindViewById<TextView>(Resource.Id.textView);
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
                            mStringGoodName = uobj.GoodName;
                            _profilepicbase64 = uobj.ProfilePicture;
                        }
                    }
                    catch { }

                    if (!string.IsNullOrEmpty(_profilepicbase64))
                    {
                        var btmpimg = BitmapHelpers.Base64ToBitmap(_profilepicbase64);
                        circular_imageViewLogo.Visibility = ViewStates.Visible;
                        imageViewLogo.Visibility = ViewStates.Gone;
                        textView.Text = string.Format("Hello, {0}", mStringGoodName);
                        circular_imageViewLogo.RecycleBitmap();
                        circular_imageViewLogo.SetImageBitmap(btmpimg);
                    }
                    else
                    {
                        circular_imageViewLogo.Visibility = ViewStates.Gone;
                        imageViewLogo.Visibility = ViewStates.Visible;
                        textView.Text = !string.IsNullOrEmpty(mStringGoodName) ? string.Format("Hello, {0}", mStringGoodName) : "";
                    }
                }
                catch { }

                IList<Android.Support.V4.App.Fragment> fragmentsarray = SupportFragmentManager.Fragments;
                if (fragmentsarray != null && fragmentsarray.Count > default(int))
                {
                    foreach (Android.Support.V4.App.Fragment fragment in fragmentsarray)
                    {
                        string tag = fragment.Tag;
                        Android.Support.V4.App.FragmentTransaction tx = SupportFragmentManager.BeginTransaction();
                        tx.Replace(Resource.Id.m_main, fragment, fragment.Tag);
                        tx.Commit();
                        break;
                    }
                }
                else
                {
                    Android.Support.V4.App.FragmentTransaction tx = SupportFragmentManager.BeginTransaction();
                    tx.Replace(Resource.Id.m_main, dashboardFragment, Constants.dashboard);
                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                this.RunOnUiThread(() =>
                {
                    Android.App.AlertDialog.Builder alertDiag = new Android.App.AlertDialog.Builder(this);
                    alertDiag.SetTitle(Resource.String.DialogHeaderError);
                    alertDiag.SetMessage(ex.Message);
                    alertDiag.SetIcon(Resource.Drawable.alert);
                    alertDiag.SetPositiveButton(Resource.String.DialogButtonOk, (senderAlert, args) =>
                    {
                        Intent intent = new Intent(this, typeof(MainActivity));
                        StartActivity(intent);
                        this.Finish();
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }

        void MenuListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            switch (e.Id)
            {
                case 0:
                    ShowFragment(dashboardFragment, Constants.dashboard);
                    break;
                case 1:
                    ShowFragment(myFarmDashboardFragment, Constants.myfarmdash);
                    break;
                case 2:
                    ShowFragment(myProfileFragment, Constants.myProfile);
                    break;
                case 3:
                    ShowFragment(addActivityFragment, Constants.addactivity);
                    break;
                case 4:
                    ShowFragment(activityViewerFragment, Constants.activityviewer);
                    break;
                /*case 5:
                    ShowFragment(askAggieFragment, Constants.askAggie);
                    break;*/
                /*case 6:
                    ShowFragment(settingsFragment, Constants.settings);
                    break;*/
                case 5:
                    {
                        var objdbTempx = new DBaseOperations();
                        var lstuxy = objdbTempx.selectTable();
                        if (lstuxy != null && lstuxy.Count > default(int))
                        {
                            var uobjx = lstuxy.FirstOrDefault();
                            if (uobjx != null && uobjx.IsAdmin > default(int))
                            {
                                ShowFragment(chatListManagerFragment, Constants.chatmanagerlist);
                            }
                            else
                            {
                                ShowFragment(chatbotFragment, Constants.chatbot);
                            }
                        }
                        else
                        {
                            ShowFragment(chatbotFragment, Constants.chatbot);
                        }
                        break;
                    }
                case 6:
                    ShowFragment(contactFragment, Constants.contactUs);
                    break;
            }

            mDrawerLayout.CloseDrawers();
            mDrawerToggle.SyncState();
        }

        private void ShowFragment(SupportFragment fragment, string tag)
        {
            if (fragment.IsVisible)
            {
                return;
            }
            Android.Support.V4.App.FragmentTransaction tx = SupportFragmentManager.BeginTransaction();
            tx.Replace(Resource.Id.m_main, fragment, (fragment.Tag == null ? tag : fragment.Tag));
            tx.Commit();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    {
                        mDrawerToggle.OnOptionsItemSelected(item);
                        return true;
                    }
                case Resource.Id.action_exit:
                    {
                        this.RunOnUiThread(() =>
                        {
                            Android.App.AlertDialog.Builder alertDiag = new Android.App.AlertDialog.Builder(this);
                            alertDiag.SetTitle(Resource.String.DialogHeaderGeneric);
                            alertDiag.SetMessage(Resource.String.appLogoutMessage);
                            alertDiag.SetIcon(Resource.Drawable.alert);
                            alertDiag.SetPositiveButton(Resource.String.DialogButtonYes, (senderAlert, args) =>
                            {
                                try
                                {
                                    objdb = new DBaseOperations();
                                    var lstu = objdb.selectTable();
                                    if (lstu != null && lstu.Count > default(int))
                                    {
                                        var uobj = lstu.FirstOrDefault();
                                        objdb.updateTable(new UserLoginInfo() { Id = uobj.Id, EmailId = uobj.EmailId, GoodName = uobj.GoodName, Password = " ", IsAdmin = uobj.IsAdmin, AuthToken = uobj.AuthToken, ProfilePicture = uobj.ProfilePicture });
                                    }
                                }
                                catch { }

                                Intent intent = new Intent(this, typeof(MainActivity));
                                StartActivity(intent);
                                this.Finish();
                            });
                            alertDiag.SetNegativeButton(Resource.String.DialogButtonNo, (senderAlert, args) =>
                            {

                            });
                            Dialog diag = alertDiag.Create();
                            diag.Show();
                            diag.SetCanceledOnTouchOutside(false);
                        });
                        return true;
                    }
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.action_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if (mDrawerLayout.IsDrawerOpen((int)GravityFlags.Left))
            {
                outState.PutString("DrawerState", "Opened");
            }
            else
            {
                outState.PutString("DrawerState", "Closed");
            }
            base.OnSaveInstanceState(outState);
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            mDrawerToggle.SyncState();
        }

        public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            mDrawerToggle.OnConfigurationChanged(newConfig);
        }
    }
}