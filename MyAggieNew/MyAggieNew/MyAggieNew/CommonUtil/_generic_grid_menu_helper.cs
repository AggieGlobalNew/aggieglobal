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
    public class _generic_grid_menu_helper : BaseAdapter
    {
        private Context mContext;
        private string[] gridViewString;
        private string[] gridViewCodeString;
        private int[] gridViewImageId;

        public _generic_grid_menu_helper(Context context, string[] gridViewString, string[] gridViewCodeString, int[] gridViewImageId)
        {
            mContext = context;
            this.gridViewImageId = gridViewImageId;
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
                gridViewAndroid = inflater.Inflate(Resource.Layout._generic_grid_menu, null);
                TextView textViewAndroid = gridViewAndroid.FindViewById<TextView>(Resource.Id.android_gridview_text);
                TextView textViewCodeAndroid = gridViewAndroid.FindViewById<TextView>(Resource.Id.android_gridview_text_hidden);
                ImageView imageViewAndroid = gridViewAndroid.FindViewById<ImageView>(Resource.Id.android_gridview_image);
                textViewAndroid.Text = gridViewString[position];
                textViewCodeAndroid.Text = gridViewCodeString[position];
                imageViewAndroid.SetImageResource(gridViewImageId[position]);
                //gridViewAndroid.SetBackgroundResource(Resource.Drawable.grid_items_border);
            }
            else
            {
                gridViewAndroid = (View)convertView;
            }
            return gridViewAndroid;
        }
    }
}