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
//using Android.Graphics;
using Uri = Android.Net.Uri;

namespace MyAggieNew
{
    public class MyProfileFragment : Android.Support.V4.App.Fragment
    {
        TextView txt_user_name, txt_email;
        CircleImageView iv_camera0, profile_image0;

        public static File _file;
        public static File _dir;
        DBaseOperations objdb;
        public static string _profilepicbase64;

        public MyProfileFragment() { }

        public static Android.Support.V4.App.Fragment newInstance(Context context)
        {
            MyProfileFragment busrouteFragment = new MyProfileFragment();
            return busrouteFragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ViewGroup root = (ViewGroup)inflater.Inflate(Resource.Layout.fragment_content_profile, null);
            StrictMode.VmPolicy.Builder builder = new StrictMode.VmPolicy.Builder();
            StrictMode.SetVmPolicy(builder.Build());

            txt_user_name = root.FindViewById<TextView>(Resource.Id.txt_user_name);
            txt_email = root.FindViewById<TextView>(Resource.Id.txt_email);

            try
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
            catch { }

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
                var request = new RestRequest("UserAccount/GetUserDetails", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("TokenKey", mStringSessionToken);
                IRestResponse response = client.Execute(request);
                var content = response.Content;
                var responseObj = Newtonsoft.Json.JsonConvert.DeserializeObject<Account>(content);
                if (responseObj.Status == ResponseStatus.Successful && !string.IsNullOrEmpty(responseObj.EmailId))
                {
                    txt_user_name.Text = string.Format("{0} {1}", responseObj.FirstName, responseObj.LastName);
                    txt_email.Text = responseObj.EmailId;
                }
            }
            catch (Exception ex)
            {
                Intent intent = new Intent(this.Activity, typeof(MainActivity));
                StartActivity(intent);
                this.Activity.Finish();
            }

            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();
                profile_image0 = root.FindViewById<CircleImageView>(Resource.Id.profile_image0);
                iv_camera0 = root.FindViewById<CircleImageView>(Resource.Id.iv_camera0);
                iv_camera0.Click += (sndr, argus) => Camera_Clicked(sndr, argus, this.Activity);
                GetAndSetExistingProfilePic(profile_image0);
            }

            return root;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
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

        public void Camera_Clicked(object sender, EventArgs e, Activity currentActivity)
        {
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
                    else
                    {
                        Intent intent = new Intent(MediaStore.ActionImageCapture);
                        _file = new File(_dir, string.Format("Image_{0}.jpg", Guid.NewGuid()));
                        intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(_file));
                        StartActivityForResult(intent, 102);
                    }
                }
                else
                {
                    Intent intent = new Intent(MediaStore.ActionImageCapture);
                    _file = new File(_dir, string.Format("Image_{0}.jpg", Guid.NewGuid()));
                    intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(_file));
                    StartActivityForResult(intent, 102);
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
                        //btn_login.Click += (sndr, argus) => Login_Clicked(sndr, argus, currentActivity);
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            try
            {
                if (Build.VERSION.SdkInt >= Build.VERSION_CODES.M)
                {
                    if (Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.WriteExternalStorage) != (int)Permission.Granted)
                    {
                        Android.Support.V4.App.ActivityCompat.RequestPermissions(this.Activity, new String[] { Manifest.Permission.WriteExternalStorage, Manifest.Permission.ReadExternalStorage }, 53);
                    }
                    else
                    {
                        if (requestCode == 102)
                        {
                            Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                            Uri contentUri = Uri.FromFile(_file);
                            mediaScanIntent.SetData(contentUri);
                            this.Activity.SendBroadcast(mediaScanIntent);
                            int height = profile_image0.Height;
                            int width = Resources.DisplayMetrics.WidthPixels;
                            using (Android.Graphics.Bitmap bitmap = _file.Path.LoadAndResizeBitmap(width, height))
                            {
                                profile_image0.RecycleBitmap();
                                profile_image0.SetImageBitmap(bitmap);

                                try
                                {
                                    this.SetProfileImageToNavMenuHeader(this.Activity, bitmap);
                                }
                                catch { }

                                _profilepicbase64 = BitmapHelpers.BitmapToBase64(bitmap);
                                try
                                {
                                    objdb = new DBaseOperations();
                                    var lstu = objdb.selectTable();
                                    if (lstu != null && lstu.Count > default(int))
                                    {
                                        var uobj = lstu.FirstOrDefault();
                                        objdb.updateTable(new UserLoginInfo() { Id = uobj.Id, EmailId = uobj.EmailId, GoodName = uobj.GoodName, Password = uobj.Password, IsAdmin = uobj.IsAdmin, AuthToken = uobj.AuthToken, ProfilePicture = _profilepicbase64 });
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                }
                else
                {
                    if (requestCode == 102)
                    {
                        Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                        Uri contentUri = Uri.FromFile(_file);
                        mediaScanIntent.SetData(contentUri);
                        this.Activity.SendBroadcast(mediaScanIntent);
                        int height = profile_image0.Height;
                        int width = Resources.DisplayMetrics.WidthPixels;
                        using (Android.Graphics.Bitmap bitmap = _file.Path.LoadAndResizeBitmap(width, height))
                        {
                            profile_image0.RecycleBitmap();
                            profile_image0.SetImageBitmap(bitmap);

                            try
                            {
                                this.SetProfileImageToNavMenuHeader(this.Activity, bitmap);
                            }
                            catch { }

                            _profilepicbase64 = BitmapHelpers.BitmapToBase64(bitmap);
                            try
                            {
                                objdb = new DBaseOperations();
                                var lstu = objdb.selectTable();
                                if (lstu != null && lstu.Count > default(int))
                                {
                                    var uobj = lstu.FirstOrDefault();
                                    objdb.updateTable(new UserLoginInfo() { Id = uobj.Id, EmailId = uobj.EmailId, GoodName = uobj.GoodName, Password = uobj.Password, IsAdmin = uobj.IsAdmin, AuthToken = uobj.AuthToken, ProfilePicture = _profilepicbase64 });
                                }
                            }
                            catch { }
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

        private void GetAndSetExistingProfilePic(CircleImageView profileimg)
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
                        _profilepicbase64 = uobj.ProfilePicture;
                    }
                }
                catch { }

                if (!string.IsNullOrEmpty(_profilepicbase64))
                {
                    var btmpimg = BitmapHelpers.Base64ToBitmap(_profilepicbase64);
                    profileimg.RecycleBitmap();
                    profileimg.SetImageBitmap(btmpimg);
                }
            }
            catch (Exception ex)
            {

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

        private void SetProfileImageToNavMenuHeader(Activity currActivity, Android.Graphics.Bitmap bmp)
        {
            CircleImageView circular_imageViewLogo = currActivity.FindViewById<CircleImageView>(Resource.Id.circular_imageViewLogo);
            ImageView imageViewLogo = currActivity.FindViewById<ImageView>(Resource.Id.imageViewLogo);
            try
            {
                circular_imageViewLogo.Visibility = ViewStates.Visible;
                imageViewLogo.Visibility = ViewStates.Gone;
                circular_imageViewLogo.RecycleBitmap();
                circular_imageViewLogo.SetImageBitmap(bmp);
            }
            catch
            {
                circular_imageViewLogo.Visibility = ViewStates.Gone;
                imageViewLogo.Visibility = ViewStates.Visible;
            }
        }
    }
}