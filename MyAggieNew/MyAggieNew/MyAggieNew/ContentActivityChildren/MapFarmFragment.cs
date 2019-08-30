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
using RestSharp;
using System.Threading;
using System.Linq;

namespace MyAggieNew
{
    public class MapFarmFragment : Android.Support.V4.App.Fragment
    {
        Spinner spinnerx0;
        Button btn_map_farm;

        string spinnerx0txt;
        string farmId = string.Empty;
        IList<FarmDetailResponse> arrFields;

        DBaseOperations objdb;

        public MapFarmFragment() { }

        public static Android.Support.V4.App.Fragment newInstance(Context context)
        {
            RegistrationFragment busrouteFragment = new RegistrationFragment();
            return busrouteFragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ViewGroup root = (ViewGroup)inflater.Inflate(Resource.Layout._fragment_content_selectfarm, null);
            try
            {
                var client = new RestClient(Common.UrlBase);
                var request = new RestRequest("Farm/GetAllFarmsDetails", Method.GET);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("IsRegister", "true");
                IRestResponse response = client.Execute(request);
                var content = response.Content;
                arrFields = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FarmDetailResponse>>(content);

                spinnerx0 = root.FindViewById<Spinner>(Resource.Id.spinnerx0);
                spinnerx0.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinnerx0_ItemSelected);
                var x = arrFields.Where(a => a.Status == ResponseStatus.Successful).ToList();
                if (x != null && x.Count() > default(int))
                {
                    spinnerx0.Adapter = new ArrayAdapter<string>(this.Activity, Android.Resource.Layout.SimpleSpinnerItem, x.Select(a => a.FarmName).ToList());
                }
                else
                {
                    throw new Exception("No farm data found");
                }

                btn_map_farm = root.FindViewById<Button>(Resource.Id.btn_map_farm);
                btn_map_farm.Click += (sndr, argus) => AddFarm_Clicked(sndr, argus, this.Activity);
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
                        MyFarmDashboardFragment obj = new MyFarmDashboardFragment();
                        Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                        tx.Replace(Resource.Id.m_main, obj, Constants.myfarmdash);
                        tx.Commit();
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
            return root;
        }

        private void Spinnerx0_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            spinnerx0txt = Convert.ToString(spinner.GetItemAtPosition(e.Position));
        }

        private void AddFarm_Clicked(object sender, EventArgs e, Activity currentActivity)
        {
            try
            {
                btn_map_farm.Click -= (sndr, argus) => AddFarm_Clicked(sndr, argus, currentActivity);

                if (!string.IsNullOrWhiteSpace(spinnerx0txt))
                {
                    currentActivity.RunOnUiThread(() =>
                    {
                        Android.App.AlertDialog.Builder alertDiag = new Android.App.AlertDialog.Builder(currentActivity);
                        alertDiag.SetTitle(Resource.String.DialogHeaderGeneric);
                        alertDiag.SetMessage(string.Format("Are you sure to add '{0}' as your farm?", spinnerx0txt));
                        alertDiag.SetIcon(Resource.Drawable.alert);
                        alertDiag.SetPositiveButton(Resource.String.DialogButtonYes, (senderAlert, args) =>
                        {
                            ProgressDialog progressDialog = ProgressDialog.Show(this.Activity, "Please wait...", "Checking account info...", true);
                            new Thread(new ThreadStart(delegate
                            {
                                currentActivity.RunOnUiThread(() => this.MapFarmToUserCall(progressDialog, currentActivity));
                            })).Start();
                        });
                        alertDiag.SetNegativeButton(Resource.String.DialogButtonNo, (senderAlert, args) =>
                        {

                        });
                        Dialog diag = alertDiag.Create();
                        diag.Show();
                        diag.SetCanceledOnTouchOutside(false);
                    });
                }
                else
                {
                    throw new Exception("Please select your farm to proceed");
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
                        //btn_map_farm.Click += (sndr, argus) => AddFarm_Clicked(sndr, argus, currentActivity);
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }

        private void MapFarmToUserCall(ProgressDialog dialog, Activity curActivity)
        {
            try
            {
                farmId = arrFields.Where(x => x.FarmName == spinnerx0txt).Select(f => f.FarmId).FirstOrDefault();

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
                var request = new RestRequest("Farm/MapFarmByUserDetail", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("TokenKey", mStringSessionToken);
                request.AddQueryParameter("FarmId", farmId);
                IRestResponse response = client.Execute(request);
                var content = response.Content;
                var responseObj = Newtonsoft.Json.JsonConvert.DeserializeObject<FarmDetailResponse>(content);
                if (responseObj.Status == ResponseStatus.Successful)
                {
                    MyFarmDashboardFragment objFragment = new MyFarmDashboardFragment();
                    Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                    tx.Replace(Resource.Id.m_main, objFragment, Constants.myfarmdash);
                    tx.Commit();
                }
                else
                {
                    throw new Exception(responseObj.Error);
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