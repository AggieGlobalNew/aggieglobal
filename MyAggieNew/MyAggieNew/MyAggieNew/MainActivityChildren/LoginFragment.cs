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
using Android;
using Android.Content.PM;

namespace MyAggieNew
{
    class LoginFragment : Android.Support.V4.App.Fragment
    {
        Button btn_login;
        EditText input_username, input_password;
        TextView tv_Rgstr_Link;
        DBaseOperations objdb;

        public LoginFragment() { }

        public static Android.Support.V4.App.Fragment newInstance(Context context)
        {
            LoginFragment busrouteFragment = new LoginFragment();
            return busrouteFragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ViewGroup root = (ViewGroup)inflater.Inflate(Resource.Layout.fragment_main_login, null);
            btn_login = root.FindViewById<Button>(Resource.Id.btn_login);
            input_username = root.FindViewById<EditText>(Resource.Id.input_username);
            input_password = root.FindViewById<EditText>(Resource.Id.input_password);
            tv_Rgstr_Link = root.FindViewById<TextView>(Resource.Id.tv_Rgstr_Link);
            btn_login.Click += (sndr, argus) => Login_Clicked(sndr, argus, this.Activity);
            tv_Rgstr_Link.Click += (sndr, argus) => Register_Clicked(sndr, argus, this.Activity);

            try
            {
                if (Build.VERSION.SdkInt >= Build.VERSION_CODES.M)
                {
                    if (Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.Camera) != (int)Permission.Granted
                        || Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.WriteExternalStorage) != (int)Permission.Granted
                        || Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.AccessFineLocation) != (int)Permission.Granted
                        || Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.AccessCoarseLocation) != (int)Permission.Granted
                        || Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.AccessWifiState) != (int)Permission.Granted
                        || Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.AccessNetworkState) != (int)Permission.Granted)
                    {
                        Android.Support.V4.App.ActivityCompat.RequestPermissions(this.Activity, new string[]
                        {
                            Manifest.Permission.Camera,
                            Manifest.Permission.WriteExternalStorage,
                            Manifest.Permission.ReadExternalStorage,
                            Manifest.Permission.AccessFineLocation,
                            Manifest.Permission.AccessCoarseLocation,
                            Manifest.Permission.AccessWifiState,
                            Manifest.Permission.AccessNetworkState
                        }, 54);
                        /*FragmentManager.FindFragmentById(Resource.Layout.fragment_content_addproduct).RequestPermissions(new string[]
                        {
                                Manifest.Permission.Camera, Manifest.Permission.WriteExternalStorage,
                                Manifest.Permission.ReadExternalStorage
                        }, 54);*/
                    }
                }
            }
            catch { }

            try
            {
                objdb = new DBaseOperations();
                var lst = objdb.selectTable();
                if (lst != null && lst.Count > default(int))
                {
                    var uobj = new UserLoginInfo();
                    uobj = lst.FirstOrDefault();
                    if (uobj.Password != " ")
                    {
                        ProgressDialog progressDialog = ProgressDialog.Show(this.Activity, "Please wait...", "Checking account info...", true);
                        new Thread(new ThreadStart(delegate
                        {
                            this.Activity.RunOnUiThread(() => this.LoginCall(progressDialog, this.Activity, uobj.EmailId, uobj.Password));
                        })).Start();
                    }
                }
            }
            catch { }

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

        protected void Register_Clicked(object sender, EventArgs e, Activity currentActivity)
        {
            tv_Rgstr_Link.Click -= (sndr, argus) => Register_Clicked(sndr, argus, currentActivity);
            try
            {
                RegistrationFragment objFragment = new RegistrationFragment();
                Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                tx.Replace(Resource.Id.main, objFragment, Constants.registration);
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
                        //tv_Rgstr_Link.Click += (sndr, argus) => Register_Clicked(sndr, argus, currentActivity);
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }

        protected void Login_Clicked(object sender, EventArgs e, Activity currentActivity)
        {
            try
            {
                btn_login.Click -= (sndr, argus) => Login_Clicked(sndr, argus, currentActivity);

                if (string.IsNullOrWhiteSpace(input_username.Text.Trim()) || string.IsNullOrWhiteSpace(input_password.Text.Trim()))
                {
                    throw new Exception("Username and password both fields are mandatory. Please use proper credential to login.");
                }
                else
                {
                    ProgressDialog progressDialog = ProgressDialog.Show(this.Activity, "Please wait...", "Checking account info...", true);
                    new Thread(new ThreadStart(delegate
                    {
                        currentActivity.RunOnUiThread(() => this.LoginCall(progressDialog, currentActivity, input_username.Text.Trim(), input_password.Text.Trim()));
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
                        //btn_login.Click += (sndr, argus) => Login_Clicked(sndr, argus, currentActivity);
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }

        private void LoginCall(ProgressDialog dialog, Activity curActivity, string username, string password)
        {
            try
            {
                var client = new RestClient(Common.UrlBase);
                var request = new RestRequest("Account/SignIn", Method.GET);
                request.AddHeader("Content-Type", "application/json");
                request.AddQueryParameter("username", username);
                request.AddQueryParameter("password", password);
                request.AddQueryParameter("userDeviceId", Android.Provider.Settings.Secure.GetString(Android.App.Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId));
                IRestResponse response = client.Execute(request);
                var content = response.Content;
                var responseObj = Newtonsoft.Json.JsonConvert.DeserializeObject<AccountResponse>(content);
                if (responseObj.Status == ResponseStatus.Successful && !string.IsNullOrEmpty(responseObj.AuthToken))
                {
                    string strGoodName = string.Empty;
                    try
                    {
                        objdb = new DBaseOperations();
                        var lst = objdb.selectTable();

                        if (lst == null || lst.Count <= default(int))
                        {
                            var usr = this.FetchUserDetails(responseObj.AuthToken);
                            if (usr != null)
                            {
                                strGoodName = string.Format("{0} {1}", usr.FirstName, usr.LastName);
                            }
                            else
                            {
                                strGoodName = this.GetUserFullName(responseObj.AuthToken);
                            }
                            objdb.createDatabase();
                            objdb.insertIntoTable(new UserLoginInfo() { EmailId = username, GoodName = strGoodName, Password = password, IsAdmin = (usr != null && usr.IsAdmin ? 1 : default(int)), AuthToken = responseObj.AuthToken });
                        }
                        else if (lst != null && lst.Count > default(int))
                        {
                            /*objdb.removeTable(lst.FirstOrDefault());*/
                            var usr = this.FetchUserDetails(responseObj.AuthToken);
                            var uobj = lst.FirstOrDefault();
                            if (!string.IsNullOrEmpty(uobj.GoodName))
                            {
                                if (usr != null)
                                {
                                    strGoodName = string.Format("{0} {1}", usr.FirstName, usr.LastName);
                                }
                                else
                                {
                                    strGoodName = this.GetUserFullName(responseObj.AuthToken);
                                }
                            }
                            else
                            {
                                strGoodName = uobj.GoodName;
                            }
                            objdb.updateTable(new UserLoginInfo() { Id = uobj.Id, EmailId = username, GoodName = strGoodName, Password = password, IsAdmin = (usr != null && usr.IsAdmin ? 1 : default(int)), AuthToken = responseObj.AuthToken, ProfilePicture = uobj.ProfilePicture });
                        }
                    }
                    catch { }

                    Intent intent = new Intent(curActivity, typeof(MainContentActivity));
                    StartActivity(intent);
                    curActivity.Finish();
                }
                else
                {
                    if (string.IsNullOrEmpty(responseObj.Error))
                    {
                        throw new Exception(responseObj.Error);
                    }
                    else
                    {
                        throw new Exception("Invalid credential.");
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

        private string GetUserFullName(string token)
        {
            string strGoodName = string.Empty;
            try
            {
                var clientProfile = new RestClient(Common.UrlBase);
                var requestProfile = new RestRequest("UserAccount/GetUserDetails", Method.POST);
                requestProfile.AddHeader("Content-Type", "application/json");
                requestProfile.AddHeader("TokenKey", token);
                IRestResponse responseProfile = clientProfile.Execute(requestProfile);
                var contentProfile = responseProfile.Content;
                var responseObjProfile = Newtonsoft.Json.JsonConvert.DeserializeObject<Account>(contentProfile);
                if (responseObjProfile.Status == ResponseStatus.Successful && !string.IsNullOrEmpty(responseObjProfile.EmailId))
                {
                    strGoodName = string.Format("{0} {1}", responseObjProfile.FirstName, responseObjProfile.LastName);
                }
            }
            catch { }
            return strGoodName;
        }

        private Account FetchUserDetails(string token)
        {
            Account responseObjProfile = new Account();
            try
            {
                var clientProfile = new RestClient(Common.UrlBase);
                var requestProfile = new RestRequest("UserAccount/GetUserDetails", Method.POST);
                requestProfile.AddHeader("Content-Type", "application/json");
                requestProfile.AddHeader("TokenKey", token);
                IRestResponse responseProfile = clientProfile.Execute(requestProfile);
                var contentProfile = responseProfile.Content;
                responseObjProfile = Newtonsoft.Json.JsonConvert.DeserializeObject<Account>(contentProfile);
            }
            catch { }
            return responseObjProfile;
        }
    }
}


