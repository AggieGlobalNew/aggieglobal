using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

namespace MyAggieNew
{
    public class ActivityByDateFragment : Android.Support.V4.App.Fragment
    {
        TextView txt_current_date;
        GridView androidGridView;
        DBaseOperations objdb;

        private string[] gridViewCategoryString;
        private string[] gridViewCategoryCodeString;
        private Bitmap[] gridCategoryViewImage;
        private string[] gridViewProductString;
        private string[] gridViewProductCodeString;
        private Bitmap[] gridProductViewImage;
        private string[] gridProductActivityFor;
        private string[] gridProductActivityCode;

        public static string dtstate;

        public ActivityByDateFragment() { }

        public static Android.Support.V4.App.Fragment newInstance(Context context)
        {
            ActivityByDateFragment busrouteFragment = new ActivityByDateFragment();
            return busrouteFragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ViewGroup root = (ViewGroup)inflater.Inflate(Resource.Layout.fragment_content_activitybydate, null);
            txt_current_date = root.FindViewById<TextView>(Resource.Id.txt_current_date_new);
            DateTime dtnow;
            var datestring = dtstate = Arguments.GetString("siteparamdate");
            dtnow = DateTime.ParseExact(datestring, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
            txt_current_date.Text = string.Format("{0} {1}, {2}", dtnow.ToString("MMMM", System.Globalization.CultureInfo.InvariantCulture), dtnow.Day.ToString(), dtnow.Year);

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
                var request = new RestRequest("Activity/GetActivityListByMonth", Method.GET);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("TokenKey", mStringSessionToken);
                request.AddQueryParameter("dateStamp", dtnow.ToString("yyyy-MM-dd"));
                IRestResponse response = client.Execute(request);
                var content = response.Content;
                var responseObj = Newtonsoft.Json.JsonConvert.DeserializeObject<IList<ActivityDetailResponse>>(content);
                if (responseObj != null && responseObj.Count() > default(int))
                {
                    var vCategoryString = responseObj.Select(r => r.CategoryName).ToList();
                    var vCategoryCodeString = responseObj.Select(r => r.CategoryID).ToList();
                    var strCategoryImgUrls = new List<string>();
                    IList<Bitmap> vCategoryViewImage = new List<Bitmap>();
                    if (vCategoryString != null && vCategoryString.Count() > default(int))
                    {
                        foreach (var v in vCategoryString)
                        {
                            strCategoryImgUrls.Add(string.Format("{0}.png", v));
                        }
                        vCategoryViewImage = BitmapHelpers.GetImageListFromUrlList(strCategoryImgUrls, mStringSessionToken, this.Activity.Resources);
                    }

                    var vProductString = responseObj.Select(r => r.ProductName).ToList();
                    var vProductCodeString = responseObj.Select(r => r.ProductId).ToList();
                    var strProductViewImage = new List<string>();
                    IList<Bitmap> vProductViewImage = new List<Bitmap>();
                    if (vProductString != null && vProductString.Count() > default(int))
                    {
                        foreach (var v in vProductString)
                        {
                            strProductViewImage.Add(string.Format("{0}.png", v));
                        }
                        vProductViewImage = BitmapHelpers.GetImageListFromUrlList(strProductViewImage, mStringSessionToken, this.Activity.Resources);
                    }

                    var vProductActivityFor = responseObj.Select(r => r.ActivityDescription).ToList();

                    var vProductActivityCode = responseObj.Select(r => r.ActivityId).ToList();

                    gridViewCategoryString = vCategoryString.ToArray();
                    gridViewCategoryCodeString = vCategoryCodeString.ToArray();
                    gridCategoryViewImage = vCategoryViewImage.ToArray();
                    gridViewProductString = vProductString.ToArray();
                    gridViewProductCodeString = vProductCodeString.ToArray();
                    gridProductViewImage = vProductViewImage.ToArray();
                    gridProductActivityFor = vProductActivityFor.ToArray();
                    gridProductActivityCode = vProductActivityCode.ToArray();
                }
                else
                {
                    //throw new Exception("No item found");

                    ActivityViewerFragment objFragment = new ActivityViewerFragment();
                    Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                    tx.Replace(Resource.Id.m_main, objFragment, Constants.activityviewer);
                    tx.Commit();
                }

                _activity_dayview_helper adapterViewAndroid = new _activity_dayview_helper(this.Activity, gridViewCategoryString, gridViewCategoryCodeString, gridCategoryViewImage, gridViewProductString, gridViewProductCodeString, gridProductViewImage, gridProductActivityFor, gridProductActivityCode);
                androidGridView = root.FindViewById<GridView>(Resource.Id.grid_view_adctivity_dayview);
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
                        ActivityViewerFragment objFragment = new ActivityViewerFragment();
                        Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                        tx.Replace(Resource.Id.m_main, objFragment, Constants.activityviewer);
                        tx.Commit();
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }

            return root;
        }

        private void ItemSearch_clicked(object sender, AdapterView.ItemClickEventArgs e, Activity currentActivity)
        {
            androidGridView.ItemClick -= (sndr, argus) => ItemSearch_clicked(sndr, argus, currentActivity);

            try
            {
                Bundle utilBundle = new Bundle();
                utilBundle.PutString("siteparam", gridProductActivityCode[e.Position]);
                utilBundle.PutString("siteparamdate", dtstate);
                ActivitydetailsFragment objFragment = new ActivitydetailsFragment();
                objFragment.Arguments = utilBundle;
                Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                tx.Replace(Resource.Id.m_main, objFragment, Constants.activitydetail);
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
    }
}