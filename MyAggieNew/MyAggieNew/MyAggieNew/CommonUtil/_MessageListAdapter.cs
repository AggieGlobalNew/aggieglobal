using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace MyAggieNew
{
    public class _MessageListAdapter : RecyclerView.Adapter
    {
        private static readonly int VIEW_TYPE_MESSAGE_SENT = 1;
        private static readonly int VIEW_TYPE_MESSAGE_RECEIVED = 2;

        private Context mContext;
        private List<Message> mMessageList;

        public _MessageListAdapter(Context context, List<Message> messageList)
        {
            mContext = context;
            //mMessageList = messageList.OrderByDescending(o => o.createdAt).ToList();
            mMessageList = messageList;
        }

        public override int ItemCount => mMessageList.Count();

        public override int GetItemViewType(int position)
        {
            Message message = mMessageList[position];

            string strEmail = string.Empty;
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
                    strEmail = uobj.EmailId;
                }
            }
            catch { }

            if (message.sender != null && message.sender.EmailId == strEmail)
            {
                return VIEW_TYPE_MESSAGE_SENT;
            }
            else
            {
                return VIEW_TYPE_MESSAGE_RECEIVED;
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            Message message = mMessageList[position];
            switch (holder.ItemViewType)
            {
                case 1:
                    ((SentMessageHolder)holder).Bind(message);
                    break;
                case 2:
                    ((ReceivedMessageHolder)holder).Bind(message);
                    break;
                default:
                    ((SentMessageHolder)holder).Bind(message);
                    break;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view;
            LayoutInflater inflater = (LayoutInflater)mContext.GetSystemService(Context.LayoutInflaterService);

            if (viewType == VIEW_TYPE_MESSAGE_SENT)
            {
                view = new View(mContext);
                view = inflater.Inflate(Resource.Layout.item_message_sent, parent, false);
                return new SentMessageHolder(view);
            }
            else if (viewType == VIEW_TYPE_MESSAGE_RECEIVED)
            {
                view = new View(mContext);
                view = inflater.Inflate(Resource.Layout.item_message_received, parent, false);
                return new ReceivedMessageHolder(view);
            }
            else
            {
                view = new View(mContext);
                view = inflater.Inflate(Resource.Layout.item_message_sent, parent, false);
                return new SentMessageHolder(view);
            }

            return null;
        }
    }

    public class SentMessageHolder : RecyclerView.ViewHolder
    {
        TextView messageText, timeText;

        public SentMessageHolder(View itemView) : base(itemView)
        {
            base.ItemView = itemView;
            messageText = itemView.FindViewById<TextView>(Resource.Id.text_message_body);
            timeText = itemView.FindViewById<TextView>(Resource.Id.text_message_time);
        }

        public void Bind(Message message)
        {
            try
            {
                messageText.Text = message.message;
                timeText.Text = message.createdAt.ToString("dd MMM hh:mm tt");
            }
            catch { }
        }
    }

    public class ReceivedMessageHolder : RecyclerView.ViewHolder
    {
        TextView messageText, timeText, nameText;
        //ImageView profileImage;
        Refractored.Controls.CircleImageView profileImage;

        public ReceivedMessageHolder(View itemView) : base(itemView)
        {
            base.ItemView = itemView;
            messageText = itemView.FindViewById<TextView>(Resource.Id.text_message_body);
            timeText = itemView.FindViewById<TextView>(Resource.Id.text_message_time);
            nameText = itemView.FindViewById<TextView>(Resource.Id.text_message_name);
            profileImage = itemView.FindViewById<Refractored.Controls.CircleImageView>(Resource.Id.image_message_profile);
        }

        public void Bind(Message message)
        {
            try
            {
                messageText.Text = message.message;
                timeText.Text = message.createdAt.ToString("dd MMM hh:mm tt");
                nameText.Text = message.sender.GoodName;
                if (!string.IsNullOrEmpty(message.sender.ProfilePicture))
                {
                    profileImage.SetImageBitmap(BitmapHelpers.Base64ToBitmap(message.sender.ProfilePicture));
                }
                else
                {
                    profileImage.SetBackgroundResource(Resource.Drawable.chatbot);
                }
            }
            catch { }
        }
    }
}