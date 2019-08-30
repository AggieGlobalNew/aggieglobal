using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using System;
using System.Linq;
using System.Collections.Generic;
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Locations;
using Android;
using Android.Content.PM;
using Android.Support.V7.Widget;
using RestSharp;
using System.Threading;

namespace MyAggieNew
{
    public class ChatbotFragment : Android.Support.V4.App.Fragment
    {
        DBaseOperations objdb;
        EditText edittext_chatbox;
        Button button_chatbox_send;
        RecyclerView mMessageRecycler;
        _MessageListAdapter mMessageAdapter;
        List<Message> msg;

        public ChatbotFragment() { }

        public static Android.Support.V4.App.Fragment newInstance(Context context)
        {
            ChatbotFragment busrouteFragment = new ChatbotFragment();
            return busrouteFragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ViewGroup root = (ViewGroup)inflater.Inflate(Resource.Layout.fragment_content_chatbot, null);

            edittext_chatbox = root.FindViewById<EditText>(Resource.Id.edittext_chatbox);
            button_chatbox_send = root.FindViewById<Button>(Resource.Id.button_chatbox_send);
            button_chatbox_send.Click += (sndr, argus) => Send_Clicked(sndr, argus, this.Activity, root);

            if (msg == null)
            {
                msg = new List<Message>();
            }

            string name = string.Empty;
            UserLoginInfo uobj = new UserLoginInfo();
            try
            {
                objdb = new DBaseOperations();
                var lstu = objdb.selectTable();
                if (lstu != null && lstu.Count > default(int))
                {
                    uobj = lstu.FirstOrDefault();
                    if (uobj.Password == " ")
                    {
                        throw new Exception("Please login again");
                    }
                    name = uobj.GoodName;
                }
            }
            catch { }

            var objmsg = new Message();
            objmsg.sender = new UserLoginInfo();
            objmsg.sender.GoodName = Constants.ChatBotName;
            objmsg.sender.ProfilePicture = BitmapHelpers.BitmapToBase64(BitmapHelpers.GetBitmapFromResource(this.Activity.Resources, Resource.Drawable.chatbot));
            objmsg.message = string.Format("Hi, {0}! How may I help you?", name);
            objmsg.createdAt = DateTime.Now;
            msg.Add(objmsg);

            this.MessageSetter(msg, this.Activity, root, (Android.Support.V4.App.Fragment)FragmentManager.FindFragmentByTag(Constants.chatbot));

            return root;
        }

        private void MessageSetter(List<Message> msgLst, Activity context, ViewGroup root, Android.Support.V4.App.Fragment currentFragment)
        {
            try
            {
                new System.Threading.Thread(new System.Threading.ThreadStart(() =>
                {
                    if (context != null)
                    {
                        context.RunOnUiThread(() =>
                        {
                            mMessageRecycler = root.FindViewById<RecyclerView>(Resource.Id.reyclerview_message_list);
                            mMessageAdapter = new _MessageListAdapter(context, msgLst);
                            mMessageRecycler.HasFixedSize = true;
                            mMessageRecycler.SetAdapter(mMessageAdapter);
                            mMessageRecycler.SetLayoutManager(new LinearLayoutManager(context));
                        });
                    }
                })).Start();
            }
            catch { }
        }

        protected void Send_Clicked(object sender, EventArgs e, Activity currentActivity, ViewGroup root)
        {
            button_chatbox_send.Click -= (sndr, argus) => Send_Clicked(sndr, argus, currentActivity, root);
            try
            {
                if (!string.IsNullOrEmpty(edittext_chatbox.Text.Trim()))
                {
                    if (msg == null)
                    {
                        msg = new List<Message>();
                    }

                    UserLoginInfo uobj = new UserLoginInfo();
                    try
                    {
                        objdb = new DBaseOperations();
                        var lstu = objdb.selectTable();
                        if (lstu != null && lstu.Count > default(int))
                        {
                            uobj = lstu.FirstOrDefault();
                        }
                    }
                    catch { }

                    var objmsg = new Message();
                    objmsg.message = edittext_chatbox.Text.Trim();
                    objmsg.createdAt = DateTime.Now;
                    objmsg.sender = new UserLoginInfo();
                    objmsg.sender.EmailId = uobj.EmailId;
                    objmsg.sender.GoodName = uobj.GoodName;
                    if (!string.IsNullOrEmpty(uobj.ProfilePicture))
                    {
                        objmsg.sender.ProfilePicture = uobj.ProfilePicture;
                    }
                    else
                    {
                        objmsg.sender.ProfilePicture = BitmapHelpers.BitmapToBase64(BitmapHelpers.GetBitmapFromResource(this.Activity.Resources, Resource.Drawable.chatbot));
                    }
                    msg.Add(objmsg);
                    this.MessageSetter(msg, currentActivity, root, (Android.Support.V4.App.Fragment)FragmentManager.FindFragmentByTag(Constants.chatbot));
                    var requestChatMsg = edittext_chatbox.Text.Trim();
                    edittext_chatbox.Text = string.Empty;

                    ThreadPool.QueueUserWorkItem(state =>
                    {
                        currentActivity.RunOnUiThread(() => this.ResponseChat(root, currentActivity, requestChatMsg));
                    });
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
                        //button_chatbox_send.Click += (sndr, argus) => Send_Clicked(sndr, argus, currentActivity, root);
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();
                    diag.SetCanceledOnTouchOutside(false);
                });
            }
        }

        private async System.Threading.Tasks.Task ResponseChat(ViewGroup root, Activity currentActivity, string Question)
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    var answer = this.Communicator(Question).FirstOrDefault().answer.Trim();
                    if (!string.IsNullOrEmpty(answer))
                    {
                        var objmsgAns = new Message();
                        objmsgAns.sender = new UserLoginInfo();
                        objmsgAns.sender.GoodName = Constants.ChatBotName;
                        objmsgAns.sender.ProfilePicture = BitmapHelpers.BitmapToBase64(BitmapHelpers.GetBitmapFromResource(currentActivity.Resources, Resource.Drawable.chatbot));
                        objmsgAns.message = answer;
                        objmsgAns.createdAt = DateTime.Now;
                        msg.Add(objmsgAns);
                        this.MessageSetter(msg, currentActivity, root, (Android.Support.V4.App.Fragment)FragmentManager.FindFragmentByTag(Constants.chatbot));
                    }
                }
                catch { }
            });
        }

        private IList<Answer> Communicator(string strQu)
        {
            ChatBotAnswers responseObj = new ChatBotAnswers();
            try
            {
                var client = new RestClient(CommonChatBot.UrlBase);
                var request = new RestRequest(CommonChatBot.UrlQuestionMakerPart, Method.POST);
                request.AddJsonBody(new { question = strQu });
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "d69d1340-eb41-4b4c-a5fb-2bba5e009b4a");
                IRestResponse response = client.Execute(request);
                var content = response.Content;
                responseObj = Newtonsoft.Json.JsonConvert.DeserializeObject<ChatBotAnswers>(content);
            }
            catch (Exception ex)
            {

            }
            return responseObj.answers;
        }
    }
}