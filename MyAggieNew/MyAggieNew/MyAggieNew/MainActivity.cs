using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

namespace MyAggieNew
{
    [Activity(Label = "@string/app_name", MainLauncher = true, Icon = "@drawable/appicon", Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity
    {
        private SupportToolbar mToolbar;
        private ActionBarDrawerToggle mDrawerToggle;
        private DrawerLayout mDrawerLayout;
        private ListView mLeftDrawer;
        private LoginFragment loginFragment;
        private RegistrationFragment registrationFragment;
        private Stack<SupportFragment> mStackFragments;

        private ArrayAdapter mLeftAdapter;
        private List<string> mLeftDataSet;

        public override void OnBackPressed()
        {
            this.RunOnUiThread(() =>
            {
                Android.App.AlertDialog.Builder alertDiag = new Android.App.AlertDialog.Builder(this);
                alertDiag.SetTitle(Resource.String.DialogHeaderGeneric);
                alertDiag.SetMessage(Resource.String.exitAppMessage);
                alertDiag.SetIcon(Resource.Drawable.alert);
                alertDiag.SetPositiveButton(Resource.String.DialogButtonYes, (senderAlert, args) =>
                {
                    this.Finish();
                    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                    this.OnBackPressed();
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
                SetContentView(Resource.Layout.activity_main);

                //var x = MakeAsyncRequest("http://116.193.133.150/publicweb/Helpdesk_API/Login/GetDefault", "application/json");
                //var x = SampleAPICall();
                //throw new Exception(x.Result.ToString());
                /*var x = SampleAPICall();
                var y = x.Result.ReadAsStringAsync();*/

                mToolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
                mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
                mLeftDrawer = FindViewById<ListView>(Resource.Id.left_drawer);

                loginFragment = new LoginFragment();
                registrationFragment = new RegistrationFragment();

                mStackFragments = new Stack<SupportFragment>();

                mLeftDrawer.Tag = 0;

                SetSupportActionBar(mToolbar);

                mLeftDataSet = new List<string>();
                mLeftDataSet.Add("Login");
                mLeftDataSet.Add("Register");
                mLeftAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, mLeftDataSet);
                mLeftDrawer.Adapter = mLeftAdapter;
                mLeftDrawer.ItemClick += MenuListView_ItemClick;

                /*try
                {
                    for (int i = 0; i < mLeftDrawer.Adapter.Count; i++)
                    {
                        var child = mLeftDrawer.Adapter.GetView(i, null, mLeftDrawer);
                        //((TextView)child).SetLinkTextColor(Resources.GetColorStateList(Resource.Color.colorWhite));
                        //((TextView)child).SetTextColor(Android.Graphics.Color.White);
                    }
                }
                catch { }*/

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

                IList<Android.Support.V4.App.Fragment> fragmentsarray = SupportFragmentManager.Fragments;
                if (fragmentsarray != null && fragmentsarray.Count > default(int))
                {
                    foreach (Android.Support.V4.App.Fragment fragment in fragmentsarray)
                    {
                        string tag = fragment.Tag;
                        Android.Support.V4.App.FragmentTransaction tx = SupportFragmentManager.BeginTransaction();
                        tx.Replace(Resource.Id.main, fragment, fragment.Tag);
                        tx.Commit();
                        break;
                    }
                }
                else
                {
                    Android.Support.V4.App.FragmentTransaction tx = SupportFragmentManager.BeginTransaction();
                    tx.Replace(Resource.Id.main, loginFragment, Constants.login);
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
                    ShowFragment(loginFragment, Constants.login);
                    break;
                case 1:
                    ShowFragment(registrationFragment, Constants.registration);
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
            tx.Replace(Resource.Id.main, fragment, (fragment.Tag == null ? tag : fragment.Tag));
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
                            alertDiag.SetMessage(Resource.String.exitAppMessage);
                            alertDiag.SetIcon(Resource.Drawable.alert);
                            alertDiag.SetPositiveButton(Resource.String.DialogButtonYes, (senderAlert, args) =>
                            {
                                this.Finish();
                                Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                                this.OnBackPressed();
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

        /*public async Task<IRestResponse<string>> SampleAPICall()
        {
            using (var client = new RestClient(new Uri("http://116.193.133.150/publicweb/Helpdesk_API/")))
            {
                var request = new RestRequest("Login/GetDefault", Method.GET);
                var result = await client.Execute<string>(request);
                return result;
            }
        }*/

        /*public Task<string> MakeAsyncRequest(string url, string contentType)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = contentType;
            request.Method = WebRequestMethods.Http.Get;
            request.Timeout = 20000;
            request.Proxy = null;

            Task<WebResponse> task = Task.Factory.FromAsync(request.BeginGetResponse, asyncResult => request.EndGetResponse(asyncResult), (object)null);

            return task.ContinueWith(t => ReadStreamFromResponse(t.Result));
        }

        private string ReadStreamFromResponse(WebResponse response)
        {
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader sr = new StreamReader(responseStream))
            {
                string strContent = sr.ReadToEnd();
                return strContent;
            }
        }*/

        /*private async Task<HttpContent> SampleAPICall()
        {
            HttpClient client = new HttpClient();
            var uri = new Uri("http://116.193.133.150/publicweb/Helpdesk_API/");
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response;
            response = await client.GetAsync(uri);
            return response.Content;
        }*/
    }
}