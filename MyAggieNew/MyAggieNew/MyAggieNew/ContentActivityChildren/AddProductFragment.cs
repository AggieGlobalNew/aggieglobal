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
using Android;
using Android.Content.PM;
using Android.Hardware;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Refractored.Controls;
using RestSharp;
using System.Threading;
using Android.Provider;
using Java.IO;
using Uri = Android.Net.Uri;
using Android.Graphics;

namespace MyAggieNew
{
    public class AddProductFragment : Android.Support.V4.App.Fragment //, ActivityCompat.IOnRequestPermissionsResultCallback
    {
        TextView txt_lvl_1;
        ImageView img_lvl_1, img_product_view;
        EditText txt_ProductName;
        Button btn_show_popup, btn_prd_cancel, btn_prd_save;
        IList<ItemPayloadModelWithBase64> objSelectedItem;

        public static File _file;
        public static File _dir;
        public static bool gotFile = default(bool);
        DBaseOperations objdb;

        public AddProductFragment() { }

        public static Android.Support.V4.App.Fragment newInstance(Context context)
        {
            AddProductFragment busrouteFragment = new AddProductFragment();
            return busrouteFragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ViewGroup root = (ViewGroup)inflater.Inflate(Resource.Layout.fragment_content_addproduct, null);

            StrictMode.VmPolicy.Builder builder = new StrictMode.VmPolicy.Builder();
            StrictMode.SetVmPolicy(builder.Build());

            txt_lvl_1 = root.FindViewById<TextView>(Resource.Id.txt_lvl_1);
            img_lvl_1 = root.FindViewById<ImageView>(Resource.Id.img_lvl_1);
            img_product_view = root.FindViewById<ImageView>(Resource.Id.img_product_view);
            txt_ProductName = root.FindViewById<EditText>(Resource.Id.txt_ProductName);
            btn_show_popup = root.FindViewById<Button>(Resource.Id.btn_show_popup);
            btn_prd_cancel = root.FindViewById<Button>(Resource.Id.btn_prd_cancel);
            btn_prd_cancel.Click += (sndr, argus) => Cancel_Product(sndr, argus, this.Activity);
            btn_prd_save = root.FindViewById<Button>(Resource.Id.btn_prd_save);
            btn_prd_save.Click += (sndr, argus) => Save_Product(sndr, argus, this.Activity);
            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();
                btn_show_popup.Click += (sndr, argus) => Camera_Clicked(sndr, argus, this.Activity, btn_show_popup);
            }

            try
            {
                if (Arguments != null)
                {
                    if (objSelectedItem == null)
                    {
                        objSelectedItem = new List<ItemPayloadModelWithBase64>();
                    }
                    objSelectedItem = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ItemPayloadModelWithBase64>>(Arguments.GetString("siteparam"));
                    txt_lvl_1.Text = objSelectedItem[0].ItemName;
                    img_lvl_1.SetImageBitmap(BitmapHelpers.Base64ToBitmap(objSelectedItem[0].ItemIcon));
                }

                if (Build.VERSION.SdkInt >= Build.VERSION_CODES.M)
                {
                    if (Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.Camera) != (int)Permission.Granted || Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.WriteExternalStorage) != (int)Permission.Granted)
                    {
                        Android.Support.V4.App.ActivityCompat.RequestPermissions(this.Activity, new string[] { Manifest.Permission.Camera, Manifest.Permission.WriteExternalStorage, Manifest.Permission.ReadExternalStorage }, 54);
                        /*FragmentManager.FindFragmentById(Resource.Layout.fragment_content_addproduct).RequestPermissions(new string[]
                        {
                            Manifest.Permission.Camera, Manifest.Permission.WriteExternalStorage,
                            Manifest.Permission.ReadExternalStorage
                        }, 54);*/
                    }
                }
            }
            catch { }

            return root;
        }

        private void CameraOpen()
        {
            Intent CamIntent = new Intent(MediaStore.ActionImageCapture);
            _file = new File(_dir, string.Format("Image_{0}.jpg", Guid.NewGuid()));
            CamIntent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(_file));
            CamIntent.PutExtra("return-data", true);
            StartActivityForResult(CamIntent, 0);
        }

        private void GalleryOpen()
        {
            Intent GalIntent = new Intent(Intent.ActionPick, MediaStore.Images.Media.ExternalContentUri);
            StartActivityForResult(GalIntent, 2);

            /*Intent chooser = Intent.CreateChooser(GalIntent, "Some text here");
            chooser.PutExtra(Intent.ExtraInitialIntents, new Intent[] { new Intent(MediaStore.ActionImageCapture) });
            StartActivityForResult(chooser, 2);*/
        }

        private void Save_Product(object sender, EventArgs e, Activity currentActivity)
        {
            btn_prd_save.Click -= (sndr, argus) => Save_Product(sndr, argus, currentActivity);
            try
            {
                if (!string.IsNullOrWhiteSpace(txt_ProductName.Text.Trim()))
                {
                    if (gotFile)
                    {
                        CommonModuleResponse obj = new CommonModuleResponse();

                        img_product_view.BuildDrawingCache(true);
                        Bitmap bmap = img_product_view.GetDrawingCache(true);
                        img_product_view.SetImageBitmap(bmap);
                        obj.fileStream = BitmapHelpers.BitmapToByteArray(Bitmap.CreateBitmap(img_product_view.GetDrawingCache(true)));

                        obj.productdata = new ProductDetailResponse();
                        obj.productdata.ProductName = txt_ProductName.Text.Trim();
                        obj.productdata.CategoryID = objSelectedItem[0].ItemCode;
                        obj.productdata.ProductTypeName = objSelectedItem[0].ItemName;

                        ProgressDialog progressDialog = ProgressDialog.Show(this.Activity, "Please wait...", "Saving your product...", true);
                        new Thread(new ThreadStart(delegate
                        {
                            currentActivity.RunOnUiThread(() => this.SaveProduct(progressDialog, currentActivity, obj));
                        })).Start();
                    }
                    else
                    {
                        throw new Exception("Please capture or chose product image");
                    }
                }
                else
                {
                    throw new Exception("Please provide your product name");
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
                        //btn_prd_save.Click += (sndr, argus) => Save_Product(sndr, argus, currentActivity);
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }

        private void SaveProduct(ProgressDialog dialog, Activity curActivity, CommonModuleResponse obj)
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

                //var x = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

                var client = new RestClient(Common.UrlBase);
                var request = new RestRequest("Product/CreateSubCategory", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("TokenKey", mStringSessionToken);
                //request.AddJsonBody(obj);
                request.AddParameter(new Parameter { Name = "application/json", Type = ParameterType.RequestBody, Value = Newtonsoft.Json.JsonConvert.SerializeObject(obj, new NoColonIsoDateTimeConverter()) });
                IRestResponse response = client.Execute(request);
                var content = response.Content;
                var responseObj = Newtonsoft.Json.JsonConvert.DeserializeObject<CommonModuleResponse>(content);
                if (responseObj != null && responseObj.productdata != null && !string.IsNullOrEmpty(responseObj.productdata.ProductId))
                {
                    curActivity.RunOnUiThread(() =>
                    {
                        Android.App.AlertDialog.Builder alertDiag = new Android.App.AlertDialog.Builder(curActivity);
                        alertDiag.SetTitle(Resource.String.DialogHeaderGeneric);
                        alertDiag.SetMessage(string.Format("Your product '{0}' has been saved successfully", obj.productdata.ProductName));
                        alertDiag.SetIcon(Resource.Drawable.success);
                        alertDiag.SetPositiveButton(Resource.String.DialogButtonOk, (senderAlert, args) =>
                        {
                            var siteparam = new List<ItemPayloadModelWithBase64>();
                            siteparam.Add(objSelectedItem.FirstOrDefault());
                            Bundle utilBundle = new Bundle();
                            utilBundle.PutString("siteparam", Newtonsoft.Json.JsonConvert.SerializeObject(siteparam));
                            AddActivityFragment objFragment = new AddActivityFragment();
                            objFragment.Arguments = utilBundle;
                            Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                            tx.Replace(Resource.Id.m_main, objFragment, Constants.dashboard);
                            tx.Commit();
                        });
                        Dialog diag = alertDiag.Create();
                        diag.Show();
                        diag.SetCanceledOnTouchOutside(false);
                    });
                }
                else
                {
                    if (responseObj != null && !string.IsNullOrEmpty(responseObj.Error))
                    {
                        throw new Exception(responseObj.Error);
                    }
                    else
                    {
                        throw new Exception("Unable to save product right now. Please try again later");
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

        private void Cancel_Product(object sender, EventArgs e, Activity currentActivity)
        {
            btn_prd_cancel.Click -= (sndr, argus) => Cancel_Product(sndr, argus, currentActivity);

            try
            {
                currentActivity.RunOnUiThread(() =>
                {
                    Android.App.AlertDialog.Builder alertDiag = new Android.App.AlertDialog.Builder(currentActivity);
                    alertDiag.SetTitle(Resource.String.DialogHeaderGeneric);
                    alertDiag.SetMessage("Are you sure you want to cancel?");
                    alertDiag.SetIcon(Resource.Drawable.alert);
                    alertDiag.SetPositiveButton(Resource.String.DialogButtonYes, (senderAlert, args) =>
                    {
                        var siteparam = new List<ItemPayloadModelWithBase64>();
                        siteparam.Add(objSelectedItem.FirstOrDefault());
                        Bundle utilBundle = new Bundle();
                        utilBundle.PutString("siteparam", Newtonsoft.Json.JsonConvert.SerializeObject(siteparam));
                        AddActivityFragment objFragment = new AddActivityFragment();
                        objFragment.Arguments = utilBundle;
                        Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                        tx.Replace(Resource.Id.m_main, objFragment, Constants.dashboard);
                        tx.Commit();
                    });
                    alertDiag.SetNegativeButton(Resource.String.DialogButtonNo, (senderAlert, args) =>
                    {
                        //btn_prd_cancel.Click += (sndr, argus) => Cancel_Product(sndr, argus, currentActivity);
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
                        //btn_prd_cancel.Click += (sndr, argus) => Cancel_Product(sndr, argus, currentActivity);
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }

        public void Camera_Clicked(object sender, EventArgs e, Activity currentActivity, Button btn)
        {
            btn_show_popup.Click -= (sndr, argus) => Camera_Clicked(sndr, argus, currentActivity, btn);
            try
            {
                if (Build.VERSION.SdkInt >= Build.VERSION_CODES.M)
                {
                    if (Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.Camera) != (int)Permission.Granted || Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.WriteExternalStorage) != (int)Permission.Granted)
                    {
                        Android.Support.V4.App.ActivityCompat.RequestPermissions(this.Activity, new string[] { Manifest.Permission.Camera, Manifest.Permission.WriteExternalStorage, Manifest.Permission.ReadExternalStorage }, 54);
                        /*FragmentManager.FindFragmentById(Resource.Layout.fragment_content_addproduct).RequestPermissions(new string[]
                        {
                            Manifest.Permission.Camera, Manifest.Permission.WriteExternalStorage,
                            Manifest.Permission.ReadExternalStorage
                        }, 54);*/
                    }
                }

                /*PopupMenu menu = new PopupMenu(currentActivity.ApplicationContext, btn);
                menu.Inflate(Resource.Menu.pop_action_menu);
                menu.MenuItemClick += (sndr, argus) => Menu_MenuItemClick(sndr, argus, this.Activity, menu);
                menu.Show();
                btn_show_popup.Click += (sndr, argus) => Camera_Clicked(sndr, argus, currentActivity, btn);*/

                currentActivity.RunOnUiThread(() =>
                {
                    Android.App.AlertDialog.Builder alertDiag = new Android.App.AlertDialog.Builder(currentActivity);
                    alertDiag.SetTitle("Select Source");
                    alertDiag.SetMessage("Please select the image source, from where you want to get the icon");
                    alertDiag.SetIcon(Resource.Drawable.nash);
                    alertDiag.SetPositiveButton("Open Camera", (senderAlert, args) =>
                    {
                        alertDiag.Dispose();
                        CameraOpen();
                    });
                    alertDiag.SetNegativeButton("Open Gallery", (senderAlert, args) =>
                    {
                        alertDiag.Dispose();
                        GalleryOpen();
                    });
                    alertDiag.SetNeutralButton("Cancel", (senderAlert, args) =>
                    {
                        //btn_show_popup.Click += (sndr, argus) => Camera_Clicked(sndr, argus, currentActivity, btn);
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
                        //btn_show_popup.Click += (sndr, argus) => Camera_Clicked(sndr, argus, currentActivity, btn);
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }

        /*private void Menu_MenuItemClick(object sender, PopupMenu.MenuItemClickEventArgs e, Activity currentActivity, PopupMenu m)
        {
            try
            {
                switch (e.Item.ItemId)
                {
                    case Resource.Id.m_OpenCamera:
                        CameraOpen();
                        break;
                    case Resource.Id.m_OpenGallery:
                        GalleryOpen();
                        break;
                    default:
                        break;
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
                        //btn_show_popup.Click += (sndr, argus) => Camera_Clicked(sndr, argus, currentActivity, btn);
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
            finally
            {
                m.Dismiss();
            }
        }*/

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            try
            {
                if (requestCode == 0 && resultCode == (int)Result.Ok)
                {
                    CropImage(this.Activity);
                }
                else if (requestCode == 2)
                {
                    if (data != null)
                    {
                        CropImage(this.Activity, data.Data);
                    }
                }
                else if (requestCode == 1)
                {
                    if (data != null && data.Data != null)
                    {
                        using (Android.Graphics.Bitmap bitmap = data.Data.Path.LoadAndResizeBitmap(Resources.DisplayMetrics.WidthPixels, img_product_view.Height))
                        {
                            img_product_view.RecycleBitmap();
                            img_product_view.SetImageBitmap(bitmap);
                            gotFile = true;
                        }
                    }
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

                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            try
            {
                //PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                //base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                switch (requestCode)
                {
                    case 54:
                        {
                            if (grantResults.Length > default(int) && grantResults[0] == Permission.Granted)
                            {
                                //Permission granted
                                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                            }
                            else
                            {
                                throw new Exception("You have declined to allow the app to access your camera and/or external media");
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                this.Activity.RunOnUiThread(() =>
                {
                    Android.App.AlertDialog.Builder alertDiag = new Android.App.AlertDialog.Builder(this.Activity);
                    alertDiag.SetTitle(Resource.String.DialogHeaderError);
                    //alertDiag.SetMessage("Your device doesn't support the crop action");
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

        private void CropImage(Context cont, Uri uri = null)
        {
            Uri _uri, _newUri;
            try
            {
                if (uri == null)
                {
                    _uri = Uri.FromFile(_file);
                }
                else
                {
                    _uri = uri;
                }

                try
                {
                    string absolutefilePath = BitmapHelpers.GetRealPathFromUri(cont, _uri);
                    _newUri = Uri.Parse(absolutefilePath.StartsWith("file://") ? absolutefilePath : string.Format("file://{0}", absolutefilePath));
                }
                catch
                {
                    _newUri = _uri;
                }

                Intent CropIntent = new Intent("com.android.camera.action.CROP");
                CropIntent.SetDataAndType(_uri, "image/*");

                CropIntent.PutExtra("crop", "true");
                CropIntent.PutExtra("outputX", 180);
                CropIntent.PutExtra("outputY", 180);
                CropIntent.PutExtra("aspectX", 1);
                CropIntent.PutExtra("aspectY", 1);
                CropIntent.PutExtra("scaleUpIfNeeded", true);
                CropIntent.PutExtra("return-data", true);
                CropIntent.PutExtra("outputFormat", Bitmap.CompressFormat.Jpeg.ToString());
                CropIntent.PutExtra(MediaStore.ExtraOutput, _newUri);
                CropIntent.AddFlags(ActivityFlags.GrantReadUriPermission);
                CropIntent.AddFlags(ActivityFlags.GrantWriteUriPermission);

                StartActivityForResult(CropIntent, 1);
            }
            catch (ActivityNotFoundException ex)
            {
                this.Activity.RunOnUiThread(() =>
                {
                    Android.App.AlertDialog.Builder alertDiag = new Android.App.AlertDialog.Builder(this.Activity);
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

        private void CreateDirectoryForPictures()
        {
            _dir = new File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), "myaggie");
            if (!_dir.Exists())
            {
                _dir.Mkdirs();
            }
        }

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities = Context.PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }
    }
}