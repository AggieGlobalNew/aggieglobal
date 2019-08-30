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
using RestSharp;
using System.Collections.Generic;
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

namespace MyAggieNew
{
    public class MyFarmDashboardFragment : Android.Support.V4.App.Fragment
    {
        GridView androidGridView;
        string[] gridViewString;
        string[] gridViewCodeString;
        int[] gridViewImageId;

        Button btn_addnewplot, btn_viewexistingplot;
        TextView txt_FarmID, txt_FarmName, txt_FarmAddress, txt_FarmSize, txt_FarmEstablishedDate, txt_CoOpName;
        DBaseOperations objdb;

        public MyFarmDashboardFragment() { }

        public static Android.Support.V4.App.Fragment newInstance(Context context)
        {
            MyFarmDashboardFragment busrouteFragment = new MyFarmDashboardFragment();
            return busrouteFragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ViewGroup root = (ViewGroup)inflater.Inflate(Resource.Layout.fragment_content_myfarm, null);

            gridViewString = new string[] { "Add New Plot", "View Existing Plot(s)" };
            gridViewCodeString = new string[] { "AP", "VP" };
            gridViewImageId = new int[] { Resource.Drawable.add_plot, Resource.Drawable.view_plot };

            _generic_grid_menu_helper adapterViewAndroid = new _generic_grid_menu_helper(this.Activity, gridViewString, gridViewCodeString, gridViewImageId);
            androidGridView = root.FindViewById<GridView>(Resource.Id.grid_view_farm_dashitems_nw);
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

            btn_addnewplot = root.FindViewById<Button>(Resource.Id.btn_addnewplot);
            btn_viewexistingplot = root.FindViewById<Button>(Resource.Id.btn_viewexistingplot);
            btn_addnewplot.Click += (sndr, argus) => AddNewPlot(sndr, argus, this.Activity);
            btn_viewexistingplot.Click += (sndr, argus) => ViewAvailablePlots(sndr, argus, this.Activity);

            txt_FarmID = root.FindViewById<TextView>(Resource.Id.txt_FarmID);
            txt_FarmName = root.FindViewById<TextView>(Resource.Id.txt_FarmName);
            txt_FarmAddress = root.FindViewById<TextView>(Resource.Id.txt_FarmAddress);
            txt_FarmSize = root.FindViewById<TextView>(Resource.Id.txt_FarmSize);
            txt_FarmEstablishedDate = root.FindViewById<TextView>(Resource.Id.txt_FarmEstablishedDate);
            txt_CoOpName = root.FindViewById<TextView>(Resource.Id.txt_CoOpName);

            try
            {
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

                var client = new RestClient(Common.UrlBase);
                var request = new RestRequest("Farm/GetFarmsDetails", Method.GET);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("TokenKey", mStringSessionToken);
                IRestResponse response = client.Execute(request);
                var content = response.Content;
                var responseObj = Newtonsoft.Json.JsonConvert.DeserializeObject<FarmDetailResponse>(content);
                if (responseObj != null && !string.IsNullOrEmpty(responseObj.FarmName))
                {
                    txt_FarmID.Text = responseObj.FarmId;
                    txt_FarmSize.Text = string.Format("{0} {1}", Convert.ToString(responseObj.FarmSize), responseObj.FarmSizeUnit);
                    txt_FarmName.Text = responseObj.FarmName;
                    txt_FarmAddress.Text = responseObj.FarmAddress;
                    txt_CoOpName.Text = responseObj.CoOpName;
                    txt_FarmEstablishedDate.Text = responseObj.FarmEstablishedDate.ToString("MMMM-yy");
                }
                else
                {
                    MapFarmFragment objFragment = new MapFarmFragment();
                    Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                    tx.Replace(Resource.Id.m_main, objFragment, Constants.mapfarm);
                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                Intent intent = new Intent(this.Activity, typeof(DashboardFragment));
                StartActivity(intent);
                this.Activity.Finish();
            }

            return root;
        }

        private void ItemSearch_clicked(object sender, AdapterView.ItemClickEventArgs e, Activity currentActivity)
        {
            try
            {
                androidGridView.ItemClick -= (sndr, argus) => ItemSearch_clicked(sndr, argus, currentActivity);

                switch (gridViewCodeString[e.Position])
                {
                    case "AP":
                        {
                            AddEditPlotFragment obj = new AddEditPlotFragment();
                            Bundle utilBundle = new Bundle();
                            utilBundle.PutString("siteparam", txt_FarmID.Text);
                            obj.Arguments = utilBundle;
                            Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                            tx.Replace(Resource.Id.m_main, obj, Constants.addeditplot);
                            tx.Commit();
                            break;
                        }
                    case "VP":
                        {
                            ViewPlotListFragment obj = new ViewPlotListFragment();
                            Bundle utilBundle = new Bundle();
                            utilBundle.PutString("siteparam", txt_FarmID.Text);
                            obj.Arguments = utilBundle;
                            Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                            tx.Replace(Resource.Id.m_main, obj, Constants.viewplot);
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

        protected void AddNewPlot(object sender, EventArgs e, Activity currentActivity)
        {
            try
            {
                btn_addnewplot.Click -= (sndr, argus) => AddNewPlot(sndr, argus, currentActivity);

                AddEditPlotFragment obj = new AddEditPlotFragment();
                Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                tx.Replace(Resource.Id.m_main, obj, Constants.addeditplot);
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
                        //btn_addnewplot.Click += (sndr, argus) => AddNewPlot(sndr, argus, currentActivity);
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }

        protected void ViewAvailablePlots(object sender, EventArgs e, Activity currentActivity)
        {
            try
            {
                btn_viewexistingplot.Click -= (sndr, argus) => ViewAvailablePlots(sndr, argus, currentActivity);

                //throw new Exception("No plot found in our system against your farm");

                ViewPlotListFragment obj = new ViewPlotListFragment();
                Bundle utilBundle = new Bundle();
                utilBundle.PutString("siteparam", txt_FarmID.Text);
                obj.Arguments = utilBundle;
                Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                tx.Replace(Resource.Id.m_main, obj, Constants.viewplot);
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
                        //btn_viewexistingplot.Click += (sndr, argus) => ViewAvailablePlots(sndr, argus, currentActivity);
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }
    }
}