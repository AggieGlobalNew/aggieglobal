using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RestSharp;

namespace MyAggieNew
{
    public class ActivitydetailsFragment : Android.Support.V4.App.Fragment
    {
        TextView txtdtl_ActivityDate, txtdtl_PlotName, txtdtl_ActivityDescription, txtdtl_ProductTypeName,
            txtdtl_product_name, txtdtl_LastHarvestedDate, txtdtl_HarvestedBefore, txtdtl_SoldBefore, txtdtl_SoldBeforeNoReason,
            txtdtl_SoldPrice, txtdtl_PlantationDate, txtdtl_TotalNumberOfResource, txtdtl_ResourceCostType, txtdtl_ResourcePrice,
            txtdtl_NumberOfProduct, txtdtl_LiveStockUsageName, txtdtl_LiveStockUtilityName, txtdtl_IsLivestockSalable, txtdtl_LastDateOfLivestockSold,
            txtdtl_SoldLiveStockAmount, txtdtl_LivestocksellingLocationName, txtdtl_LiveStockName, txtdtl_ResourceTypeName, txtdtl_ResourceCostTypeName,
            txtdtl_SoldLiveStockProductName, txtdtl_ResourceMaintenanceCostTypeName, txtdtl_ResourceMaintenancePrice;

        Button btn_back, btn_delete, btn_activity_updaterecord;

        public string ActivityID = string.Empty;
        string mStringSessionToken = string.Empty;

        public static string selecteddate;

        IList<ItemPayloadModelWithBase64> objSelectedItem;

        public ActivitydetailsFragment() { }

        public static Android.Support.V4.App.Fragment newInstance(Context context)
        {
            ActivitydetailsFragment busrouteFragment = new ActivitydetailsFragment();
            return busrouteFragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ViewGroup root = null;
            try
            {
                DBaseOperations objdb;
                objdb = new DBaseOperations();
                var lstu = objdb.selectTable();
                if (lstu != null && lstu.Count > default(int))
                {
                    var uobj = lstu[0];
                    if (uobj.Password == " ")
                    {
                        throw new Exception("Please login again");
                    }
                    mStringSessionToken = uobj.AuthToken;
                }

                root = (ViewGroup)inflater.Inflate(Resource.Layout.fragment_content_activitydetails, null);

                if (Arguments != null)
                {
                    ActivityID = Arguments.GetString("siteparam");
                    selecteddate = Arguments.GetString("siteparamdate");
                }

                InatialiseAllControl(root);

                btn_back = root.FindViewById<Button>(Resource.Id.btn_activity_back);
                btn_delete = root.FindViewById<Button>(Resource.Id.btn_activity_delete);
                btn_activity_updaterecord = root.FindViewById<Button>(Resource.Id.btn_activity_updaterecord);
                btn_back.Click += (sndr, argus) => Back_Clicked(sndr, argus, this.Activity);
                btn_delete.Click += (sndr, argus) => Delete_Clicked(sndr, argus, this.Activity);
                btn_activity_updaterecord.Click += (sndr, argus) => Update_Clicked(sndr, argus, this.Activity);

                var _activityDetails = GetActivityById(ActivityID);

                FillActivityDetails(_activityDetails);

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
                        AddActivityFragment objFragment = new AddActivityFragment();
                        Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                        tx.Replace(Resource.Id.m_main, objFragment, Constants.addactivity);
                        tx.Commit();
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }

            return root;
        }

        private void InatialiseAllControl(ViewGroup root)
        {
            txtdtl_ActivityDate = root.FindViewById<TextView>(Resource.Id.txtdtl_ActivityDate);
            txtdtl_PlotName = root.FindViewById<TextView>(Resource.Id.txtdtl_PlotName);
            txtdtl_ActivityDescription = root.FindViewById<TextView>(Resource.Id.txtdtl_ActivityDescription);
            //txtdtl_category_name = root.FindViewById<TextView>(Resource.Id.txtdtl_category_name);
            txtdtl_ProductTypeName = root.FindViewById<TextView>(Resource.Id.txtdtl_ProductTypeName);
            txtdtl_product_name = root.FindViewById<TextView>(Resource.Id.txtdtl_product_name);

            txtdtl_LastHarvestedDate = root.FindViewById<TextView>(Resource.Id.txtdtl_LastHarvestedDate);
            txtdtl_HarvestedBefore = root.FindViewById<TextView>(Resource.Id.txtdtl_HarvestedBefore);
            txtdtl_SoldBefore = root.FindViewById<TextView>(Resource.Id.txtdtl_SoldBefore);
            txtdtl_SoldBeforeNoReason = root.FindViewById<TextView>(Resource.Id.txtdtl_SoldBeforeNoReason);
            txtdtl_SoldPrice = root.FindViewById<TextView>(Resource.Id.txtdtl_SoldPrice);
            txtdtl_PlantationDate = root.FindViewById<TextView>(Resource.Id.txtdtl_PlantationDate);
            txtdtl_TotalNumberOfResource = root.FindViewById<TextView>(Resource.Id.txtdtl_TotalNumberOfResource);
            txtdtl_ResourceCostType = root.FindViewById<TextView>(Resource.Id.txtdtl_ResourceCostType);
            txtdtl_ResourcePrice = root.FindViewById<TextView>(Resource.Id.txtdtl_ResourcePrice);
            txtdtl_NumberOfProduct = root.FindViewById<TextView>(Resource.Id.txtdtl_NumberOfProduct);
            txtdtl_LiveStockUsageName = root.FindViewById<TextView>(Resource.Id.txtdtl_LiveStockUsageName);
            txtdtl_LiveStockUtilityName = root.FindViewById<TextView>(Resource.Id.txtdtl_LiveStockUtilityName);
            txtdtl_IsLivestockSalable = root.FindViewById<TextView>(Resource.Id.txtdtl_IsLivestockSalable);
            txtdtl_LastDateOfLivestockSold = root.FindViewById<TextView>(Resource.Id.txtdtl_LastDateOfLivestockSold);
            txtdtl_SoldLiveStockAmount = root.FindViewById<TextView>(Resource.Id.txtdtl_SoldLiveStockAmount);
            txtdtl_LivestocksellingLocationName = root.FindViewById<TextView>(Resource.Id.txtdtl_LivestocksellingLocationName);
            txtdtl_LiveStockName = root.FindViewById<TextView>(Resource.Id.txtdtl_LiveStockName);
            txtdtl_ResourceTypeName = root.FindViewById<TextView>(Resource.Id.txtdtl_ResourceTypeName);
            txtdtl_ResourceCostTypeName = root.FindViewById<TextView>(Resource.Id.txtdtl_ResourceCostTypeName);
            txtdtl_SoldLiveStockProductName = root.FindViewById<TextView>(Resource.Id.txtdtl_SoldLiveStockProductName);
            txtdtl_ResourceMaintenanceCostTypeName = root.FindViewById<TextView>(Resource.Id.txtdtl_ResourceMaintenanceCostTypeName);
            txtdtl_ResourceMaintenancePrice = root.FindViewById<TextView>(Resource.Id.txtdtl_ResourceMaintenancePrice);

            txtdtl_LastHarvestedDate.Visibility = ViewStates.Gone;
            txtdtl_HarvestedBefore.Visibility = ViewStates.Gone;
            txtdtl_SoldBefore.Visibility = ViewStates.Gone;
            txtdtl_SoldBeforeNoReason.Visibility = ViewStates.Gone;
            txtdtl_SoldPrice.Visibility = ViewStates.Gone;
            txtdtl_PlantationDate.Visibility = ViewStates.Gone;
            txtdtl_TotalNumberOfResource.Visibility = ViewStates.Gone;
            txtdtl_ResourceCostType.Visibility = ViewStates.Gone;
            txtdtl_ResourcePrice.Visibility = ViewStates.Gone;
            txtdtl_NumberOfProduct.Visibility = ViewStates.Gone;
            txtdtl_LiveStockUsageName.Visibility = ViewStates.Gone;
            txtdtl_LiveStockUtilityName.Visibility = ViewStates.Gone;
            txtdtl_IsLivestockSalable.Visibility = ViewStates.Gone;
            txtdtl_LastDateOfLivestockSold.Visibility = ViewStates.Gone;
            txtdtl_SoldLiveStockAmount.Visibility = ViewStates.Gone;
            txtdtl_LivestocksellingLocationName.Visibility = ViewStates.Gone;
            txtdtl_LiveStockName.Visibility = ViewStates.Gone;
            txtdtl_ResourceTypeName.Visibility = ViewStates.Gone;
            txtdtl_ResourceCostTypeName.Visibility = ViewStates.Gone;
            txtdtl_SoldLiveStockProductName.Visibility = ViewStates.Gone;
            txtdtl_ResourceMaintenanceCostTypeName.Visibility = ViewStates.Gone;
            txtdtl_ResourceMaintenancePrice.Visibility = ViewStates.Gone;


        }

        private ActivityDetailResponse GetActivityById(string ActivityId)
        {
            ActivityDetailResponse oboActivity = new ActivityDetailResponse();

            try
            {
                var client = new RestClient(Common.UrlBase);
                var request = new RestRequest("Activity/GetActivityDetail", Method.GET);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("TokenKey", mStringSessionToken);
                request.AddQueryParameter("ActivityId", System.Net.WebUtility.UrlEncode(ActivityId));
                IRestResponse response = client.Execute(request);
                var content = response.Content;
                var responseObj = Newtonsoft.Json.JsonConvert.DeserializeObject<IList<ActivityDetailResponse>>(content);
                if (responseObj != null && responseObj.Count()>0)
                {
                    oboActivity = responseObj.FirstOrDefault();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }            

            return oboActivity;
        }

        private void FillActivityDetails(ActivityDetailResponse ActivityDetail)
        {
            txtdtl_ActivityDate.Text = "Activity Date : " + ActivityDetail.ActivityDate.ToString("dd-MM-yyyy");
            txtdtl_PlotName.Text = "Plot Name : " + ActivityDetail.PlotName;
            txtdtl_ActivityDescription.Text = "Actyvity Type : " + ActivityDetail.ActivityDescription;
            //txtdtl_category_name.Text = "Catagory Name : " + ActivityDetail.CategoryName;
            txtdtl_product_name.Text = "Product Name : " + ActivityDetail.ProductName;

            var _productType = (ActivityDetail.ProductTypeId == ProductType.Crop.GetHashCode()? ProductType.Crop: ActivityDetail.ProductTypeId == ProductType.LiveStock.GetHashCode()? 
                ProductType.LiveStock: ActivityDetail.ProductTypeId == ProductType.Resource.GetHashCode() ? ProductType.Resource: ProductType.None);

            txtdtl_ProductTypeName.Text = "Product Type : " + _productType;

            switch (_productType)
            {
                case ProductType.Crop:
                    {
                        FillCropActivity(ActivityDetail);
                        break;
                    }
                case ProductType.LiveStock:
                    {
                        FillLivestockActivity(ActivityDetail);
                        break;
                    }
                case ProductType.Resource:
                    {
                        FillResourceActivity(ActivityDetail);
                        break;
                    }
                default:
                    {
                        FillCropActivity(ActivityDetail);
                        break;
                    }
            }
        }

        private void FillCropActivity(ActivityDetailResponse ActivityDetail)
        {
            txtdtl_PlantationDate.Text = "Plantation Date : " + ActivityDetail.PlantationDate.ToString("dd-MM-yyyy");
            txtdtl_PlantationDate.Visibility = ViewStates.Visible;

            txtdtl_HarvestedBefore.Text = "Harvested Before : " + (ActivityDetail.IsHarvestedBefore == true ? "Yes" : "No");
            txtdtl_HarvestedBefore.Visibility = ViewStates.Visible;

            if (ActivityDetail.IsHarvestedBefore)
            {
                txtdtl_LastHarvestedDate.Text = "Last Harvested Date : " + ActivityDetail.LastHarvestedDate.ToString("dd-MM-yyyy");
                txtdtl_LastHarvestedDate.Visibility = ViewStates.Visible;
            }

            txtdtl_SoldBefore.Text = "Sold Before : " + (ActivityDetail.IsSoldBefore == true ? "Yes" : "No");
            txtdtl_SoldBefore.Visibility = ViewStates.Visible;

            if (ActivityDetail.IsSoldBefore)
            {
                txtdtl_SoldPrice.Text = "Sold Price : " + ActivityDetail.SoldPrice;
                txtdtl_SoldPrice.Visibility = ViewStates.Visible;
            }
            else
            {
                txtdtl_SoldBeforeNoReason.Text = "Reason : " + ActivityDetail.IsSoldBeforeNoReason;
                txtdtl_SoldBeforeNoReason.Visibility = ViewStates.Visible;
            }
        }

        private void FillLivestockActivity(ActivityDetailResponse ActivityDetail)
        {
            txtdtl_TotalNumberOfResource.Text = "Livestock Count : " + ActivityDetail.NumberOfLivestock;
            txtdtl_TotalNumberOfResource.Visibility = ViewStates.Visible;

            txtdtl_LiveStockUsageName.Text = "Use Type : " + ActivityDetail.LiveStockUsageName;
            txtdtl_LiveStockUsageName.Visibility = ViewStates.Visible;

            txtdtl_IsLivestockSalable.Text = "Livestock Sale : " + (ActivityDetail.IsLivestockSalable == true ? "Yes" : "No");
            txtdtl_IsLivestockSalable.Visibility = ViewStates.Visible;

            if (ActivityDetail.IsLivestockSalable)
            {
                txtdtl_LastDateOfLivestockSold.Text = "Last Sale Date : " + ActivityDetail.LastDateOfLivestockSold.ToString("dd-MM-yyyy");
                txtdtl_LastDateOfLivestockSold.Visibility = ViewStates.Visible;

                txtdtl_LivestocksellingLocationName.Text = "Sale Location : " + ActivityDetail.LivestocksellingLocationName;
                txtdtl_LivestocksellingLocationName.Visibility = ViewStates.Visible;

                txtdtl_LiveStockUtilityName.Text = "Last Sell Type : " + ActivityDetail.LiveStockUtilityName;
                txtdtl_LiveStockUtilityName.Visibility = ViewStates.Visible;

                txtdtl_SoldLiveStockAmount.Text = "Sell Amount : " + ActivityDetail.SoldLiveStockAmount;
                txtdtl_SoldLiveStockAmount.Visibility = ViewStates.Visible;
            }
            
        }

        private void FillResourceActivity(ActivityDetailResponse ActivityDetail)
        {
            txtdtl_TotalNumberOfResource.Text = "Resource Count : " + ActivityDetail.NumberOfResource;
            txtdtl_TotalNumberOfResource.Visibility = ViewStates.Visible;

            txtdtl_ResourceTypeName.Text = "Resource Type Name : " + ActivityDetail.ResourceTypeName;
            txtdtl_ResourceTypeName.Visibility = ViewStates.Visible;

            txtdtl_ResourceCostTypeName.Text = "Cost Type : " + ActivityDetail.ResourceCostTypeName;
            txtdtl_ResourceCostTypeName.Visibility = ViewStates.Visible;

            txtdtl_ResourcePrice.Text = "Cost Price : " + ActivityDetail.ResourcePrice;
            txtdtl_ResourcePrice.Visibility = ViewStates.Visible;

            txtdtl_ResourceMaintenanceCostTypeName.Text = "Maintenan Type : " + ActivityDetail.ResourceMaintenaceCostTypeName;
            txtdtl_ResourceMaintenanceCostTypeName.Visibility = ViewStates.Visible;

            txtdtl_ResourceMaintenancePrice.Text = "Maintenan Cost : " + ActivityDetail.ResourceMaintenancePrice;
            txtdtl_ResourceMaintenancePrice.Visibility = ViewStates.Visible;
        }

        private void Update_Clicked(object sender, EventArgs e, Activity currentActivity)
        {
            btn_activity_updaterecord.Click -= (sndr, argus) => Update_Clicked(sndr, argus, currentActivity);
            try
            {
                Bundle utilBundle = new Bundle();
                utilBundle.PutString("siteparamid", ActivityID);
                AddSelectedItemFragment objFragment = new AddSelectedItemFragment();
                objFragment.Arguments = utilBundle;
                Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                tx.Replace(Resource.Id.m_main, objFragment, Constants.addselecteditem);
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
                        btn_activity_updaterecord.Click += (sndr, argus) => Update_Clicked(sndr, argus, currentActivity);
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }

        private void Back_Clicked(object sender, EventArgs e, Activity currentActivity)
        {
            btn_back.Click -= (sndr, argus) => Back_Clicked(sndr, argus, currentActivity);
            try
            {
                Bundle utilBundle = new Bundle();
                utilBundle.PutString("siteparamdate", selecteddate);
                ActivityByDateFragment objFragment = new ActivityByDateFragment();
                objFragment.Arguments = utilBundle;
                Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                tx.Replace(Resource.Id.m_main, objFragment, Constants.addactivitybydate);
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
                        btn_back.Click += (sndr, argus) => Back_Clicked(sndr, argus, currentActivity);
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }

        private void Delete_Clicked(object sender, EventArgs e, Activity currentActivity)
        {
            btn_delete.Click -= (sndr, argus) => Delete_Clicked(sndr, argus, currentActivity);
            try
            {
                currentActivity.RunOnUiThread(() =>
                {
                    Android.App.AlertDialog.Builder alertDiag = new Android.App.AlertDialog.Builder(currentActivity);
                    alertDiag.SetTitle(Resource.String.DialogHeaderGeneric);
                    alertDiag.SetMessage("Are you sure you want to delete this activity?");
                    alertDiag.SetIcon(Resource.Drawable.alert);
                    alertDiag.SetPositiveButton(Resource.String.DialogButtonOk, (senderAlert, args) =>
                    {
                        ProgressDialog progressDialog = null;
                        progressDialog = ProgressDialog.Show(this.Activity, "Please wait...", "Deleting current activity...", true);
                        new Thread(new ThreadStart(delegate
                        {
                            currentActivity.RunOnUiThread(() => this.DeleteData(progressDialog, currentActivity));
                        })).Start();
                    });
                    alertDiag.SetNegativeButton(Resource.String.DialogButtonNo, (senderAlert, args) =>
                    {
                        //btn_save.Click += (sndr, argus) => Save_Clicked(sndr, argus, currentActivity);
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
                        //btn_save.Click += (sndr, argus) => Save_Clicked(sndr, argus, currentActivity);
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }

        private void DeleteData(ProgressDialog dialog, Activity curActivity)
        {
            try
            {
                var client = new RestClient(Common.UrlBase);
                var request = new RestRequest("Activity/DeleteActivity", Method.GET);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("TokenKey", mStringSessionToken);
                request.AddQueryParameter("ActivityId", System.Net.WebUtility.UrlEncode(ActivityID));
                IRestResponse response = client.Execute(request);
                var content = response.Content;
                var responseObj = Newtonsoft.Json.JsonConvert.DeserializeObject<ActivityDetailResponse>(content);
                if (responseObj != null && responseObj.Status== ResponseStatus.Successful)
                {
                    curActivity.RunOnUiThread(() =>
                    {
                        Android.App.AlertDialog.Builder alertDiag = new Android.App.AlertDialog.Builder(curActivity);
                        alertDiag.SetTitle(Resource.String.DialogHeaderGeneric);
                        alertDiag.SetMessage("Activity has been deleted successfully");
                        alertDiag.SetIcon(Resource.Drawable.success);
                        alertDiag.SetPositiveButton(Resource.String.DialogButtonOk, (senderAlert, args) =>
                        {
                            Bundle utilBundle = new Bundle();
                            utilBundle.PutString("siteparamdate", selecteddate);
                            ActivityByDateFragment objFragment = new ActivityByDateFragment();
                            objFragment.Arguments = utilBundle;
                            Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                            tx.Replace(Resource.Id.m_main, objFragment, Constants.addactivitybydate);
                            tx.Commit();
                        });
                        Dialog diag = alertDiag.Create();
                        diag.Show();
                        diag.SetCanceledOnTouchOutside(false);
                    });
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