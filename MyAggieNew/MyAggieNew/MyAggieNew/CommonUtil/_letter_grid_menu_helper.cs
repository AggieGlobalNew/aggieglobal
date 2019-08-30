using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace MyAggieNew
{
    public class _letter_grid_menu_helper : BaseAdapter
    {
        private Context mContext;
        private string[] gridViewString;
        private string[] gridViewCodeString;
        private string[] gridViewLetterImage;

        public _letter_grid_menu_helper(Context context, string[] gridViewString, string[] gridViewCodeString, string[] gridViewLetterImage)
        {
            mContext = context;
            this.gridViewLetterImage = gridViewLetterImage;
            this.gridViewString = gridViewString;
            this.gridViewCodeString = gridViewCodeString;
        }

        public override int Count => gridViewString.Count();

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return default(int);
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View gridViewAndroid;
            LayoutInflater inflater = (LayoutInflater)mContext.GetSystemService(Context.LayoutInflaterService);

            if (convertView == null)
            {
                gridViewAndroid = new View(mContext);
                gridViewAndroid = inflater.Inflate(Resource.Layout._letter_grid_menu, null);
                TextView textViewAndroid = gridViewAndroid.FindViewById<TextView>(Resource.Id.android_gridview_text_l);
                TextView textViewCodeAndroid = gridViewAndroid.FindViewById<TextView>(Resource.Id.android_gridview_text_hidden_l);
                TextView imageViewAndroid = gridViewAndroid.FindViewById<TextView>(Resource.Id.android_gridview_hdr_text_l);
                textViewAndroid.Text = gridViewString[position];
                textViewCodeAndroid.Text = gridViewCodeString[position];
                imageViewAndroid.Text = gridViewLetterImage[position];
            }
            else
            {
                gridViewAndroid = (View)convertView;
            }
            return gridViewAndroid;
        }
    }
}