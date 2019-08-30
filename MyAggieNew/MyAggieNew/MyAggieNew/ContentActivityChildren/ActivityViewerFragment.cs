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
using Com.Github.Sundeepk.Compactcalendarview;
using Java.Util;
using Android.Graphics;
using static Android.App.Usage.UsageEvents;
using RestSharp;
using System.Linq;

namespace MyAggieNew
{
    public class ActivityViewerFragment : Android.Support.V4.App.Fragment
    {
        DBaseOperations objdb;
        TextView txt_calendar_title;
        CompactCalendarView compactCalendarView;
        public Java.Text.SimpleDateFormat dateFormatForMonth = new Java.Text.SimpleDateFormat("MMM - yyyy");
        public static SupportFragment mCurrentFragment = new SupportFragment();

        public ActivityViewerFragment() { }

        public static Android.Support.V4.App.Fragment newInstance(Context context)
        {
            ActivityViewerFragment busrouteFragment = new ActivityViewerFragment();
            return busrouteFragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ViewGroup root = (ViewGroup)inflater.Inflate(Resource.Layout.fragment_content_activityviewer, null);
            try
            {
                if (mCurrentFragment.FragmentManager == null)
                {
                    mCurrentFragment = this;
                }

                txt_calendar_title = root.FindViewById<TextView>(Resource.Id.txt_calendar_title);
                compactCalendarView = root.FindViewById<CompactCalendarView>(Resource.Id.compactcalendar_view);
                compactCalendarView.SetFirstDayOfWeek(Calendar.Monday);
                compactCalendarView.ShouldDrawIndicatorsBelowSelectedDays(true);
                txt_calendar_title.Text = DateTime.Now.ToString("MMMM - yyyy");

                DateTime currentDate = DateTime.Now;
                var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                //var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

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

                try
                {
                    compactCalendarView.RemoveAllEvents();
                    compactCalendarView.AddEvents(this.LoadActivities(firstDayOfMonth.ToString("yyyy-MM-dd"), mStringSessionToken));
                }
                catch { }
                compactCalendarView.SetListener(new CompactCalendarViewListener(this.Activity, txt_calendar_title, compactCalendarView));
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

        public IList<Com.Github.Sundeepk.Compactcalendarview.Domain.Event> LoadActivities(string date, string token)
        {
            List<ActivityDetailCountByDateResponse> lst;
            try
            {
                lst = new List<ActivityDetailCountByDateResponse>();
                var client = new RestClient(Common.UrlBase);
                var request = new RestRequest("Activity/GetActivityCountByDate", Method.GET);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("TokenKey", token);
                request.AddQueryParameter("dateStamp", date);
                IRestResponse response = client.Execute(request);
                var content = response.Content;
                var responseObj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ActivityDetailCountByDateResponse>>(content);
                if (responseObj != null && responseObj.Count() > default(int) && responseObj[0].ActivityDate != DateTime.MinValue)
                {
                    var dt = DateTime.ParseExact(date, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                    return this.PrepareEvents(responseObj);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        protected IList<Com.Github.Sundeepk.Compactcalendarview.Domain.Event> PrepareEvents(IList<ActivityDetailCountByDateResponse> activities)
        {
            IList<Com.Github.Sundeepk.Compactcalendarview.Domain.Event> lst;
            try
            {
                if (activities != null && activities.Count() > default(int))
                {
                    lst = new List<Com.Github.Sundeepk.Compactcalendarview.Domain.Event>();
                    foreach (var activity in activities)
                    {
                        if (activity != null)
                        {
                            for (int i = 0; i < activity.ActivityCount; i++)
                            {
                                var evnt = new Com.Github.Sundeepk.Compactcalendarview.Domain.Event(Color.ParseColor("#CA5100"), DateTimeOffset.Parse(activity.ActivityDate.ToString()).ToUnixTimeMilliseconds());
                                lst.Add(evnt);
                            }
                        }
                    }
                    return lst;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void RedirectToDayView(DateTime currentDate)
        {
            try
            {
                Bundle utilBundle = new Bundle();
                utilBundle.PutString("siteparamdate", currentDate.ToString("dd-MM-yyyy"));
                ActivityByDateFragment objFragment = new ActivityByDateFragment();
                objFragment.Arguments = utilBundle;
                Android.Support.V4.App.FragmentTransaction tx = mCurrentFragment.FragmentManager.BeginTransaction();
                tx.Replace(Resource.Id.m_main, objFragment, Constants.addactivitybydate);
                tx.Commit();
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
    }

    public class CompactCalendarViewListener : Java.Lang.Object, CompactCalendarView.ICompactCalendarViewListener
    {
        TextView tv;
        Activity currentActivity;
        CompactCalendarView cview;
        ActivityViewerFragment superObj = new ActivityViewerFragment();

        public CompactCalendarViewListener(Activity _currentActivity, TextView _tv, CompactCalendarView _cview)
        {
            tv = _tv;
            currentActivity = _currentActivity;
            cview = _cview;
        }

        public void OnDayClick(Date p0)
        {
            try
            {
                Java.Text.SimpleDateFormat dateFormat = new Java.Text.SimpleDateFormat("YYYY-MM-dd hh:mm:ss");
                DateTime sa = Convert.ToDateTime(dateFormat.Format(p0));

                IList<Com.Github.Sundeepk.Compactcalendarview.Domain.Event> day_events = cview.GetEvents(p0);
                if (day_events != null && day_events.Count > default(int))
                {
                    //Redirect to ActivityDayView
                    superObj.RedirectToDayView(sa);
                }
                else
                {
                    throw new Exception(string.Format("No event found on {0}", sa.ToString("dd-MM-yyyy")));
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

        public void OnMonthScroll(Date p0)
        {
            try
            {
                Java.Text.SimpleDateFormat dateFormat = new Java.Text.SimpleDateFormat("YYYY-MM-dd hh:mm:ss");
                DateTime sa = Convert.ToDateTime(dateFormat.Format(p0));
                tv.Text = sa.ToString("MMMM - yyyy");

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

                var firstDayOfMonth = new DateTime(sa.Year, sa.Month, 1);
                try
                {
                    cview.RemoveAllEvents();
                    //cview.AddEvents(new ActivityViewerFragment().LoadActivities(firstDayOfMonth.ToString("yyyy-MM-dd"), mStringSessionToken));
                    cview.AddEvents(superObj.LoadActivities(firstDayOfMonth.ToString("yyyy-MM-dd"), mStringSessionToken));
                }
                catch { }
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
    }
}