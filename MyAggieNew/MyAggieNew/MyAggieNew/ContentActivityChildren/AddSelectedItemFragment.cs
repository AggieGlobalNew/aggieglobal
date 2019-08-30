using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static Android.App.DatePickerDialog;
using AlertDialog = Android.App.AlertDialog;
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

namespace MyAggieNew
{
    public class AddSelectedItemFragment : Android.Support.V4.App.Fragment
    {
        IList<ItemPayloadModelWithBase64> objSelectedItem;
        IList<string> lstLivestockFor = null;
        IList<string> lstLivestockSellLocation = null;
        IList<string> lstLivestockSellType = null;
        IList<string> lstResourceOwnType = null;
        IList<string> lstResourceCost = null;
        IList<string> lstResourceMaintenaceCost = null;

        List<PlotDetailResponse> lstDBPlot = null;
        List<ProductResourcesResponse> lstProductResources = null;
        List<ActivityDescriptions> lstActivityDesc = null;

        ImageView img_lvl_1, img_lvl_2;
        TextView txt_mainhead, txt_lvl_1, txt_lvl_2, txt_livestockSellType, txt_livestockSellWho;
        EditText date_EditText, dateLastHarvest_EditText, input_saleAmount,
            input_saleProductDesc, input_livestockCount, input_livestockForOther, dateLastLivestockSell_EditText,
            input_livestockSellTypeOther, input_livestockSellNo, input_livestockSellWhoOther, input_resourceType, input_resourceNumber,
            input_resourceOwnTypeOther, input_price, input_maintenaceprice, activityDate_EditText;
        Button btn_cancel, btn_save;
        Switch switchChooseHarvest, switchSaleProduct, switchLivestockSell;
        Spinner spinner_livestockFor, spinner_livestockSellType, spinner_livestockSellWho, spinner_resourceOwnType,
            spinner_resourceCost, spinner_resourceMaintenaceCost, spinner_SelectPlot, spinner_activityComplete;
        TextInputLayout dateLastHarvest_TextInputLayout;

        string parent_tag, spinnerlivestockForTxt, spinnerlivestockSellTypeTxt, spinnerlivestockSellWhoTxt,
            spinnerResourceOwnTypeTxt, spinnerResourceCostTxt, spinnerResourceMaintenaceCostTxt,
            spinnerSelectPlotTxt, spinneractivityCompleteTxt, spinneractivityCompleteDescTxt;

        string mStringSessionToken = string.Empty;

        ProductType activityType = ProductType.None;

        bool IsInsert = true;
        ActivityDetailResponse _activityDetails = new ActivityDetailResponse();

        //IList<string> arrFields;

        public AddSelectedItemFragment() { }

        public static Android.Support.V4.App.Fragment newInstance(Context context)
        {
            AddSelectedItemFragment busrouteFragment = new AddSelectedItemFragment();
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

                if (Arguments != null)
                {
                    if (objSelectedItem == null)
                    {
                        objSelectedItem = new List<ItemPayloadModelWithBase64>();
                    }
                    string siteparam = string.Empty;
                    string activityIdParam = string.Empty;
                    try
                    {
                        siteparam = Arguments.GetString("siteparam");
                    }
                    catch { }
                    if (!string.IsNullOrEmpty(siteparam))
                    {
                        objSelectedItem = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ItemPayloadModelWithBase64>>(siteparam);
                    }
                    try
                    {
                        activityIdParam = Arguments.GetString("siteparamid");
                    }
                    catch { }
                    if (!string.IsNullOrEmpty(activityIdParam))
                    {
                        IsInsert = false;
                        _activityDetails = GetActivityById(activityIdParam);

                        var objCategory = new ItemPayloadModelWithBase64();
                        objCategory.ItemCode = _activityDetails.CategoryID;
                        objCategory.ItemName = _activityDetails.CategoryName;
                        objCategory.prdType = (ProductType)_activityDetails.ProductTypeId;
                        objCategory.ItemIcon = BitmapHelpers.BitmapToBase64(BitmapHelpers.GetImageFromUrl(string.Format("{0}.png", _activityDetails.CategoryName.Trim()), mStringSessionToken));
                        objSelectedItem.Add(objCategory);

                        var objProduct = new ItemPayloadModelWithBase64();
                        objProduct.ItemCode = _activityDetails.ProductId;
                        objProduct.ItemName = _activityDetails.ProductName;
                        objProduct.ItemIcon = BitmapHelpers.BitmapToBase64(BitmapHelpers.GetImageFromUrl(string.Format("{0}.png", _activityDetails.ProductName.Trim()), mStringSessionToken));
                        objProduct.prdType = (ProductType)_activityDetails.ProductTypeId;
                        objSelectedItem.Add(objProduct);
                    }
                    else
                    {
                        IsInsert = true;
                    }
                }
                else
                {
                    throw new Exception("No item found for activity saving");
                }

                

                lstProductResources = FillAllDropdown("ProductResources/GetProductResourcesList");

                activityType = objSelectedItem[1].prdType;
                lstActivityDesc = GetActivityDescription(activityType);

                switch (objSelectedItem[1].prdType)
                {
                    case ProductType.Crop:
                        {
                            root = (ViewGroup)inflater.Inflate(Resource.Layout.fragment_content_addcropitem, null);
                            LoadCropActivity(root);
                            break;
                        }
                    case ProductType.LiveStock:
                        {
                            root = (ViewGroup)inflater.Inflate(Resource.Layout.fragment_content_addlivestockitem, null);
                            LoadLivestockActivity(root);
                            break;
                        }
                    case ProductType.Resource:
                        {
                            root = (ViewGroup)inflater.Inflate(Resource.Layout.fragment_content_addresourceitem, null);
                            LoadResourceActivity(root);
                            break;
                        }
                    default:
                        {
                            root = (ViewGroup)inflater.Inflate(Resource.Layout.fragment_content_addcropitem, null);
                            LoadCropActivity(root);
                            break;
                        }
                }

                lstDBPlot = GetAllPlotByUser();
                List<string> lstPlot = lstDBPlot.Count() > 0 ? lstDBPlot.Select(u => u.PlotName).ToList() : new List<string>() { "None" };

                if (lstDBPlot.Count() == 0 || (lstDBPlot.FirstOrDefault() != null && string.IsNullOrEmpty(lstDBPlot.FirstOrDefault().PlotId)))
                {
                    lstPlot = new List<string>();
                    this.Activity.RunOnUiThread(() =>
                    {
                        Android.App.AlertDialog.Builder alertDiag = new Android.App.AlertDialog.Builder(this.Activity);
                        alertDiag.SetTitle(Resource.String.DialogHeaderError);
                        alertDiag.SetMessage("Please add a new plot before staring any activities");
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
                else
                {
                    lstPlot.Insert(0, "Select");
                }

                spinner_SelectPlot = root.FindViewById<Spinner>(Resource.Id.spinner_SelectPlot);
                spinner_SelectPlot.ItemSelected += (sndr, argus) => spinner_SelectPlot_ItemSelected(sndr, argus, this.Activity);
                spinner_SelectPlot.Adapter = new ArrayAdapter<string>(this.Activity, Android.Resource.Layout.SimpleSpinnerItem, lstPlot);

                var lstActivityDescText = lstActivityDesc.Select(u => u.ActivityDescription) != null ? lstActivityDesc.Select(u => u.ActivityDescription).ToList() : new List<string>();
                lstActivityDescText.Insert(0, "Select");
                spinner_activityComplete = root.FindViewById<Spinner>(Resource.Id.spinner_activityComplete);
                spinner_activityComplete.ItemSelected += (sndr, argus) => spinner_activityComplete_ItemSelected(sndr, argus, this.Activity);
                spinner_activityComplete.Adapter = new ArrayAdapter<string>(this.Activity, Android.Resource.Layout.SimpleSpinnerItem, lstActivityDescText);

                activityDate_EditText = root.FindViewById<EditText>(Resource.Id.activityDate_EditText);
                activityDate_EditText.Click += delegate
                {
                    OnClickActivityDateEditText(this.Activity);
                };

                if (!IsInsert)
                {
                    int activityCompleteItemId = lstActivityDescText.IndexOf(_activityDetails.ActivityDescription.Trim());
                    spinner_activityComplete.SetSelection(activityCompleteItemId);

                    activityDate_EditText.Text = _activityDetails.ActivityDate.ToString("dd-MM-yyyy");

                    int plotItemId = lstPlot.IndexOf(_activityDetails.PlotName.Trim());
                    spinner_SelectPlot.SetSelection(plotItemId);
                }

                txt_mainhead = root.FindViewById<TextView>(Resource.Id.txt_mainhead);
                txt_lvl_1 = root.FindViewById<TextView>(Resource.Id.txt_lvl_1);
                txt_lvl_2 = root.FindViewById<TextView>(Resource.Id.txt_lvl_2);
                img_lvl_1 = root.FindViewById<ImageView>(Resource.Id.img_lvl_1);
                img_lvl_2 = root.FindViewById<ImageView>(Resource.Id.img_lvl_2);

                if (Arguments != null)
                {
                    /*if (objSelectedItem == null)
                    {
                        objSelectedItem = new List<ItemPayloadModelWithBase64>();
                    }
                    objSelectedItem = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ItemPayloadModelWithBase64>>(Arguments.GetString("siteparam"));*/
                    if (objSelectedItem != null && objSelectedItem.Count() > default(int))
                    {
                        txt_mainhead.Text = "Selected Details:";
                        img_lvl_1.Visibility = ViewStates.Visible;
                        txt_lvl_1.Visibility = ViewStates.Visible;
                        img_lvl_2.Visibility = ViewStates.Visible;
                        txt_lvl_2.Visibility = ViewStates.Visible;
                        txt_lvl_1.Text = objSelectedItem[0] != null ? objSelectedItem[0].ItemName : "";
                        txt_lvl_2.Text = objSelectedItem[1] != null ? objSelectedItem[1].ItemName : "";
                        img_lvl_1.SetImageBitmap(BitmapHelpers.Base64ToBitmap(objSelectedItem[0].ItemIcon));
                        img_lvl_2.SetImageBitmap(BitmapHelpers.Base64ToBitmap(objSelectedItem[1].ItemIcon));
                    }
                    else
                    {
                        txt_mainhead.Text = "Select Details:";
                        img_lvl_1.Visibility = ViewStates.Gone;
                        txt_lvl_1.Visibility = ViewStates.Gone;
                        img_lvl_2.Visibility = ViewStates.Gone;
                        txt_lvl_2.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    objSelectedItem = null;
                    txt_mainhead.Text = "Select Details:";
                    img_lvl_1.Visibility = ViewStates.Gone;
                    txt_lvl_1.Visibility = ViewStates.Gone;
                    img_lvl_2.Visibility = ViewStates.Gone;
                    txt_lvl_2.Visibility = ViewStates.Gone;
                }

                btn_cancel = root.FindViewById<Button>(Resource.Id.btn_cancel);
                btn_save = root.FindViewById<Button>(Resource.Id.btn_save);
                btn_cancel.Click += (sndr, argus) => Cancel_Clicked(sndr, argus, this.Activity);
                btn_save.Click += (sndr, argus) => Save_Clicked(sndr, argus, this.Activity);
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

        private void LoadResourceActivity(ViewGroup root)
        {
            try
            {
                lstResourceOwnType = lstProductResources.Count() > 0 ? lstProductResources.Where(u => u.ProductRessourceType == ProductRessourceType.ResourceType).Select(u => u.ProductResourceName).ToList() : new List<string>();
                lstResourceOwnType.Insert(0, "Select");

                lstResourceCost = lstProductResources.Count() > 0 ? lstProductResources.Where(u => u.ProductRessourceType == ProductRessourceType.ResourceCostType).Select(u => u.ProductResourceName).ToList() : new List<string>();
                lstResourceCost.Insert(0, "Select");

                lstResourceMaintenaceCost = lstProductResources.Count() > 0 ? lstProductResources.Where(u => u.ProductRessourceType == ProductRessourceType.ResourceMaintenaceCostType).Select(u => u.ProductResourceName).ToList() : new List<string>();
                lstResourceMaintenaceCost.Insert(0, "Select");

                //input_resourceType = root.FindViewById<EditText>(Resource.Id.input_resourceType);
                input_resourceNumber = root.FindViewById<EditText>(Resource.Id.input_resourceNumber);
                input_resourceOwnTypeOther = root.FindViewById<EditText>(Resource.Id.input_resourceOwnTypeOther);
                input_price = root.FindViewById<EditText>(Resource.Id.input_price);
                input_maintenaceprice = root.FindViewById<EditText>(Resource.Id.input_maintenaceprice);

                spinner_resourceOwnType = root.FindViewById<Spinner>(Resource.Id.spinner_resourceOwnType);
                spinner_resourceOwnType.ItemSelected += (sndr, argus) => spinner_resourceOwnType_ItemSelected(sndr, argus, this.Activity);
                spinner_resourceOwnType.Adapter = new ArrayAdapter<string>(this.Activity, Android.Resource.Layout.SimpleSpinnerItem, lstResourceOwnType);

                spinner_resourceCost = root.FindViewById<Spinner>(Resource.Id.spinner_resourceCost);
                spinner_resourceCost.ItemSelected += (sndr, argus) => spinner_resourceCost_ItemSelected(sndr, argus, this.Activity);
                spinner_resourceCost.Adapter = new ArrayAdapter<string>(this.Activity, Android.Resource.Layout.SimpleSpinnerItem, lstResourceCost);

                spinner_resourceMaintenaceCost = root.FindViewById<Spinner>(Resource.Id.spinner_resourceMaintenaceCost);
                spinner_resourceMaintenaceCost.ItemSelected += (sndr, argus) => spinner_resourceMaintenaceCost_ItemSelected(sndr, argus, this.Activity);
                spinner_resourceMaintenaceCost.Adapter = new ArrayAdapter<string>(this.Activity, Android.Resource.Layout.SimpleSpinnerItem, lstResourceMaintenaceCost);

                input_resourceOwnTypeOther.Visibility = ViewStates.Gone;

                if (!IsInsert)
                {                    
                    input_resourceNumber.Text = _activityDetails.NumberOfResource.ToString();
                    int resourceOwnTypeItemId = lstResourceOwnType.IndexOf(_activityDetails.ResourceTypeName.Trim());
                    spinner_resourceOwnType.SetSelection(resourceOwnTypeItemId);

                    int resourceCostTypeItemId = lstResourceCost.IndexOf(_activityDetails.ResourceCostTypeName.Trim());
                    spinner_resourceCost.SetSelection(resourceCostTypeItemId);

                    int resourceMaintenaceCostTypeItemId = lstResourceMaintenaceCost.IndexOf(_activityDetails.ResourceMaintenaceCostTypeName.Trim());
                    spinner_resourceMaintenaceCost.SetSelection(resourceMaintenaceCostTypeItemId);

                    input_price.Text = _activityDetails.ResourcePrice.ToString();
                    input_maintenaceprice.Text = _activityDetails.ResourceMaintenancePrice.ToString();
                }

                //input_resourceType.Visibility = ViewStates.Gone;
            }
            catch { }
        }

        private void LoadLivestockActivity(ViewGroup root)
        {
            try
            {
                lstLivestockFor = lstProductResources.Count() > 0 ? lstProductResources.Where(u => u.ProductRessourceType == ProductRessourceType.LiveStockUsage).Select(u => u.ProductResourceName).ToList() : new List<string>();
                lstLivestockFor.Insert(0, "Select");

                lstLivestockSellLocation = lstProductResources.Count() > 0 ? lstProductResources.Where(u => u.ProductRessourceType == ProductRessourceType.LivestocksellingLocation).Select(u => u.ProductResourceName).ToList() : new List<string>();
                lstLivestockSellLocation.Insert(0, "Select");

                lstLivestockSellType = lstProductResources.Count() > 0 ? lstProductResources.Where(u => u.ProductRessourceType == ProductRessourceType.LivestockUtility).Select(u => u.ProductResourceName).ToList() : new List<string>();
                lstLivestockSellType.Insert(0, "Select");

                //input_livestockType = root.FindViewById<EditText>(Resource.Id.input_livestockType);
                input_livestockCount = root.FindViewById<EditText>(Resource.Id.input_livestockCount);
                input_livestockForOther = root.FindViewById<EditText>(Resource.Id.input_livestockForOther);
                input_livestockSellTypeOther = root.FindViewById<EditText>(Resource.Id.input_livestockSellTypeOther);
                input_livestockSellNo = root.FindViewById<EditText>(Resource.Id.input_livestockSellNo);
                input_livestockSellWhoOther = root.FindViewById<EditText>(Resource.Id.input_livestockSellWhoOther);

                spinner_livestockFor = root.FindViewById<Spinner>(Resource.Id.spinner_livestockFor);
                spinner_livestockFor.ItemSelected += (sndr, argus) => spinner_livestockFor_ItemSelected(sndr, argus, this.Activity);
                spinner_livestockFor.Adapter = new ArrayAdapter<string>(this.Activity, Android.Resource.Layout.SimpleSpinnerItem, lstLivestockFor);

                spinner_livestockSellType = root.FindViewById<Spinner>(Resource.Id.spinner_livestockSellType);
                spinner_livestockSellType.ItemSelected += (sndr, argus) => spinner_livestockSellType_ItemSelected(sndr, argus, this.Activity);
                spinner_livestockSellType.Adapter = new ArrayAdapter<string>(this.Activity, Android.Resource.Layout.SimpleSpinnerItem, lstLivestockSellType);

                spinner_livestockSellWho = root.FindViewById<Spinner>(Resource.Id.spinner_livestockSellWho);
                spinner_livestockSellWho.ItemSelected += (sndr, argus) => spinner_livestockSellWho_ItemSelected(sndr, argus, this.Activity);
                spinner_livestockSellWho.Adapter = new ArrayAdapter<string>(this.Activity, Android.Resource.Layout.SimpleSpinnerItem, lstLivestockSellLocation);

                switchLivestockSell = root.FindViewById<Switch>(Resource.Id.switchLivestockSell);
                switchLivestockSell.CheckedChange += (sndr, argus) => SwitchLivestockSell_ItemSelected(sndr, argus, this.Activity);

                dateLastLivestockSell_EditText = root.FindViewById<EditText>(Resource.Id.dateLastLivestockSell_EditText);
                dateLastLivestockSell_EditText.Click += delegate
                {
                    OnClickDateLastLivestockSell(this.Activity);
                };

                txt_livestockSellType = root.FindViewById<TextView>(Resource.Id.txt_livestockSellType);
                txt_livestockSellWho = root.FindViewById<TextView>(Resource.Id.txt_livestockSellWho);

                input_livestockForOther.Visibility = ViewStates.Gone;
                dateLastLivestockSell_EditText.Visibility = ViewStates.Gone;
                spinner_livestockSellType.Visibility = ViewStates.Gone;
                input_livestockSellTypeOther.Visibility = ViewStates.Gone;
                input_livestockSellNo.Visibility = ViewStates.Gone;
                spinner_livestockSellWho.Visibility = ViewStates.Gone;
                input_livestockSellWhoOther.Visibility = ViewStates.Gone;
                txt_livestockSellType.Visibility = ViewStates.Gone;
                txt_livestockSellWho.Visibility = ViewStates.Gone;

                if (!IsInsert)
                {
                    input_livestockCount.Text = _activityDetails.NumberOfLivestock.ToString();

                    int livestockForItemId = lstLivestockFor.IndexOf(_activityDetails.LiveStockUsageName.Trim());
                    spinner_livestockFor.SetSelection(livestockForItemId);

                    if (_activityDetails.IsLivestockSalable)
                    {
                        switchLivestockSell.Text = "YES";
                        switchLivestockSell.Checked = true;

                        dateLastLivestockSell_EditText.Text = _activityDetails.LastDateOfLivestockSold.ToString("dd-MM-yyyy");
                        dateLastLivestockSell_EditText.Visibility = ViewStates.Visible;

                        int livestockSellWhoItemId = lstLivestockSellLocation.IndexOf(_activityDetails.LivestocksellingLocationName.Trim());
                        spinner_livestockSellWho.SetSelection(livestockSellWhoItemId);
                        spinner_livestockSellWho.Visibility = ViewStates.Visible;
                        txt_livestockSellWho.Visibility = ViewStates.Visible;

                        int livestockSellTypeItemId = lstLivestockSellType.IndexOf(_activityDetails.LiveStockUtilityName.Trim());
                        spinner_livestockSellType.SetSelection(livestockSellTypeItemId);
                        spinner_livestockSellType.Visibility = ViewStates.Visible;
                        txt_livestockSellType.Visibility = ViewStates.Visible;

                        input_livestockSellNo.Text = _activityDetails.SoldLiveStockAmount.ToString();
                        input_livestockSellNo.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        switchLivestockSell.Text = "NO";
                        switchLivestockSell.Checked = false;
                    }
                }

            }
            catch { }
        }

        private void LoadCropActivity(ViewGroup root)
        {
            try
            {
                if (IsInsert)
                {
                    date_EditText = root.FindViewById<EditText>(Resource.Id.date_EditText);
                    date_EditText.Click += delegate
                    {
                        OnClickDateEditText(this.Activity);
                    };

                    dateLastHarvest_EditText = root.FindViewById<EditText>(Resource.Id.dateLastHarvest_EditText);
                    dateLastHarvest_EditText.Click += delegate
                    {
                        OnClickdateLastHarvestEditText(this.Activity);
                    };

                    dateLastHarvest_EditText.Visibility = ViewStates.Gone;

                    switchChooseHarvest = root.FindViewById<Switch>(Resource.Id.switchChooseHarvest);
                    switchSaleProduct = root.FindViewById<Switch>(Resource.Id.switchSaleProduct);

                    switchChooseHarvest.CheckedChange += (sndr, argus) => SpnChooseHarvest_ItemSelected(sndr, argus, this.Activity);
                    switchSaleProduct.CheckedChange += (sndr, argus) => SpnSaleProduct_ItemSelected(sndr, argus, this.Activity);

                    date_EditText.RequestFocus();

                    input_saleAmount = root.FindViewById<EditText>(Resource.Id.input_saleAmount);
                    input_saleProductDesc = root.FindViewById<EditText>(Resource.Id.input_saleProductDesc);
                    input_saleAmount.Visibility = ViewStates.Gone;

                    dateLastHarvest_TextInputLayout = root.FindViewById<TextInputLayout>(Resource.Id.dateLastHarvest_TextInputLayout);
                    dateLastHarvest_TextInputLayout.Visibility = ViewStates.Gone;
                }
                else
                {
                    dateLastHarvest_TextInputLayout = root.FindViewById<TextInputLayout>(Resource.Id.dateLastHarvest_TextInputLayout);
                    date_EditText = root.FindViewById<EditText>(Resource.Id.date_EditText);
                    date_EditText.Click += delegate
                    {
                        OnClickDateEditText(this.Activity);
                    };

                    dateLastHarvest_EditText = root.FindViewById<EditText>(Resource.Id.dateLastHarvest_EditText);
                    dateLastHarvest_EditText.Click += delegate
                    {
                        OnClickdateLastHarvestEditText(this.Activity);
                    };

                    switchChooseHarvest = root.FindViewById<Switch>(Resource.Id.switchChooseHarvest);
                    switchSaleProduct = root.FindViewById<Switch>(Resource.Id.switchSaleProduct);

                    switchChooseHarvest.CheckedChange += (sndr, argus) => SpnChooseHarvest_ItemSelected(sndr, argus, this.Activity);
                    switchSaleProduct.CheckedChange += (sndr, argus) => SpnSaleProduct_ItemSelected(sndr, argus, this.Activity);

                    input_saleAmount = root.FindViewById<EditText>(Resource.Id.input_saleAmount);
                    input_saleProductDesc = root.FindViewById<EditText>(Resource.Id.input_saleProductDesc);


                    date_EditText.Text = _activityDetails.PlantationDate.ToString("dd-MM-yyyy");

                    if (_activityDetails.IsHarvestedBefore)
                    {
                        switchChooseHarvest.Text = "YES";
                        switchChooseHarvest.Checked = true;
                        dateLastHarvest_EditText.Text = _activityDetails.LastHarvestedDate.ToString("dd-MM-yyyy");
                        dateLastHarvest_EditText.Visibility = ViewStates.Visible;
                        dateLastHarvest_TextInputLayout.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        switchChooseHarvest.Text = "NO";
                        switchChooseHarvest.Checked = false;
                        dateLastHarvest_EditText.Visibility = ViewStates.Gone;
                        dateLastHarvest_TextInputLayout.Visibility = ViewStates.Gone;
                    }

                    if (_activityDetails.IsSoldBefore)
                    {
                        switchSaleProduct.Text= "YES";
                        switchSaleProduct.Checked = true;
                        input_saleProductDesc.Text = string.Empty;
                        input_saleProductDesc.Visibility = ViewStates.Gone;
                        input_saleAmount.Text = _activityDetails.SoldPrice.ToString();
                        input_saleAmount.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        switchSaleProduct.Text = "NO";
                        switchSaleProduct.Checked = false;
                        input_saleProductDesc.Text = _activityDetails.IsSoldBeforeNoReason;
                        input_saleProductDesc.Visibility = ViewStates.Visible;
                        input_saleAmount.Text = string.Empty;
                        input_saleAmount.Visibility = ViewStates.Gone;
                    }                    

                    date_EditText.RequestFocus();
                }

                
            }
            catch { }

            
        }

        private void spinner_SelectPlot_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e, Activity currentActivity)
        {
            Spinner spinner = (Spinner)sender;
            try
            {
                spinnerSelectPlotTxt = Convert.ToString(spinner.GetItemAtPosition(e.Position));
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

        private void spinner_activityComplete_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e, Activity currentActivity)
        {
            spinner_activityComplete.ItemSelected -= (sndr, argus) => spinner_activityComplete_ItemSelected(sndr, argus, currentActivity);
            Spinner spinner = (Spinner)sender;
            try
            {
                spinneractivityCompleteDescTxt = Convert.ToString(spinner.GetItemAtPosition(e.Position));
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
                        spinner_activityComplete.ItemSelected += (sndr, argus) => spinner_activityComplete_ItemSelected(sndr, argus, currentActivity);
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }

        private void spinner_resourceMaintenaceCost_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e, Activity currentActivity)
        {
            Spinner spinner = (Spinner)sender;
            try
            {
                spinnerResourceMaintenaceCostTxt = Convert.ToString(spinner.GetItemAtPosition(e.Position));
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

        private void spinner_resourceCost_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e, Activity currentActivity)
        {
            Spinner spinner = (Spinner)sender;
            try
            {
                spinnerResourceCostTxt = Convert.ToString(spinner.GetItemAtPosition(e.Position));
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

        private void spinner_resourceOwnType_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e, Activity currentActivity)
        {
            Spinner spinner = (Spinner)sender;
            try
            {
                spinnerResourceOwnTypeTxt = Convert.ToString(spinner.GetItemAtPosition(e.Position));
                input_resourceOwnTypeOther.Text = "";

                if (spinnerResourceOwnTypeTxt.Trim().ToUpper() == "OTHERS")
                {
                    input_resourceOwnTypeOther.Visibility = ViewStates.Visible;
                    var toast = Toast.MakeText(currentActivity, ("Please enter other type."),
                    ToastLength.Long);
                    toast.Show();
                }
                else
                {
                    input_resourceOwnTypeOther.Visibility = ViewStates.Gone;
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

                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }

        private void spinner_livestockSellWho_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e, Activity currentActivity)
        {
            Spinner spinner = (Spinner)sender;
            try
            {
                spinnerlivestockSellWhoTxt = Convert.ToString(spinner.GetItemAtPosition(e.Position));
                input_livestockSellWhoOther.Text = "";

                if (spinnerlivestockSellWhoTxt.Trim().ToUpper() == "OTHERS")
                {
                    input_livestockSellWhoOther.Visibility = ViewStates.Visible;
                    var toast = Toast.MakeText(currentActivity, ("Please enter other type."),
                    ToastLength.Long);
                    toast.Show();
                }
                else
                {
                    input_livestockSellWhoOther.Visibility = ViewStates.Gone;
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

                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }

        private void spinner_livestockSellType_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e, Activity currentActivity)
        {
            Spinner spinner = (Spinner)sender;
            try
            {
                spinnerlivestockSellTypeTxt = Convert.ToString(spinner.GetItemAtPosition(e.Position));
                input_livestockSellTypeOther.Text = "";

                if (spinnerlivestockSellTypeTxt.Trim().ToUpper() == "OTHERS")
                {
                    input_livestockSellTypeOther.Visibility = ViewStates.Visible;
                    var toast = Toast.MakeText(currentActivity, ("Please enter other type."), ToastLength.Long);
                    toast.Show();
                }
                else
                {
                    input_livestockSellTypeOther.Visibility = ViewStates.Gone;
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

                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }

        private void spinner_livestockFor_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e, Activity currentActivity)
        {
            Spinner spinner = (Spinner)sender;
            try
            {
                spinnerlivestockForTxt = Convert.ToString(spinner.GetItemAtPosition(e.Position));
                input_livestockForOther.Text = "";

                if (spinnerlivestockForTxt.Trim().ToUpper() == "OTHERS")
                {
                    input_livestockForOther.Visibility = ViewStates.Visible;
                    var toast = Toast.MakeText(currentActivity, ("Please enter other type."),
                    ToastLength.Long);
                    toast.Show();
                }
                else
                {
                    input_livestockForOther.Visibility = ViewStates.Gone;
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

                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }

        private void SwitchLivestockSell_ItemSelected(object sender, CompoundButton.CheckedChangeEventArgs e, Activity currentActivity)
        {
            var switchObj = (Switch)sender;
            try
            {

                if (e.IsChecked)
                {
                    switchObj.Text = Resources.GetString(Resource.String.DialogButtonYes);

                    dateLastLivestockSell_EditText.Visibility = ViewStates.Visible;
                    spinner_livestockSellType.Visibility = ViewStates.Visible;

                    if (spinnerlivestockSellTypeTxt != null)
                    {
                        if (spinnerlivestockSellTypeTxt.Trim().ToUpper() == "OTHERS")
                        {
                            input_livestockSellTypeOther.Visibility = ViewStates.Visible;
                        }
                        else
                        {
                            input_livestockSellTypeOther.Visibility = ViewStates.Gone;
                        }
                    }
                    else
                    {
                        input_livestockSellTypeOther.Visibility = ViewStates.Gone;
                    }

                    input_livestockSellNo.Visibility = ViewStates.Visible;

                    spinner_livestockSellWho.Visibility = ViewStates.Visible;

                    if (spinnerlivestockSellWhoTxt != null)
                    {
                        if (spinnerlivestockSellWhoTxt.Trim().ToUpper() == "OTHERS")
                        {
                            input_livestockSellWhoOther.Visibility = ViewStates.Visible;
                        }
                        else
                        {
                            input_livestockSellWhoOther.Visibility = ViewStates.Gone;
                        }
                    }
                    else
                    {
                        input_livestockSellWhoOther.Visibility = ViewStates.Gone;
                    }

                    txt_livestockSellType.Visibility = ViewStates.Visible;
                    txt_livestockSellWho.Visibility = ViewStates.Visible;
                    dateLastLivestockSell_EditText.RequestFocus();
                }
                else
                {
                    switchObj.Text = Resources.GetString(Resource.String.DialogButtonNo);

                    dateLastLivestockSell_EditText.Visibility = ViewStates.Gone;
                    spinner_livestockSellType.Visibility = ViewStates.Gone;
                    input_livestockSellTypeOther.Visibility = ViewStates.Gone;
                    input_livestockSellNo.Visibility = ViewStates.Gone;
                    spinner_livestockSellWho.Visibility = ViewStates.Gone;
                    input_livestockSellWhoOther.Visibility = ViewStates.Gone;
                    txt_livestockSellType.Visibility = ViewStates.Gone;
                    txt_livestockSellWho.Visibility = ViewStates.Gone;
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
                        switchObj.Text = Resources.GetString(Resource.String.DialogButtonNo);
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }

        private void OnClickDateEditText(Activity contentActivity)
        {
            var dateTimeNow = DateTime.Now;
            DatePickerDialog datePicker = new DatePickerDialog(contentActivity, AlertDialog.ThemeDeviceDefaultLight, new OnDateSetListener(date_EditText), dateTimeNow.Year, (dateTimeNow.Month), dateTimeNow.Day);
            datePicker.DatePicker.MaxDate = DateTimeOffset.Parse(dateTimeNow.ToString()).ToUnixTimeMilliseconds();
            datePicker.Show();
        }

        private void OnClickActivityDateEditText(Activity contentActivity)
        {
            var dateTimeNow = DateTime.Now;
            DatePickerDialog datePicker = new DatePickerDialog(contentActivity, AlertDialog.ThemeDeviceDefaultLight, new OnDateSetListener(activityDate_EditText), dateTimeNow.Year, (dateTimeNow.Month), dateTimeNow.Day);
            datePicker.DatePicker.MaxDate = DateTimeOffset.Parse(dateTimeNow.ToString()).ToUnixTimeMilliseconds();
            datePicker.Show();
        }

        private void OnClickDateLastLivestockSell(Activity contentActivity)
        {
            var dateTimeNow = DateTime.Now;
            DatePickerDialog datePicker = new DatePickerDialog(contentActivity, AlertDialog.ThemeDeviceDefaultLight, new OnDateSetListener(dateLastLivestockSell_EditText), dateTimeNow.Year, (dateTimeNow.Month), dateTimeNow.Day);
            datePicker.DatePicker.MaxDate = DateTimeOffset.Parse(dateTimeNow.ToString()).ToUnixTimeMilliseconds();
            datePicker.Show();
        }

        private void OnClickdateLastHarvestEditText(Activity contentActivity)
        {
            var dateTimeNow = DateTime.Now;
            DatePickerDialog datePicker = new DatePickerDialog(contentActivity, AlertDialog.ThemeDeviceDefaultLight, new OnDateSetListener(dateLastHarvest_EditText), dateTimeNow.Year, (dateTimeNow.Month), dateTimeNow.Day);
            datePicker.DatePicker.MaxDate = DateTimeOffset.Parse(dateTimeNow.ToString()).ToUnixTimeMilliseconds();
            datePicker.Show();
        }

        private void SpnChooseHarvest_ItemSelected(object sender, CompoundButton.CheckedChangeEventArgs e, Activity currentActivity)
        {
            var switchObj = (Switch)sender;
            try
            {
                if (e.IsChecked)
                {
                    switchObj.Text = Resources.GetString(Resource.String.DialogButtonYes);
                    dateLastHarvest_EditText.Visibility = ViewStates.Visible;
                    dateLastHarvest_TextInputLayout.Visibility = ViewStates.Visible;
                    dateLastHarvest_EditText.RequestFocus();
                }
                else
                {
                    dateLastHarvest_EditText.Visibility = ViewStates.Gone;
                    dateLastHarvest_TextInputLayout.Visibility = ViewStates.Gone;
                    switchObj.Text = Resources.GetString(Resource.String.DialogButtonNo);
                }

                var toast = Toast.MakeText(currentActivity, (e.IsChecked ?
                    "Please enter last harvest date." : string.Empty),
                    ToastLength.Long);
                toast.Show();
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
                        switchObj.Text = Resources.GetString(Resource.String.DialogButtonNo);
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }

        private void SpnSaleProduct_ItemSelected(object sender, CompoundButton.CheckedChangeEventArgs e, Activity currentActivity)
        {
            var switchObj = (Switch)sender;
            try
            {
                if (e.IsChecked)
                {
                    switchObj.Text = Resources.GetString(Resource.String.DialogButtonYes);
                    input_saleProductDesc.Visibility = ViewStates.Gone;
                    input_saleAmount.Visibility = ViewStates.Visible;
                    input_saleAmount.RequestFocus();
                }
                else
                {
                    switchObj.Text = Resources.GetString(Resource.String.DialogButtonNo);
                    input_saleAmount.Visibility = ViewStates.Gone;
                    input_saleProductDesc.Visibility = ViewStates.Visible;
                    input_saleProductDesc.RequestFocus();
                }

                var toast = Toast.MakeText(currentActivity, (e.IsChecked ?
                    "Please enter the sale amount." : "Please provide a reason for no sales."),
                    ToastLength.Long);
                toast.Show();
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
                        switchObj.Text = Resources.GetString(Resource.String.DialogButtonNo);
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }

        private void Cancel_Clicked(object sender, EventArgs e, Activity currentActivity)
        {
            try
            {
                btn_cancel.Click -= (sndr, argus) => Cancel_Clicked(sndr, argus, currentActivity);

                currentActivity.RunOnUiThread(() =>
                {
                    Android.App.AlertDialog.Builder alertDiag = new Android.App.AlertDialog.Builder(currentActivity);
                    alertDiag.SetTitle(Resource.String.DialogHeaderGeneric);
                    alertDiag.SetMessage(Resource.String.cancelFromAddItemMessage);
                    alertDiag.SetIcon(Resource.Drawable.alert);
                    alertDiag.SetPositiveButton(Resource.String.DialogButtonYes, (senderAlert, args) =>
                    {
                        Bundle utilBundle = new Bundle();
                        utilBundle.PutString("siteparam", Newtonsoft.Json.JsonConvert.SerializeObject(objSelectedItem));
                        AddActivityFragment objFragment = new AddActivityFragment();
                        objFragment.Arguments = utilBundle;
                        Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                        tx.Replace(Resource.Id.m_main, objFragment, Constants.addactivity);
                        tx.Commit();
                    });
                    alertDiag.SetNegativeButton(Resource.String.DialogButtonNo, (senderAlert, args) =>
                    {
                        //btn_cancel.Click += (sndr, argus) => Cancel_Clicked(sndr, argus, currentActivity);
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
                        btn_cancel.Click += (sndr, argus) => Cancel_Clicked(sndr, argus, currentActivity);
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }

        private void Save_Clicked(object sender, EventArgs e, Activity currentActivity)
        {
            try
            {
                if (ValidateForm(currentActivity, activityType))
                {
                    btn_save.Click -= (sndr, argus) => Save_Clicked(sndr, argus, currentActivity);
                    ProgressDialog progressDialog = null;
                    progressDialog = ProgressDialog.Show(this.Activity, "Please wait...", "Saving current activity...", true);
                    new Thread(new ThreadStart(delegate
                    {
                        currentActivity.RunOnUiThread(() => this.SaveData(progressDialog, currentActivity));
                    })).Start();
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
                        //btn_save.Click += (sndr, argus) => Save_Clicked(sndr, argus, currentActivity);
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }

        private void SaveData(ProgressDialog dialog, Activity curActivity)
        {
            try
            {
                List<ProductResourcesResponse> lstPrdResource = new List<ProductResourcesResponse>();

                ActivityDetailResponse objActivityDetail = new ActivityDetailResponse();

                if (!IsInsert)
                {
                    objActivityDetail.ActivityId = _activityDetails.ActivityId;
                }

                objActivityDetail.ProductId = objSelectedItem[1].ItemCode;
                objActivityDetail.ProductName = objSelectedItem[1].ItemName;
                objActivityDetail.CategoryID = objSelectedItem[0].ItemCode;
                objActivityDetail.ProductTypeId = activityType.GetHashCode();
                objActivityDetail.PlotId = lstDBPlot.Where(u => u.PlotName.Equals(spinnerSelectPlotTxt.Trim())).Select(u => u.PlotId).FirstOrDefault();
                objActivityDetail.ActivityDate = DateTime.ParseExact(activityDate_EditText.Text.Trim(), "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                objActivityDetail.ActivityDescriptionId = lstActivityDesc.Where(u => u.ProductType == activityType.GetHashCode() && u.ActivityDescription.Contains(spinneractivityCompleteDescTxt)).Select(u => u.ActivityDescriptionId).FirstOrDefault();
                
                objActivityDetail.PlantationDate = DateTime.Now;
                objActivityDetail.LastHarvestedDate = DateTime.Now;
                objActivityDetail.LastDateOfLivestockSold = DateTime.Now;

                if (activityType == ProductType.Crop)
                {
                    objActivityDetail.PlantationDate = DateTime.ParseExact(date_EditText.Text.Trim(), "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);


                    if (switchChooseHarvest.Text.Trim().ToUpper() == "YES")
                    {
                        objActivityDetail.IsHarvestedBefore = true;
                        objActivityDetail.LastHarvestedDate = DateTime.ParseExact(dateLastHarvest_EditText.Text.Trim(), "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        objActivityDetail.IsHarvestedBefore = false;
                    }

                    if (switchSaleProduct.Text.Trim().ToUpper() == "YES")
                    {
                        objActivityDetail.IsSoldBefore = true;
                        objActivityDetail.IsSoldBeforeNoReason = string.Empty;
                        objActivityDetail.SoldPrice = Convert.ToDecimal(input_saleAmount.Text.Trim());
                    }
                    else
                    {
                        objActivityDetail.IsSoldBefore = false;
                        objActivityDetail.IsSoldBeforeNoReason = input_saleProductDesc.Text.Trim();
                        objActivityDetail.SoldPrice = 0;
                    }
                }
                else if (activityType == ProductType.LiveStock)
                {
                    objActivityDetail.NumberOfLivestock = Convert.ToInt32(input_livestockCount.Text.Trim());
                    

                    if (spinnerlivestockForTxt.Trim().ToUpper() == "OTHERS")
                    {
                        objActivityDetail.LiveStockUsageName = input_livestockForOther.Text.Trim();
                        objActivityDetail.LiveStockUsageId = 0;
                        lstPrdResource.Add(new ProductResourcesResponse { ProductResourceId ="", ProductResourceName= objActivityDetail.LiveStockUsageName, ProductRessourceType= ProductRessourceType.LiveStockUsage });
                    }
                    else
                    {
                        objActivityDetail.LiveStockUsageId =   ProductRessourceType.LiveStockUsage.GetHashCode();
                        objActivityDetail.LiveStockUsageName = spinnerlivestockForTxt.Trim();

                        var _liveStockUsageId = lstProductResources.Where(u => u.ProductRessourceType == ProductRessourceType.LiveStockUsage && u.ProductResourceName == objActivityDetail.LiveStockUsageName).Select(u=>u.ProductResourceId).FirstOrDefault();
                        var _liveStockUsageIdVal = _liveStockUsageId == null ? string.Empty : _liveStockUsageId.ToString();
                        lstPrdResource.Add(new ProductResourcesResponse { ProductResourceId = _liveStockUsageIdVal, ProductResourceName = objActivityDetail.LiveStockUsageName, ProductRessourceType = ProductRessourceType.LiveStockUsage });
                    }

                    if (switchLivestockSell.Text.Trim().ToUpper() == "YES")
                    {
                        objActivityDetail.IsLivestockSalable = true;
                        objActivityDetail.LastDateOfLivestockSold = DateTime.ParseExact(dateLastLivestockSell_EditText.Text.Trim(), "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);

                        if (spinnerlivestockSellWhoTxt.Trim().ToUpper() == "OTHERS")
                        {
                            objActivityDetail.LivestocksellingLocationId = 0;
                            objActivityDetail.LivestocksellingLocationName = input_livestockSellWhoOther.Text.Trim();
                            lstPrdResource.Add(new ProductResourcesResponse { ProductResourceId = "", ProductResourceName = objActivityDetail.LivestocksellingLocationName, ProductRessourceType = ProductRessourceType.LivestocksellingLocation });
                        }
                        else
                        {
                            objActivityDetail.LivestocksellingLocationId = ProductRessourceType.LivestocksellingLocation.GetHashCode();
                            objActivityDetail.LivestocksellingLocationName = spinnerlivestockSellWhoTxt.Trim();

                            var _livestocksellingLocationId = lstProductResources.Where(u => u.ProductRessourceType == ProductRessourceType.LivestocksellingLocation && u.ProductResourceName == objActivityDetail.LivestocksellingLocationName).Select(u => u.ProductResourceId).FirstOrDefault();
                            var _livestocksellingLocationIdVal = _livestocksellingLocationId == null ? string.Empty : _livestocksellingLocationId.ToString();
                            lstPrdResource.Add(new ProductResourcesResponse { ProductResourceId = _livestocksellingLocationIdVal, ProductResourceName = objActivityDetail.LivestocksellingLocationName, ProductRessourceType = ProductRessourceType.LivestocksellingLocation });
                        }


                        if (spinnerlivestockSellTypeTxt.Trim().ToUpper() == "OTHERS")
                        {
                            objActivityDetail.LiveStockUtilityId = 0;
                            objActivityDetail.LiveStockUtilityName = input_livestockSellTypeOther.Text.Trim();
                            lstPrdResource.Add(new ProductResourcesResponse { ProductResourceId = "", ProductResourceName = objActivityDetail.LiveStockUtilityName, ProductRessourceType = ProductRessourceType.LivestockUtility });
                        }
                        else
                        {
                            objActivityDetail.LiveStockUtilityId = ProductRessourceType.LivestockUtility.GetHashCode();
                            objActivityDetail.LiveStockUtilityName = spinnerlivestockSellTypeTxt.Trim();

                            var _liveStockUtilityId = lstProductResources.Where(u => u.ProductRessourceType == ProductRessourceType.LivestockUtility && u.ProductResourceName == objActivityDetail.LiveStockUtilityName).Select(u => u.ProductResourceId).FirstOrDefault();
                            var _liveStockUtilityIdVal = _liveStockUtilityId == null ? string.Empty : _liveStockUtilityId.ToString();
                            lstPrdResource.Add(new ProductResourcesResponse { ProductResourceId = _liveStockUtilityIdVal, ProductResourceName = objActivityDetail.LiveStockUtilityName, ProductRessourceType = ProductRessourceType.LivestockUtility });
                        }

                        objActivityDetail.SoldLiveStockAmount = Convert.ToDecimal(input_livestockSellNo.Text.Trim());

                    }
                    else
                    {
                        objActivityDetail.IsLivestockSalable = false;
                    }

                }
                else if (activityType == ProductType.Resource)
                {
                    objActivityDetail.NumberOfResource = Convert.ToInt32(input_resourceNumber.Text.Trim());
                    if (spinnerResourceOwnTypeTxt.Trim().ToUpper() == "OTHERS")
                    {
                        objActivityDetail.ResourceTypeId = 0;
                        objActivityDetail.ResourceTypeName = input_resourceOwnTypeOther.Text.Trim();
                        lstPrdResource.Add(new ProductResourcesResponse { ProductResourceId = "", ProductResourceName = objActivityDetail.ResourceTypeName, ProductRessourceType = ProductRessourceType.ResourceType });
                    }
                    else
                    {
                        objActivityDetail.ResourceTypeId = ProductRessourceType.ResourceType.GetHashCode();
                        objActivityDetail.ResourceTypeName = spinnerResourceOwnTypeTxt.Trim();

                        var _resourceTypeId = lstProductResources.Where(u => u.ProductRessourceType == ProductRessourceType.ResourceType && u.ProductResourceName == objActivityDetail.ResourceTypeName).Select(u => u.ProductResourceId).FirstOrDefault();
                        var _resourceTypeIdVal = _resourceTypeId == null ? string.Empty : _resourceTypeId.ToString();
                        lstPrdResource.Add(new ProductResourcesResponse { ProductResourceId = _resourceTypeIdVal, ProductResourceName = objActivityDetail.ResourceTypeName, ProductRessourceType = ProductRessourceType.ResourceType });
                    }

                    objActivityDetail.ResourceCostTypeId = ProductRessourceType.ResourceCostType.GetHashCode();
                    objActivityDetail.ResourceCostTypeName = spinnerResourceCostTxt.Trim();

                    var _resourceCostTypeId = lstProductResources.Where(u => u.ProductRessourceType == ProductRessourceType.ResourceCostType && u.ProductResourceName == objActivityDetail.ResourceCostTypeName).Select(u => u.ProductResourceId).FirstOrDefault();
                    var _resourceCostTypeIdVal = _resourceCostTypeId == null ? string.Empty : _resourceCostTypeId.ToString();
                    lstPrdResource.Add(new ProductResourcesResponse { ProductResourceId = _resourceCostTypeIdVal, ProductResourceName = objActivityDetail.ResourceCostTypeName, ProductRessourceType = ProductRessourceType.ResourceCostType });

                    objActivityDetail.ResourcePrice = Convert.ToDecimal(input_price.Text.Trim());
                    objActivityDetail.ResourceMaintenanceCostTypeId = ProductRessourceType.ResourceMaintenaceCostType.GetHashCode();
                    objActivityDetail.ResourceMaintenaceCostTypeName = spinnerResourceMaintenaceCostTxt.Trim();
                    objActivityDetail.ResourceMaintenancePrice = Convert.ToDecimal(input_maintenaceprice.Text.Trim());

                    var _resourceMaintenanceCostTypeId = lstProductResources.Where(u => u.ProductRessourceType == ProductRessourceType.ResourceMaintenaceCostType && u.ProductResourceName == objActivityDetail.ResourceMaintenaceCostTypeName).Select(u => u.ProductResourceId).FirstOrDefault();
                    var _resourceMaintenanceCostTypeIdVal = _resourceMaintenanceCostTypeId == null ? string.Empty : _resourceMaintenanceCostTypeId.ToString();
                    lstPrdResource.Add(new ProductResourcesResponse { ProductResourceId = _resourceMaintenanceCostTypeIdVal, ProductResourceName = objActivityDetail.ResourceMaintenaceCostTypeName, ProductRessourceType = ProductRessourceType.ResourceMaintenaceCostType });
                }

                if (lstPrdResource.Count() > 0)
                {
                    objActivityDetail.ProductResourceList = lstPrdResource;
                }

                //API call
                var client = new RestClient(Common.UrlBase);
                var request = new RestRequest("Activity/CreateUpdateActivity", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("TokenKey", mStringSessionToken);
                //request.AddJsonBody(objActivityDetail);
                request.AddParameter(new Parameter { Name = "application/json", Type = ParameterType.RequestBody, Value = Newtonsoft.Json.JsonConvert.SerializeObject(objActivityDetail, new NoColonIsoDateTimeConverter()) });
                IRestResponse response = client.Execute(request);
                var content = response.Content;
                var responseObj = Newtonsoft.Json.JsonConvert.DeserializeObject<ActivityDetailResponse>(content);

                if (responseObj != null && responseObj.Status == ResponseStatus.Successful)
                {
                    curActivity.RunOnUiThread(() =>
                    {
                        Android.App.AlertDialog.Builder alertDiag = new Android.App.AlertDialog.Builder(curActivity);
                        alertDiag.SetTitle(Resource.String.DialogHeaderGeneric);
                        alertDiag.SetMessage("Activity has been saved successfully");
                        alertDiag.SetIcon(Resource.Drawable.success);
                        alertDiag.SetPositiveButton(Resource.String.DialogButtonOk, (senderAlert, args) =>
                        {
                            Bundle utilBundle = new Bundle();
                            utilBundle.PutString("siteparamdate", objActivityDetail.ActivityDate.ToString("dd-MM-yyyy"));
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
                else
                {
                    if (!string.IsNullOrEmpty(responseObj.Error))
                        throw new Exception(responseObj.Error);
                    else
                        throw new Exception("Unable to save activity. Please try again later.");
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

        private bool ValidateForm(Activity currentActivity, ProductType ActivityType)
        {
            bool isValidate = true;
            string errorMsg = string.Empty;

            if (spinnerSelectPlotTxt.Trim() == "" || spinnerSelectPlotTxt.Trim().ToUpper() == "SELECT")
            {
                isValidate = false;
                errorMsg = "Please select plot.";
            }
            else if (activityDate_EditText.Text.Trim() == "")
            {
                isValidate = false;
                errorMsg = "Please enter activity date.";
            }
            else if (ActivityType == ProductType.Crop)
            {
                isValidate = ValidateCrop(ref errorMsg);
            }
            else if (ActivityType == ProductType.LiveStock)
            {
                isValidate = ValidateLiveStock(ref errorMsg);
            }
            else if (ActivityType == ProductType.Resource)
            {
                isValidate = ValidateResource(ref errorMsg);
            }

            if (!isValidate)
            {
                currentActivity.RunOnUiThread(() =>
                {
                    Android.App.AlertDialog.Builder alertDiag = new Android.App.AlertDialog.Builder(currentActivity);
                    alertDiag.SetTitle(Resource.String.DialogHeaderError);
                    alertDiag.SetMessage(errorMsg);
                    alertDiag.SetIcon(Resource.Drawable.alert);
                    alertDiag.SetPositiveButton(Resource.String.DialogButtonOk, (senderAlert, args) =>
                    {

                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }

            return isValidate;
        }

        private bool ValidateCrop(ref string ErrorMsg)
        {
            bool isValidate = true;

            if (date_EditText.Text.Trim() == "")
            {
                isValidate = false;
                ErrorMsg = "Please enter last plant date.";
            }
            else if (switchChooseHarvest.Text.Trim().ToUpper() == "YES" && dateLastHarvest_EditText.Text.Trim() == "")
            {
                isValidate = false;
                ErrorMsg = "Please select last harvest date.";
            }
            else if (switchSaleProduct.Text.Trim().ToUpper() == "NO" && input_saleProductDesc.Text.Trim() == "")
            {
                isValidate = false;
                ErrorMsg = "Please enter the reason for not harvest.";
            }
            else if (switchSaleProduct.Text.Trim().ToUpper() == "YES" && input_saleAmount.Text.Trim() == "")
            {
                isValidate = false;
                ErrorMsg = "Please enter the sale amount.";
            }

            return isValidate;
        }

        private bool ValidateLiveStock(ref string ErrorMsg)
        {
            bool isValidate = true;

            if (input_livestockCount.Text.Trim() == "")
            {
                isValidate = false;
                ErrorMsg = "Please enter livestock count.";
            }
            else if (spinnerlivestockForTxt.Trim() == "" || spinnerlivestockForTxt.Trim().ToUpper() == "SELECT")
            {
                isValidate = false;
                ErrorMsg = "Please select livestock use.";
            }
            else if (spinnerlivestockForTxt.Trim().ToUpper() == "OTHERS" && input_livestockForOther.Text.Trim() == "")
            {
                isValidate = false;
                ErrorMsg = "Please enter livestock other use.";
            }
            else if (switchLivestockSell.Text.Trim().ToUpper() == "YES")
            {
                if (dateLastLivestockSell_EditText.Text.Trim() == "")
                {
                    isValidate = false;
                    ErrorMsg = "Please enter livestock sell date.";
                }
                else if (spinnerlivestockSellTypeTxt.Trim() == "" || spinnerlivestockSellTypeTxt.Trim().ToUpper() == "SELECT")
                {
                    isValidate = false;
                    ErrorMsg = "Please select livestock sell type.";
                }
                else if (spinnerlivestockSellTypeTxt.Trim().ToUpper() == "OTHERS" && input_livestockSellTypeOther.Text.Trim() == "")
                {
                    isValidate = false;
                    ErrorMsg = "Please enter livestock other sell type.";
                }
                else if (input_livestockSellNo.Text.Trim() == "")
                {
                    isValidate = false;
                    ErrorMsg = "Please enter livestock sell count.";
                }
                else if (spinnerlivestockSellWhoTxt.Trim() == "" || spinnerlivestockSellWhoTxt.Trim().ToUpper() == "SELECT")
                {
                    isValidate = false;
                    ErrorMsg = "Please select livestock sell who.";
                }
                else if (spinnerlivestockSellWhoTxt.Trim().ToUpper() == "OTHERS" && input_livestockSellWhoOther.Text.Trim() == "")
                {
                    isValidate = false;
                    ErrorMsg = "Please enter livestock other sell who.";
                }
            }


            return isValidate;
        }

        private bool ValidateResource(ref string ErrorMsg)
        {
            bool isValidate = true;

            if (input_resourceNumber.Text.Trim() == "")
            {
                isValidate = false;
                ErrorMsg = "Please enter resource count.";
            }
            else if (spinnerResourceOwnTypeTxt.Trim() == "" || spinnerResourceOwnTypeTxt.Trim().ToUpper() == "SELECT")
            {
                isValidate = false;
                ErrorMsg = "Please select resource Type.";
            }
            else if (spinnerResourceOwnTypeTxt.Trim().ToUpper() == "OTHERS" && input_resourceOwnTypeOther.Text.Trim() == "")
            {
                isValidate = false;
                ErrorMsg = "Please enter resource other Type.";
            }
            else if (spinnerResourceCostTxt.Trim() == "" || spinnerResourceCostTxt.Trim().ToUpper() == "SELECT")
            {
                isValidate = false;
                ErrorMsg = "Please select resource cost type.";
            }
            else if (input_price.Text.Trim() == "")
            {
                isValidate = false;
                ErrorMsg = "Please enter resource cost value.";
            }
            else if (spinnerResourceMaintenaceCostTxt.Trim() == "" || spinnerResourceMaintenaceCostTxt.Trim().ToUpper() == "SELECT")
            {
                isValidate = false;
                ErrorMsg = "Please select resource maintenace cost type.";
            }
            else if (input_maintenaceprice.Text.Trim() == "")
            {
                isValidate = false;
                ErrorMsg = "Please enter resource cost value.";
            }

            return isValidate;
        }

        private List<ProductResourcesResponse> FillAllDropdown(string RequestURL)
        {
            List<ProductResourcesResponse> value = null;

            try
            {
                var client = new RestClient(Common.UrlBase);
                var request = new RestRequest(RequestURL, Method.GET);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("TokenKey", mStringSessionToken);
                IRestResponse response = client.Execute(request);
                var content = response.Content;
                var responseObj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProductResourcesResponse>>(content);

                if (responseObj != null && responseObj.Count() > default(int))
                {
                    value = responseObj;
                }

            }
            catch { }

            return value;
        }

        public List<PlotDetailResponse> GetAllPlotByUser()
        {
            List<PlotDetailResponse> lstPlot = new List<PlotDetailResponse>();

            var client = new RestClient(Common.UrlBase);
            var request = new RestRequest("Farm/GetFarmsDetails", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("TokenKey", mStringSessionToken);
            IRestResponse response = client.Execute(request);
            var content = response.Content;
            var responseObj = Newtonsoft.Json.JsonConvert.DeserializeObject<FarmDetailResponse>(content);

            if (responseObj != null && !string.IsNullOrEmpty(responseObj.FarmName))
            {
                string FrmId = responseObj.FarmId;

                client = new RestClient(Common.UrlBase);
                var requestPlot = new RestRequest("Plot/GetPlotListDetails", Method.GET);
                requestPlot.AddHeader("Content-Type", "application/json");
                requestPlot.AddHeader("TokenKey", mStringSessionToken);
                requestPlot.AddQueryParameter("farmid", System.Net.WebUtility.UrlEncode(FrmId));
                IRestResponse responsePlotObj = client.Execute(requestPlot);
                var contentPlot = responsePlotObj.Content;
                var responsePlot = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PlotDetailResponse>>(contentPlot);

                if (responsePlot != null && responsePlot.Count() > default(int))
                {
                    lstPlot = responsePlot;
                }
            }

            return lstPlot;
        }

        public List<ActivityDescriptions> GetActivityDescription(ProductType ActivityType)
        {
            List<ActivityDescriptions> lstActivityDescription = null;

            var client = new RestClient(Common.UrlBase);
            var request = new RestRequest("ProductResources/GetAllActivityDescriptions", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("TokenKey", mStringSessionToken);
            IRestResponse response = client.Execute(request);
            var content = response.Content;
            var responseObj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ActivityDescriptions>>(content);

            if (responseObj != null && responseObj.Count() > 0)
            {
                var allActivities = responseObj.Where(u => u.ProductType == ActivityType.GetHashCode());
                lstActivityDescription = allActivities != null ? allActivities.ToList() : new List<ActivityDescriptions>();
            }
            else
            {
                lstActivityDescription = new List<ActivityDescriptions>();
            }

            return lstActivityDescription;
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
                if (responseObj != null && responseObj.Count() > 0)
                {
                    oboActivity = responseObj.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }

            return oboActivity;
        }
    }

    public class OnDateSetListener : Java.Lang.Object, IOnDateSetListener
    {
        EditText _et;

        public OnDateSetListener(EditText ParentControl)
        {
            _et = ParentControl;
        }
        public void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
        {
            _et.Text = string.Format("{0}-{1}-{2}", dayOfMonth.ToString("00"), (month + 1).ToString("00"), year.ToString("0000"));
        }
    }
}