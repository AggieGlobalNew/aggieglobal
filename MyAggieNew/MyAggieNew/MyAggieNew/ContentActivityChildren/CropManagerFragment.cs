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
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

namespace MyAggieNew
{
    public class CropManagerFragment : Android.Support.V4.App.Fragment
    {
        GridView androidGridView;
        ImageView imgCrop;
        TextView txt_croptitle;
        string[] gridViewString;
        string[] gridViewCodeString;
        int[] gridViewImageId;
        IList<ItemPayloadModel> lstbase;
        ItemPayloadModel objbase_level1, objbase_level2;

        public CropManagerFragment() { }

        public static Android.Support.V4.App.Fragment newInstance(Context context)
        {
            CropManagerFragment busrouteFragment = new CropManagerFragment();
            return busrouteFragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ViewGroup root = (ViewGroup)inflater.Inflate(Resource.Layout.fragment_content_cropmanager, null);

            try
            {
                lstbase = new List<ItemPayloadModel>();
                objbase_level1 = new ItemPayloadModel();
                objbase_level1 = Newtonsoft.Json.JsonConvert.DeserializeObject<ItemPayloadModel>(Arguments.GetString("siteparam"));
                imgCrop = root.FindViewById<ImageView>(Resource.Id.imgCrop);
                imgCrop.SetImageResource(objbase_level1.ItemIcon);
                txt_croptitle = root.FindViewById<TextView>(Resource.Id.txt_croptitle);
                txt_croptitle.Text = objbase_level1.ItemName;
                lstbase.Add(objbase_level1);
            }
            catch { }

            gridViewString = new string[] { "Corn", "Barley", "Onion", "Sweet Potato", "Flour", "Back" };
            gridViewCodeString = new string[] { "Corn", "Barley", "Onion", "SweetPotato", "Flour", "BCK" };
            gridViewImageId = new int[] { Resource.Drawable.ic_corn, Resource.Drawable.ic_barley, Resource.Drawable.ic_onion, Resource.Drawable.ic_sweetpotato, Resource.Drawable.ic_flour, Resource.Drawable.ic_back };

            _generic_grid_menu_helper adapterViewAndroid = new _generic_grid_menu_helper(this.Activity, gridViewString, gridViewCodeString, gridViewImageId);
            androidGridView = root.FindViewById<GridView>(Resource.Id.grid_view_crops);
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

            return root;
        }

        private void ItemSearch_clicked(object sender, AdapterView.ItemClickEventArgs e, Activity currentActivity)
        {
            try
            {
                androidGridView.ItemClick -= (sndr, argus) => ItemSearch_clicked(sndr, argus, currentActivity);

                if (lstbase == null)
                {
                    lstbase = new List<ItemPayloadModel>();
                }

                switch (gridViewCodeString[e.Position])
                {
                    case "BCK":
                        {
                            AddActivityFragment objFragment = new AddActivityFragment();
                            Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                            tx.Replace(Resource.Id.m_main, objFragment, Constants.addactivity);
                            tx.Commit();
                            break;
                        }
                    case "Corn":
                        {
                            AddSelectedItemFragment objFragment = new AddSelectedItemFragment();

                            Bundle utilBundle = new Bundle();
                            objbase_level2 = new ItemPayloadModel()
                            {
                                ItemName = gridViewString[e.Position],
                                ItemCode = gridViewCodeString[e.Position],
                                ItemIcon = gridViewImageId[e.Position]
                            };
                            lstbase.Add(objbase_level2);
                            utilBundle.PutString("siteparam", Newtonsoft.Json.JsonConvert.SerializeObject(lstbase));
                            utilBundle.PutString("parent_tag", Constants.cropmanager);
                            objFragment.Arguments = utilBundle;

                            Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                            tx.Replace(Resource.Id.m_main, objFragment, Constants.addselecteditem);
                            tx.Commit();
                            break;
                        }
                    case "Barley":
                        {
                            AddSelectedItemFragment objFragment = new AddSelectedItemFragment();

                            Bundle utilBundle = new Bundle();
                            objbase_level2 = new ItemPayloadModel()
                            {
                                ItemName = gridViewString[e.Position],
                                ItemCode = gridViewCodeString[e.Position],
                                ItemIcon = gridViewImageId[e.Position]
                            };
                            lstbase.Add(objbase_level2);
                            utilBundle.PutString("siteparam", Newtonsoft.Json.JsonConvert.SerializeObject(lstbase));
                            utilBundle.PutString("parent_tag", Constants.cropmanager);
                            objFragment.Arguments = utilBundle;

                            Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                            tx.Replace(Resource.Id.m_main, objFragment, Constants.addselecteditem);
                            tx.Commit();
                            break;
                        }
                    case "Onion":
                        {
                            AddSelectedItemFragment objFragment = new AddSelectedItemFragment();

                            Bundle utilBundle = new Bundle();
                            objbase_level2 = new ItemPayloadModel()
                            {
                                ItemName = gridViewString[e.Position],
                                ItemCode = gridViewCodeString[e.Position],
                                ItemIcon = gridViewImageId[e.Position]
                            };
                            lstbase.Add(objbase_level2);
                            utilBundle.PutString("siteparam", Newtonsoft.Json.JsonConvert.SerializeObject(lstbase));
                            utilBundle.PutString("parent_tag", Constants.cropmanager);
                            objFragment.Arguments = utilBundle;

                            Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                            tx.Replace(Resource.Id.m_main, objFragment, Constants.addselecteditem);
                            tx.Commit();
                            break;
                        }
                    case "SweetPotato":
                        {
                            AddSelectedItemFragment objFragment = new AddSelectedItemFragment();

                            Bundle utilBundle = new Bundle();
                            objbase_level2 = new ItemPayloadModel()
                            {
                                ItemName = gridViewString[e.Position],
                                ItemCode = gridViewCodeString[e.Position],
                                ItemIcon = gridViewImageId[e.Position]
                            };
                            lstbase.Add(objbase_level2);
                            utilBundle.PutString("siteparam", Newtonsoft.Json.JsonConvert.SerializeObject(lstbase));
                            utilBundle.PutString("parent_tag", Constants.cropmanager);
                            objFragment.Arguments = utilBundle;

                            Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                            tx.Replace(Resource.Id.m_main, objFragment, Constants.addselecteditem);
                            tx.Commit();
                            break;
                        }
                    case "Flour":
                        {
                            AddSelectedItemFragment objFragment = new AddSelectedItemFragment();

                            Bundle utilBundle = new Bundle();
                            objbase_level2 = new ItemPayloadModel()
                            {
                                ItemName = gridViewString[e.Position],
                                ItemCode = gridViewCodeString[e.Position],
                                ItemIcon = gridViewImageId[e.Position]
                            };
                            lstbase.Add(objbase_level2);
                            utilBundle.PutString("siteparam", Newtonsoft.Json.JsonConvert.SerializeObject(lstbase));
                            utilBundle.PutString("parent_tag", Constants.cropmanager);
                            objFragment.Arguments = utilBundle;

                            Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                            tx.Replace(Resource.Id.m_main, objFragment, Constants.addselecteditem);
                            tx.Commit();
                            break;
                        }
                    default:
                        {
                            androidGridView.ItemClick += (sndr, argus) => ItemSearch_clicked(sndr, argus, currentActivity);
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
    }
}