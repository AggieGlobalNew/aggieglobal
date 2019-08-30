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
    public class AddEditPlotFragment : Android.Support.V4.App.Fragment
    {
        Button btn_add_new_plot, btn_cancel_plot;
        EditText input_plot_name, input_plot_size, input_plot_notes;
        Spinner spinner_soilph, spinner_soiltype;
        Switch isorganic_switch;

        bool isorganic;
        string farmid, plotid, spinnersoilphtxt, spinnersoiltypetxt;
        SoilDataResponse soilMstrBasu;

        DBaseOperations objdb;

        public AddEditPlotFragment() { }

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
            ViewGroup root = (ViewGroup)inflater.Inflate(Resource.Layout.fragment_content_plotaddedit, null);

            farmid = Arguments.GetString("siteparam");
            plotid = Arguments.GetString("siteparam0");
            soilMstrBasu = this.GetSoilMasterDtl(farmid);

            btn_add_new_plot = root.FindViewById<Button>(Resource.Id.btn_add_new_plot);
            btn_cancel_plot = root.FindViewById<Button>(Resource.Id.btn_cancel_plot);
            btn_add_new_plot.Click += (sndr, argus) => AddPlot_Clicked(sndr, argus, this.Activity);
            btn_cancel_plot.Click += (sndr, argus) => Cancel_Selection(sndr, argus, this.Activity);

            input_plot_name = root.FindViewById<EditText>(Resource.Id.input_plot_name);
            input_plot_size = root.FindViewById<EditText>(Resource.Id.input_plot_size);
            input_plot_notes = root.FindViewById<EditText>(Resource.Id.input_plot_notes);

            spinner_soilph = root.FindViewById<Spinner>(Resource.Id.spinner_soilph);
            spinner_soiltype = root.FindViewById<Spinner>(Resource.Id.spinner_soiltype);
            spinner_soilph.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_soilph_ItemSelected);
            spinner_soiltype.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_soiltype_ItemSelected);
            List<string> lstPh = new List<string>();
            lstPh.Add("Unknown");
            lstPh.AddRange(soilMstrBasu.soilphdetail.Select(p => p.SoilPhvalue.ToString()).ToList());
            spinner_soilph.Adapter = new ArrayAdapter<string>(this.Activity, Android.Resource.Layout.SimpleSpinnerItem, lstPh);
            List<string> lsttype = new List<string>();
            lsttype.Add("Unknown");
            lsttype.AddRange(soilMstrBasu.soildetail.Select(s => s.SoilName).ToList());
            spinner_soiltype.Adapter = new ArrayAdapter<string>(this.Activity, Android.Resource.Layout.SimpleSpinnerItem, lsttype);

            isorganic_switch = root.FindViewById<Switch>(Resource.Id.isorganic_switch);
            isorganic_switch.CheckedChange += delegate (object sender, CompoundButton.CheckedChangeEventArgs e)
            {
                isorganic = e.IsChecked;
                /*var toast = Toast.MakeText(this.Activity, (e.IsChecked ? "Admin mode activated. Login with administrative credential" : "Admin mode de-activated. Login with non-admin user credential"),
                    ToastLength.Long);
                toast.Show();*/
            };

            if (!string.IsNullOrEmpty(plotid))
            {
                ProgressDialog progressDialog = ProgressDialog.Show(this.Activity, "Please wait...", "Fetching plot details...", true);
                new Thread(new ThreadStart(delegate
                {
                    this.Activity.RunOnUiThread(() => this.GetPlotById(progressDialog, this.Activity, plotid, soilMstrBasu));
                })).Start();
            }

            return root;
        }

        private void spinner_soilph_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            spinnersoilphtxt = Convert.ToString(spinner.GetItemAtPosition(e.Position));
        }

        private void spinner_soiltype_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            spinnersoiltypetxt = Convert.ToString(spinner.GetItemAtPosition(e.Position));
        }

        protected void AddPlot_Clicked(object sender, EventArgs e, Activity currentActivity)
        {
            try
            {
                btn_add_new_plot.Click -= (sndr, argus) => AddPlot_Clicked(sndr, argus, currentActivity);

                if (string.IsNullOrEmpty(input_plot_name.Text.Trim()) || string.IsNullOrEmpty(input_plot_size.Text.Trim()))
                {
                    throw new Exception("Please do not leave any mandatory field blank");
                }
                if (string.IsNullOrWhiteSpace(spinnersoilphtxt) || string.IsNullOrWhiteSpace(spinnersoiltypetxt))
                {
                    throw new Exception("Please select Soil Ph and Soil Type");
                }

                string plotid = string.Empty;
                try
                {
                    plotid = Arguments.GetString("siteparam0");
                }
                catch { }

                currentActivity.RunOnUiThread(() =>
                {
                    Android.App.AlertDialog.Builder alertDiag = new Android.App.AlertDialog.Builder(currentActivity);
                    alertDiag.SetTitle(Resource.String.DialogHeaderGeneric);
                    alertDiag.SetMessage(string.Format("Are you sure you want to {0} '{1}' plot?", (!string.IsNullOrEmpty(plotid) ? "update" : "save"), input_plot_name.Text.Trim()));
                    alertDiag.SetIcon(Resource.Drawable.alert);
                    alertDiag.SetPositiveButton(Resource.String.DialogButtonYes, (senderAlert, args) =>
                    {
                        PlotDetailResponse obj = new PlotDetailResponse();
                        obj.PlotId = plotid;
                        obj.FarmId = Arguments.GetString("siteparam");
                        obj.Notes = input_plot_notes.Text;
                        obj.Organic = isorganic;
                        obj.PlotName = input_plot_name.Text.Trim();
                        decimal plSize = default(decimal);
                        decimal.TryParse(input_plot_size.Text.Trim(), out plSize);
                        obj.PlotSize = plSize;
                        if (soilMstrBasu.soildetail.Where(s => s.SoilName == spinnersoiltypetxt).FirstOrDefault() != null)
                        {
                            obj.SoilId = soilMstrBasu.soildetail.Where(s => s.SoilName == spinnersoiltypetxt).ToList().FirstOrDefault().SoilId;
                        }
                        else
                        {
                            obj.SoilId = string.Empty;
                        }
                        if (soilMstrBasu.soilphdetail.Where(s => s.SoilPhvalue.ToString() == spinnersoilphtxt).FirstOrDefault() != null)
                        {
                            obj.SoilPhId = soilMstrBasu.soilphdetail.Where(s => s.SoilPhvalue == Convert.ToInt32(spinnersoilphtxt)).ToList().FirstOrDefault().SoilPhId;
                        }
                        else
                        {
                            obj.SoilPhId = string.Empty;
                        }
                        ProgressDialog progressDialog = ProgressDialog.Show(this.Activity, "Please wait...", "Submitting your plot data...", true);
                        new Thread(new ThreadStart(delegate
                        {
                            this.Activity.RunOnUiThread(() => this.SavePlot(progressDialog, this.Activity, obj));
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
                        //btn_add_new_plot.Click += (sndr, argus) => AddPlot_Clicked(sndr, argus, currentActivity);
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }

        protected void Cancel_Selection(object sender, EventArgs e, Activity currentActivity)
        {
            try
            {
                btn_cancel_plot.Click -= (sndr, argus) => Cancel_Selection(sndr, argus, currentActivity);

                currentActivity.RunOnUiThread(() =>
                {
                    Android.App.AlertDialog.Builder alertDiag = new Android.App.AlertDialog.Builder(currentActivity);
                    alertDiag.SetTitle(Resource.String.DialogHeaderGeneric);
                    alertDiag.SetMessage(Resource.String.cancelFromAddItemMessage);
                    alertDiag.SetIcon(Resource.Drawable.alert);
                    alertDiag.SetPositiveButton(Resource.String.DialogButtonYes, (senderAlert, args) =>
                    {
                        MyFarmDashboardFragment objFragment = new MyFarmDashboardFragment();
                        Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                        tx.Replace(Resource.Id.m_main, objFragment, Constants.myfarmdash);
                        tx.Commit();
                    });
                    alertDiag.SetNegativeButton(Resource.String.DialogButtonNo, (senderAlert, args) =>
                    {
                        //btn_cancel_plot.Click += (sndr, argus) => Cancel_Selection(sndr, argus, currentActivity);
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
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
                        //btn_cancel_plot.Click += (sndr, argus) => Cancel_Selection(sndr, argus, currentActivity);
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }

        protected void SavePlot(ProgressDialog dialog, Activity curActivity, PlotDetailResponse obj)
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
                var request = new RestRequest("Plot/CreateUpdatePlot", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("TokenKey", mStringSessionToken);
                request.AddJsonBody(new { PlotName = obj.PlotName, PlotSize = obj.PlotSize, Organic = obj.Organic, SoilPhId = obj.SoilPhId, SoilId = obj.SoilId, Notes = obj.Notes, FarmId = obj.FarmId, PlotId = obj.PlotId });
                IRestResponse response = client.Execute(request);
                var content = response.Content;
                var responseObj = Newtonsoft.Json.JsonConvert.DeserializeObject<PlotDetailResponse>(content);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (responseObj.Status == ResponseStatus.Successful)
                    {
                        curActivity.RunOnUiThread(() =>
                        {
                            Android.App.AlertDialog.Builder alertDiag = new Android.App.AlertDialog.Builder(curActivity);
                            alertDiag.SetTitle(Resource.String.DialogHeaderGeneric);
                            alertDiag.SetMessage(string.Format("'{0}' plot has been {1} successfully", obj.PlotName, (string.IsNullOrEmpty(obj.PlotId) ? "saved" : "updated")));
                            alertDiag.SetIcon(Resource.Drawable.success);
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
                    else
                    {
                        if (!string.IsNullOrEmpty(responseObj.Error))
                        {
                            throw new Exception(responseObj.Error);
                        }
                        else
                        {
                            throw new Exception("Unable to save data now. Please try again later");
                        }
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

        private SoilDataResponse GetSoilMasterDtl(string farmid)
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
                var request = new RestRequest("Plot/GetSoilDetails", Method.GET);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("TokenKey", mStringSessionToken);
                IRestResponse response = client.Execute(request);
                var content = response.Content;
                var responseObj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SoilDataResponse>>(content);

                if (responseObj != null && responseObj.Count() > default(int))
                {
                    var soilComplexObj = responseObj.FirstOrDefault();
                    if (soilComplexObj != null && soilComplexObj.soildetail != null && soilComplexObj.soildetail.Count() > default(int) && soilComplexObj.soilphdetail != null && soilComplexObj.soilphdetail.Count() > default(int))
                    {
                        /*var lstSoilPh = new List<SoilPhDetailResponse>();
                        lstSoilPh.Add(new SoilPhDetailResponse() { SoilPhId = string.Empty, SoilPhvalue = -1, Status = ResponseStatus.Successful, Error = string.Empty });
                        lstSoilPh.AddRange(soilComplexObj.soilphdetail);
                        soilComplexObj.soilphdetail = null;
                        soilComplexObj.soilphdetail = lstSoilPh;*/
                        return soilComplexObj;
                    }
                    else
                    {
                        return new SoilDataResponse();
                    }
                }
                else
                {
                    return new SoilDataResponse();
                }
            }
            catch (Exception)
            {
                return new SoilDataResponse();
            }
        }

        private void GetPlotById(ProgressDialog dialog, Activity curActivity, string plotid, SoilDataResponse sm)
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
                var request = new RestRequest("Plot/GetPlotDetailsById", Method.GET);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("TokenKey", mStringSessionToken);
                request.AddQueryParameter("PlotId", System.Net.WebUtility.UrlEncode(plotid));
                IRestResponse response = client.Execute(request);
                var content = response.Content;

                var responseObj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PlotDetailResponse>>(content);
                if (responseObj != null && responseObj.Count() > default(int))
                {
                    var obj = responseObj.FirstOrDefault();
                    obj.PlotId = plotid;
                    input_plot_name.Text = obj.PlotName;
                    input_plot_size.Text = obj.PlotSize.ToString();
                    input_plot_notes.Text = obj.Notes;
                    isorganic_switch.Checked = obj.Organic;

                    ArrayAdapter adap = (ArrayAdapter)spinner_soilph.Adapter;
                    int a = default(int);
                    if (sm.soilphdetail.Where(p => p.SoilPhId == obj.SoilPhId).FirstOrDefault() != null)
                    {
                        a = adap.GetPosition(sm.soilphdetail.Where(p => p.SoilPhId == obj.SoilPhId).Select(s => s.SoilPhvalue).FirstOrDefault().ToString());
                    }
                    spinner_soilph.SetSelection(a);

                    ArrayAdapter adap_ = (ArrayAdapter)spinner_soiltype.Adapter;
                    int a_ = default(int);
                    if (sm.soildetail.Where(p => p.SoilId == obj.SoilId).FirstOrDefault() != null)
                    {
                        a_ = adap_.GetPosition(sm.soildetail.Where(p => p.SoilId == obj.SoilId).Select(s => s.SoilName).FirstOrDefault().ToString());
                    }
                    spinner_soiltype.SetSelection(a_);

                    btn_add_new_plot.Text = "Update";
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