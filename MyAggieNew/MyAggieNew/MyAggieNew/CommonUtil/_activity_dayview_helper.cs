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
    public class _activity_dayview_helper : BaseAdapter
    {
        private Context mContext;
        private string[] gridViewCategoryString;
        private string[] gridViewCategoryCodeString;
        private Bitmap[] gridCategoryViewImage;
        private string[] gridViewProductString;
        private string[] gridViewProductCodeString;
        private Bitmap[] gridProductViewImage;
        private string[] gridProductActivityFor;
        private string[] gridProductActivityCode;

        public _activity_dayview_helper(Context context, string[] gridViewCategoryString, string[] gridViewCategoryCodeString, Bitmap[] gridCategoryViewImage, string[] gridViewProductString, string[] gridViewProductCodeString, Bitmap[] gridProductViewImage, string[] gridProductActivityFor, string[] gridProductActivityCode)
        {
            mContext = context;
            this.gridViewCategoryString = gridViewCategoryString;
            this.gridViewCategoryCodeString = gridViewCategoryCodeString;
            this.gridCategoryViewImage = gridCategoryViewImage;
            this.gridViewProductString = gridViewProductString;
            this.gridViewProductCodeString = gridViewProductCodeString;
            this.gridProductViewImage = gridProductViewImage;
            this.gridProductActivityFor = gridProductActivityFor;
            this.gridProductActivityCode = gridProductActivityCode;
        }

        public override int Count => gridViewProductCodeString.Count();

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
                gridViewAndroid = inflater.Inflate(Resource.Layout._activity_dayview, null);

                TextView txt_category_dtl = gridViewAndroid.FindViewById<TextView>(Resource.Id.txt_category_dtl);
                TextView txt_category_code_dtl = gridViewAndroid.FindViewById<TextView>(Resource.Id.txt_category_code_dtl);
                ImageView img_category_dtl = gridViewAndroid.FindViewById<ImageView>(Resource.Id.img_category_dtl);

                TextView txt_product_dtl = gridViewAndroid.FindViewById<TextView>(Resource.Id.txt_product_dtl);
                TextView txt_product_code_dtl = gridViewAndroid.FindViewById<TextView>(Resource.Id.txt_product_code_dtl);
                ImageView img_product_dtl = gridViewAndroid.FindViewById<ImageView>(Resource.Id.img_product_dtl);

                TextView txt_activity_for = gridViewAndroid.FindViewById<TextView>(Resource.Id.txt_activity_for);

                TextView txt_activity_code_dtl = gridViewAndroid.FindViewById<TextView>(Resource.Id.txt_activity_code_dtl);

                txt_category_dtl.Text = gridViewCategoryString[position];
                txt_category_code_dtl.Text = gridViewCategoryCodeString[position];
                img_category_dtl.SetImageBitmap(gridCategoryViewImage[position]);

                txt_product_dtl.Text = gridViewProductString[position];
                txt_product_code_dtl.Text = gridViewProductCodeString[position];
                img_product_dtl.SetImageBitmap(gridProductViewImage[position]);

                txt_activity_for.Text = gridProductActivityFor[position];

                txt_activity_code_dtl.Text = gridProductActivityCode[position];
            }
            else
            {
                gridViewAndroid = (View)convertView;
            }
            return gridViewAndroid;
        }
    }
}