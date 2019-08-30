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
using System.Linq;
using System.Net.Mail;
using System.Threading;
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

namespace MyAggieNew
{
    public class ContactFragment : Android.Support.V4.App.Fragment
    {
        EditText input_contact_subject, input_contact_notes;
        Button btn_contact_cancel, btn_sendmsg;

        public ContactFragment() { }

        public static Android.Support.V4.App.Fragment newInstance(Context context)
        {
            ContactFragment busrouteFragment = new ContactFragment();
            return busrouteFragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ViewGroup root = (ViewGroup)inflater.Inflate(Resource.Layout.fragment_content_contact, null);
            input_contact_subject = root.FindViewById<EditText>(Resource.Id.input_contact_subject);
            input_contact_notes = root.FindViewById<EditText>(Resource.Id.input_contact_notes);
            btn_contact_cancel = root.FindViewById<Button>(Resource.Id.btn_contact_cancel);
            btn_sendmsg = root.FindViewById<Button>(Resource.Id.btn_sendmsg);
            btn_contact_cancel.Click += (sndr, argus) => Cancel_Contact(sndr, argus, this.Activity);
            btn_sendmsg.Click += (sndr, argus) => Send_Message(sndr, argus, this.Activity);
            return root;
        }

        protected void Send_Message(object sender, EventArgs e, Activity currentActivity)
        {
            btn_sendmsg.Click -= (sndr, argus) => Cancel_Contact(sndr, argus, currentActivity);
            try
            {
                if (string.IsNullOrEmpty(input_contact_subject.Text.Trim()) || string.IsNullOrEmpty(input_contact_notes.Text.Trim()))
                {
                    throw new Exception("All fields are mandatory. Please complete all fields before sending");
                }
                else
                {
                    ProgressDialog progressDialog = ProgressDialog.Show(this.Activity, "Please wait...", "Sending your message...", true);
                    new Thread(new ThreadStart(delegate
                    {
                        currentActivity.RunOnUiThread(() => this.SendEmail(progressDialog, currentActivity, input_contact_subject.Text.Trim(), input_contact_notes.Text.Trim()));
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

                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }

        private void SendEmail(ProgressDialog dialog, Activity curActivity, string subject, string notes)
        {
            try
            {
                string mStringLoginInfo = string.Empty;
                string mStringSessionToken = string.Empty;
                try
                {
                    var objdb = new DBaseOperations();
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

                using (MailMessage mail = new MailMessage())
                {
                    using (SmtpClient SmtpServer = new SmtpClient(CommonEmailSetup.Host))
                    {
                        mail.From = new MailAddress(CommonEmailSetup.AdminEmailID);
                        mail.To.Add(CommonEmailSetup.SupportEmailID);
                        mail.Subject = string.Format("Mail From: {0}, Subject: {1}", mStringLoginInfo, subject);
                        mail.Body = string.Format("<b>Mail From:</b> {0}<br><b>Notes:</b><br>{1}", mStringLoginInfo, notes.Replace("\n", "<br>"));
                        mail.IsBodyHtml = true;

                        SmtpServer.Port = CommonEmailSetup.Port;
                        SmtpServer.EnableSsl = true;
                        SmtpServer.UseDefaultCredentials = false;
                        SmtpServer.Credentials = new System.Net.NetworkCredential(CommonEmailSetup.AdminEmailID, CommonEmailSetup.AdminEmalPassword);

                        //SmtpServer.Send(mail);
                        SmtpServer.SendMailAsync(mail);

                        curActivity.RunOnUiThread(() =>
                        {
                            Android.App.AlertDialog.Builder alertDiag = new Android.App.AlertDialog.Builder(curActivity);
                            alertDiag.SetTitle(Resource.String.DialogHeaderGeneric);
                            alertDiag.SetMessage("Thank you for contacting with us. We will get back to you soon");
                            alertDiag.SetIcon(Resource.Drawable.success);
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

        protected void Cancel_Contact(object sender, EventArgs e, Activity currentActivity)
        {
            try
            {
                btn_contact_cancel.Click -= (sndr, argus) => Cancel_Contact(sndr, argus, currentActivity);
                DashboardFragment obj = new DashboardFragment();
                Android.Support.V4.App.FragmentTransaction tx = FragmentManager.BeginTransaction();
                tx.Replace(Resource.Id.m_main, obj, Constants.dashboard);
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
                        //btn_contact_cancel.Click += (sndr, argus) => Cancel_Contact(sndr, argus, currentActivity);
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }
    }
}