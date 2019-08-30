using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Refractored.Controls;
using RestSharp;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Provider;
using Java.IO;
//using Android.Graphics;
using Uri = Android.Net.Uri;

namespace MyAggieNew
{
    class RegistrationFragment : Android.Support.V4.App.Fragment
    {
        EditText input_fname, input_lname, input_useremail, input_userpassword, input_userconfpassword;
        CircleImageView iv_camera, profile_image;
        Button btn_rgster, btn_cancelrgstr;
        Account obj;
        public static File _file;
        public static File _dir;
        DBaseOperations objdb;
        public static string _profilepicbase64;

        public RegistrationFragment() { }

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
            ViewGroup root = (ViewGroup)inflater.Inflate(Resource.Layout.fragment_main_registration, null);

            StrictMode.VmPolicy.Builder builder = new StrictMode.VmPolicy.Builder();
            StrictMode.SetVmPolicy(builder.Build());

            input_fname = root.FindViewById<EditText>(Resource.Id.input_fname);
            input_lname = root.FindViewById<EditText>(Resource.Id.input_lname);
            input_userpassword = root.FindViewById<EditText>(Resource.Id.input_userpassword);
            input_userconfpassword = root.FindViewById<EditText>(Resource.Id.input_userconfpassword);
            input_useremail = root.FindViewById<EditText>(Resource.Id.input_useremail);

            btn_rgster = root.FindViewById<Button>(Resource.Id.btn_rgster);
            btn_rgster.Click += (sndr, argus) => Register_Clicked(sndr, argus, this.Activity);

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

            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();
                profile_image = root.FindViewById<CircleImageView>(Resource.Id.profile_image);
                iv_camera = root.FindViewById<CircleImageView>(Resource.Id.iv_camera);
                iv_camera.Click += (sndr, argus) => Camera_Clicked(sndr, argus, this.Activity);
            }
            return root;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
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

                            }
                            else
                            {
                                throw new Exception("You declined to allow the app to access your camera");
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

        public void Register_Clicked(object sender, EventArgs e, Activity currentActivity)
        {
            ProgressDialog progressDialog = null;
            try
            {
                btn_rgster.Click -= (sndr, argus) => Register_Clicked(sndr, argus, currentActivity);

                if (string.IsNullOrWhiteSpace(input_fname.Text.Trim()) || string.IsNullOrWhiteSpace(input_lname.Text.Trim())
                    || string.IsNullOrWhiteSpace(input_userpassword.Text.Trim()) || string.IsNullOrWhiteSpace(input_userconfpassword.Text.Trim())
                    || string.IsNullOrWhiteSpace(input_useremail.Text.Trim()))
                {
                    throw new Exception("All fields are mandatory. Please do not leave any field blank.");
                }

                if (input_userpassword.Text.Trim() != input_userconfpassword.Text.Trim())
                {
                    throw new Exception("Password does not matche. Please retype the password.");
                }

                obj = new Account();
                obj.FirstName = input_fname.Text.Trim();
                obj.LastName = input_lname.Text.Trim();
                obj.password = input_userpassword.Text.Trim();
                obj.UserDeviceId = Android.Provider.Settings.Secure.GetString(Android.App.Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
                obj.UserTypeId = 1;
                obj.EmailId = input_useremail.Text.Trim();
                obj.Address = " ";
                obj.FarmId = default(int);
                progressDialog = ProgressDialog.Show(this.Activity, "Please wait...", "Registering your account...", true);
                new Thread(new ThreadStart(delegate
                {
                    currentActivity.RunOnUiThread(() => this.Register(obj, progressDialog, currentActivity));
                })).Start();
            }
            catch (Exception ex)
            {
                currentActivity.RunOnUiThread(() =>
                {
                    if (progressDialog != null && progressDialog.IsShowing)
                    {
                        progressDialog.Hide();
                        progressDialog.Dismiss();
                    }

                    Android.App.AlertDialog.Builder alertDiag = new Android.App.AlertDialog.Builder(currentActivity);
                    alertDiag.SetTitle(Resource.String.DialogHeaderError);
                    alertDiag.SetMessage(ex.Message);
                    alertDiag.SetIcon(Resource.Drawable.alert);
                    alertDiag.SetPositiveButton(Resource.String.DialogButtonOk, (senderAlert, args) =>
                    {
                        //btn_rgster.Click += (sndr, argus) => Register_Clicked(sndr, argus, currentActivity);
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }

        private void Register(Account obj, ProgressDialog dialog, Activity curActivity)
        {
            try
            {
                var client = new RestClient(Common.UrlBase);
                var request = new RestRequest("Account/Register", Method.POST);
                //{ "FirstName" : "Anik", "LastName" : "Sen", "UserTypeId" : 1, "password" : "1234", "Address" : " ", "EmailId" : "asen@atlassoft.com", "FarmId" : default(int), "UserDeviceId": "71e28a9140f8712b" }
                request.AddJsonBody(new { FirstName = obj.FirstName, LastName = obj.LastName, UserTypeId = obj.UserTypeId, password = obj.password, Address = obj.Address, EmailId = obj.EmailId, FarmId = obj.FarmId, UserDeviceId = obj.UserDeviceId });
                request.AddHeader("Content-Type", "application/json");
                //request.AddHeader("token", "UnIwWlFXN0lFYWt0TG91d0tSQnU4WDZwSFhrNmU3VnZVR1A1V3lxSHVaND06YW5pay5zZW5Ab3V0bG9vay5jb206NjM2Njk1NDMyMDUwMzY5NDgy");
                IRestResponse response = client.Execute(request);
                var content = response.Content;
                var responseObj = Newtonsoft.Json.JsonConvert.DeserializeObject<AccountResponse>(content);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (responseObj.Status == ResponseStatus.Successful && !string.IsNullOrEmpty(responseObj.AuthToken))
                    {
                        objdb = new DBaseOperations();
                        var lst = objdb.selectTable();
                        if (lst != null && lst.Count > default(int))
                        {
                            objdb.removeTable(lst.FirstOrDefault());
                        }
                        /*if (!objdb.isDBExists())
                        {
                            objdb.createDatabase();
                        }*/
                        objdb.createDatabase();
                        objdb.insertIntoTable(new UserLoginInfo()
                        {
                            EmailId = input_useremail.Text.Trim(),
                            GoodName = string.Format("{0} {1}", obj.FirstName, obj.LastName),
                            Password = obj.password,
                            IsAdmin = obj.IsAdmin ? 1 : default(int),
                            AuthToken = responseObj.AuthToken,
                            ProfilePicture = _profilepicbase64
                        });

                        Intent intent = new Intent(curActivity, typeof(MainContentActivity));
                        StartActivity(intent);
                        curActivity.Finish();
                    }
                    else
                    {
                        throw new Exception(responseObj.Error);
                    }
                }
                else
                {
                    throw new Exception("Unable to register at this moment. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                //throw ex;

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

        //protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
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
                        if (requestCode == 102)
                        {
                            Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                            Uri contentUri = Uri.FromFile(_file);
                            mediaScanIntent.SetData(contentUri);
                            this.Activity.SendBroadcast(mediaScanIntent);
                            int height = profile_image.Height;
                            int width = Resources.DisplayMetrics.WidthPixels;
                            //int width = profile_image.Width;
                            using (Android.Graphics.Bitmap bitmap = _file.Path.LoadAndResizeBitmap(width, height))
                            {
                                profile_image.RecycleBitmap();
                                profile_image.SetImageBitmap(bitmap);

                                _profilepicbase64 = BitmapHelpers.BitmapToBase64(bitmap);
                            }
                        }
                    }
                }
                else
                {
                    //if (requestCode == 102 && resultCode == (int)Result.Ok)
                    if (requestCode == 102)
                    {
                        Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                        Uri contentUri = Uri.FromFile(_file);
                        mediaScanIntent.SetData(contentUri);
                        this.Activity.SendBroadcast(mediaScanIntent);
                        int height = profile_image.Height;
                        int width = Resources.DisplayMetrics.WidthPixels;
                        //int width = profile_image.Width;
                        using (Android.Graphics.Bitmap bitmap = _file.Path.LoadAndResizeBitmap(width, height))
                        {
                            profile_image.RecycleBitmap();
                            profile_image.SetImageBitmap(bitmap);

                            _profilepicbase64 = BitmapHelpers.BitmapToBase64(bitmap);
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

        private void CreateDirectoryForPictures()
        {
            //_dir = new File(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonPictures), "myaggie");
            //System.IO.Path.Combine((string)Android.OS.Environment.ExternalStorageDirectory, "myaggie");
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

        private List<FarmDetailResponse> GetAllFirms()
        {
            try
            {
                var client = new RestClient(Common.UrlBase);
                var request = new RestRequest("Farm/GetAllFarmsDetails", Method.GET);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("IsRegister", "true");
                IRestResponse response = client.Execute(request);
                var content = response.Content;
                var responseObj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FarmDetailResponse>>(content);
                return responseObj;
            }
            catch (Exception ex)
            {
                return new List<FarmDetailResponse>();
            }
        }
    }
}