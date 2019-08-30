using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace MyAggieNew
{
    public class _generic_grid_menu_bitmap_helper : BaseAdapter
    {
        private Context mContext;
        private string[] gridViewString;
        private string[] gridViewCodeString;
        private string[] gridViewTypeCodeString;
        private Bitmap[] gridViewImage;

        public _generic_grid_menu_bitmap_helper(Context context, string[] gridViewString, string[] gridViewCodeString, string[] gridViewTypeCodeString, Bitmap[] gridViewImage)
        {
            mContext = context;
            this.gridViewImage = gridViewImage;
            this.gridViewString = gridViewString;
            this.gridViewCodeString = gridViewCodeString;
            this.gridViewTypeCodeString = gridViewTypeCodeString;
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
                gridViewAndroid = inflater.Inflate(Resource.Layout._generic_grid_menu_extended, null);
                TextView textViewAndroid = gridViewAndroid.FindViewById<TextView>(Resource.Id.android_gridview_txt);
                TextView textViewCodeAndroid = gridViewAndroid.FindViewById<TextView>(Resource.Id.android_gridview_hdntxt0);
                TextView textViewTypeCodeAndroid = gridViewAndroid.FindViewById<TextView>(Resource.Id.android_gridview_hdntxt1);
                ImageView imageViewAndroid = gridViewAndroid.FindViewById<ImageView>(Resource.Id.android_gridview_img);
                textViewAndroid.Text = gridViewString[position];
                textViewCodeAndroid.Text = gridViewCodeString[position];
                textViewTypeCodeAndroid.Text = gridViewTypeCodeString[position];
                imageViewAndroid.SetImageBitmap(gridViewImage[position]);
            }
            else
            {
                gridViewAndroid = (View)convertView;
            }
            return gridViewAndroid;
        }
    }
}