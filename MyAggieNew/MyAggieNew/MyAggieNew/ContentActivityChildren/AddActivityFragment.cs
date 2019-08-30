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
using System.IO;
using System.Linq;
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

namespace MyAggieNew
{
    public class AddActivityFragment : Android.Support.V4.App.Fragment
    {
        TextView txt_main, txt_item_lvl1;
        ImageView img_item_lvl1;
        GridView androidGridView;
        DBaseOperations objdb;
        string[] gridViewString;
        string[] gridViewCodeString;
        string[] gridViewTypeCodeString;
        Bitmap[] gridViewImages;
        IList<ItemPayloadModelWithBase64> objSelectedItem;

        public AddActivityFragment() { }

        public static Android.Support.V4.App.Fragment newInstance(Context context)
        {
            AddActivityFragment busrouteFragment = new AddActivityFragment();
            return busrouteFragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ViewGroup root = (ViewGroup)inflater.Inflate(Resource.Layout.fragment_content_addactivity, null);
            txt_main = root.FindViewById<TextView>(Resource.Id.txt_main);
            txt_item_lvl1 = root.FindViewById<TextView>(Resource.Id.txt_item_lvl1);
            img_item_lvl1 = root.FindViewById<ImageView>(Resource.Id.img_item_lvl1);

            try
            {
                if (Arguments != null)
                {
                    if (objSelectedItem == null)
                    {
                        objSelectedItem = new List<ItemPayloadModelWithBase64>();
                    }
                    objSelectedItem = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ItemPayloadModelWithBase64>>(Arguments.GetString("siteparam"));
                    if (objSelectedItem.FirstOrDefault() != null)
                    {
                        txt_main.Text = "Selected Category:";
                        img_item_lvl1.Visibility = ViewStates.Visible;
                        txt_item_lvl1.Visibility = ViewStates.Visible;
                        txt_item_lvl1.Text = objSelectedItem.FirstOrDefault() != null ? objSelectedItem.FirstOrDefault().ItemName : "";
                        img_item_lvl1.SetImageBitmap(BitmapHelpers.Base64ToBitmap(objSelectedItem.FirstOrDefault().ItemIcon));
                    }
                    else
                    {
                        txt_main.Text = "Select Sub-Category:";
                        img_item_lvl1.Visibility = ViewStates.Gone;
                        txt_item_lvl1.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    objSelectedItem = null;
                    txt_main.Text = "Select Category:";
                    img_item_lvl1.Visibility = ViewStates.Gone;
                    txt_item_lvl1.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception ex) { }

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

                if (objSelectedItem == null || objSelectedItem.Count() <= default(int))
                {
                    var client = new RestClient(Common.UrlBase);
                    var request = new RestRequest("Product/GetCategoryList", Method.GET);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddHeader("TokenKey", mStringSessionToken);
                    IRestResponse response = client.Execute(request);
                    var content = response.Content;
                    var responseObj = Newtonsoft.Json.JsonConvert.DeserializeObject<IList<CategoryMasterResponse>>(content);
                    if (responseObj != null && responseObj.Count() > default(int))
                    {
                        var gtypecodes = new List<string>();
                        var gstrings = responseObj.Select(r => r.ProductTypeName).ToList();
                        var gcodes = responseObj.Select(r => r.ProductTypeId).ToList();
                        //responseObj.Select(r => r.pr)
                        var gimages = BitmapHelpers.GetImageListFromUrlList(responseObj.Select(r => r.catImageName).ToList(), mStringSessionToken, this.Activity.Resources);
                        gstrings.Add("Back to Dashboard");
                        gcodes.Add("BCK");
                        foreach (var x in gcodes)
                        {
                            gtypecodes.Add(ProductType.None.GetHashCode().ToString());
                        }
                        gimages.Add(BitmapFactory.DecodeResource(this.Activity.Resources, Resource.Drawable.back));

                        gridViewString = gstrings.ToArray();
                        gridViewCodeString = gcodes.ToArray();
                        gridViewTypeCodeString = gtypecodes.ToArray();
                        gridViewImages = gimages.ToArray();
                    }
                    else
                    {
                        throw new Exception("No item found");
                    }
                }
                else if (objSelectedItem.Count() > default(int) && !string.IsNullOrEmpty(objSelectedItem.FirstOrDefault().ItemCode))
                {
                    var gstrings = new List<string>();
                    var gcodes = new List<string>();
                    var gtypecodes = new List<string>();
                    IList<Bitmap> gimages = new List<Bitmap>();

                    var client = new RestClient(Common.UrlBase);
                    var request = new RestRequest("Product/GetSubCategoryList", Method.GET);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddHeader("TokenKey", mStringSessionToken);
                    request.AddQueryParameter("catId", System.Net.WebUtility.UrlEncode(objSelectedItem.FirstOrDefault().ItemCode.Replace("Ø", "")));
                    IRestResponse response = client.Execute(request);
                    var content = response.Content;
                    var responseObj = Newtonsoft.Json.JsonConvert.DeserializeObject<IList<ProductDetailResponse>>(content);
                    if (responseObj != null && responseObj.Count() > default(int))
                    {
                        gstrings = responseObj.Select(r => r.ProductName).ToList();
                        gcodes = responseObj.Select(r => r.ProductId).ToList();
                        gtypecodes = responseObj.Select(r => r.prodType.GetHashCode().ToString()).ToList();
                        gimages = BitmapHelpers.GetImageListFromUrlList(responseObj.Select(r => r.prodImageName).ToList(), mStringSessionToken, this.Activity.Resources);
                        gstrings.Add(string.Format("Back to {0}", responseObj.FirstOrDefault().ProductTypeName));
                        gcodes.Add(string.Format("Ø{0}", responseObj.Select(r => r.CategoryID).FirstOrDefault()));
                        gtypecodes.Add(ProductType.None.GetHashCode().ToString());
                        gimages.Add(BitmapFactory.DecodeResource(this.Activity.Resources, Resource.Drawable.back));
                    }

                    gstrings.Add("Back to Dashboard");
                    gcodes.Add("BCK");
                    gtypecodes.Add(ProductType.None.GetHashCode().ToString());
                    gimages.Add(BitmapFactory.DecodeResource(this.Activity.Resources, Resource.Drawable.backtoprevious));

                    gstrings.Add("Add New Product");
                    gcodes.Add("NPR");
                    gtypecodes.Add(ProductType.None.GetHashCode().ToString());
                    gimages.Add(BitmapFactory.DecodeResource(this.Activity.Resources, Resource.Drawable.addprd));

                    gridViewString = gstrings.ToArray();
                    gridViewCodeString = gcodes.ToArray();
                    gridViewTypeCodeString = gtypecodes.ToArray();
                    gridViewImages = gimages.ToArray();
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
                        DashboardFragment objFragment = new DashboardFragment();
                        Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                        tx.Replace(Resource.Id.m_main, objFragment, Constants.dashboard);
                        tx.Commit();
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }

            try
            {
                if (gridViewCodeString.Count() == gridViewImages.Count())
                {
                    _generic_grid_menu_bitmap_helper adapterViewAndroid = new _generic_grid_menu_bitmap_helper(this.Activity, gridViewString, gridViewCodeString, gridViewTypeCodeString, gridViewImages);
                    androidGridView = root.FindViewById<GridView>(Resource.Id.grid_view_activities);
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
                    throw new Exception("No data available. Please report to admin");
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
                        DashboardFragment objFragment = new DashboardFragment();
                        Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                        tx.Replace(Resource.Id.m_main, objFragment, Constants.dashboard);
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
                if (gridViewCodeString[e.Position] == "BCK")
                {
                    DashboardFragment objFragment = new DashboardFragment();
                    Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                    tx.Replace(Resource.Id.m_main, objFragment, Constants.dashboard);
                    tx.Commit();
                }
                else if (gridViewCodeString[e.Position] == "NPR")
                {
                    if (objSelectedItem == null)
                    {
                        objSelectedItem = new List<ItemPayloadModelWithBase64>();
                    }
                    objSelectedItem.Add(new ItemPayloadModelWithBase64()
                    {
                        ItemName = gridViewString[e.Position],
                        ItemCode = gridViewCodeString[e.Position],
                        ItemIcon = BitmapHelpers.BitmapToBase64(gridViewImages[e.Position]),
                        prdType = (ProductType)Convert.ToInt32(gridViewTypeCodeString[e.Position])
                    });

                    Bundle utilBundle = new Bundle();
                    utilBundle.PutString("siteparam", Newtonsoft.Json.JsonConvert.SerializeObject(objSelectedItem));
                    AddProductFragment objFragment = new AddProductFragment();
                    objFragment.Arguments = utilBundle;
                    Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                    tx.Replace(Resource.Id.m_main, objFragment, Constants.dashboard);
                    tx.Commit();
                }
                else
                {
                    //Identification of parent selection
                    if (gridViewCodeString[e.Position].Contains("Ø"))
                    {
                        objSelectedItem = null;
                    }

                    if (objSelectedItem == null)
                    {
                        objSelectedItem = new List<ItemPayloadModelWithBase64>();
                    }
                    objSelectedItem.Add(new ItemPayloadModelWithBase64()
                    {
                        ItemName = gridViewString[e.Position],
                        ItemCode = gridViewCodeString[e.Position],
                        ItemIcon = BitmapHelpers.BitmapToBase64(gridViewImages[e.Position]),
                        prdType = (ProductType)Convert.ToInt32(gridViewTypeCodeString[e.Position])
                    });

                    if (gridViewCodeString[e.Position].Contains("Ø"))
                    {
                        AddActivityFragment objFragment = new AddActivityFragment();
                        Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                        tx.Replace(Resource.Id.m_main, objFragment, Constants.addactivity);
                        tx.Commit();
                    }
                    else
                    {
                        Bundle utilBundle = new Bundle();
                        if (objSelectedItem == null || objSelectedItem.Count() <= default(int))
                        {
                            AddActivityFragment objFragment = new AddActivityFragment();
                            Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                            tx.Replace(Resource.Id.m_main, objFragment, Constants.addactivity);
                            tx.Commit();
                        }
                        else if (objSelectedItem != null && objSelectedItem.Count() > default(int) && objSelectedItem.Count() <= 1)
                        {
                            utilBundle.PutString("siteparam", Newtonsoft.Json.JsonConvert.SerializeObject(objSelectedItem));
                            AddActivityFragment objFragment = new AddActivityFragment();
                            objFragment.Arguments = utilBundle;
                            Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                            tx.Replace(Resource.Id.m_main, objFragment, Constants.addactivity);
                            tx.Commit();
                        }
                        else if (objSelectedItem != null && objSelectedItem.Count() > 1)
                        {
                            utilBundle.PutString("siteparam", Newtonsoft.Json.JsonConvert.SerializeObject(objSelectedItem));
                            AddSelectedItemFragment objFragment = new AddSelectedItemFragment();
                            objFragment.Arguments = utilBundle;
                            Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                            tx.Replace(Resource.Id.m_main, objFragment, Constants.addactivity);
                            tx.Commit();
                        }
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
    }
}