using System;
using System.Linq;
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
using RestSharp;
using System.Threading;

namespace MyAggieNew
{
    public class ViewPlotListFragment : Android.Support.V4.App.Fragment
    {
        GridView androidGridView;
        string[] gridViewString;
        string[] gridViewCodeString;
        string[] gridViewLetterImage;
        string farmid;

        DBaseOperations objdb;

        public ViewPlotListFragment() { }

        public static Android.Support.V4.App.Fragment newInstance(Context context)
        {
            AddEditPlotFragment busrouteFragment = new AddEditPlotFragment();
            return busrouteFragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ViewGroup root = (ViewGroup)inflater.Inflate(Resource.Layout.fragment_content_plotlist, null);

            try
            {
                farmid = Arguments.GetString("siteparam");
            }
            catch { }

            ProgressDialog progressDialog = ProgressDialog.Show(this.Activity, "Please wait...", "Fetching existing plots...", true);
            new Thread(new ThreadStart(delegate
            {
                this.Activity.RunOnUiThread(() => this.GetPlotsByUser(progressDialog, this.Activity, root, farmid));
            })).Start();

            return root;
        }

        private void ItemSearch_clicked(object sender, AdapterView.ItemClickEventArgs e, Activity currentActivity)
        {
            try
            {
                androidGridView.ItemClick -= (sndr, argus) => ItemSearch_clicked(sndr, argus, currentActivity);

                AddEditPlotFragment obj = new AddEditPlotFragment();
                Bundle utilBundle = new Bundle();
                utilBundle.PutString("siteparam", farmid);
                utilBundle.PutString("siteparam0", gridViewCodeString[e.Position]);
                obj.Arguments = utilBundle;
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
                        androidGridView.ItemClick += (sndr, argus) => ItemSearch_clicked(sndr, argus, currentActivity);
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }


        protected void GetPlotsByUser(ProgressDialog dialog, Activity curActivity, ViewGroup root, string frmid)
        {
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

                var client = new RestClient(Common.UrlBase);
                var request = new RestRequest("Plot/GetPlotListDetails", Method.GET);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("TokenKey", mStringSessionToken);
                request.AddQueryParameter("farmid", System.Net.WebUtility.UrlEncode(frmid));
                IRestResponse response = client.Execute(request);
                var content = response.Content;
                var responseObj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PlotDetailResponse>>(content);
                if (responseObj != null && responseObj.Count() > default(int))
                {
                    var newresponseObj = responseObj.OrderBy(o => o.PlotName);
                    if (newresponseObj != null && newresponseObj.Count() > default(int) && newresponseObj.FirstOrDefault() != null && !string.IsNullOrEmpty(newresponseObj.FirstOrDefault().PlotId))
                    {
                        gridViewString = newresponseObj.Select(s => s.PlotName).ToArray();
                        gridViewCodeString = newresponseObj.Select(p => p.PlotId).ToArray();
                        var sp = newresponseObj.Select(s => s.PlotName).ToArray();
                        var sd = new List<string>();
                        foreach (var s in sp)
                        {
                            sd.Add(s[0].ToString().ToUpper());
                        }
                        gridViewLetterImage = sd.ToArray();
                        _letter_grid_menu_helper adapterViewAndroid = new _letter_grid_menu_helper(this.Activity, gridViewString, gridViewCodeString, gridViewLetterImage);
                        androidGridView = root.FindViewById<GridView>(Resource.Id.grid_view_plot_list_l);
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
                    }
                    else
                    {
                        throw new Exception("No plot found in our system");
                    }
                }
            }
            catch (Exception ex)
            {
                curActivity.RunOnUiThread(() =>
                {
                    Android.App.AlertDialog.Builder alertDiag = new Android.App.AlertDialog.Builder(curActivity);
                    alertDiag.SetTitle(Resource.String.DialogHeaderError);
                    alertDiag.SetMessage(ex.Message);
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
            }
            finally
            {
                if (dialog != null && dialog.IsShowing)
                {
                    dialog.Hide();
                    dialog.Dismiss();
                }
            }
        }
    }
}