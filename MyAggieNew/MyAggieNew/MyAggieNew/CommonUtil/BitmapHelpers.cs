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
using Android.Graphics;
using Android.Graphics.Drawables;
using System.IO;
using Android.Media;
using Android.Util;
using RestSharp;
using Android.Provider;

namespace MyAggieNew
{
    public static class BitmapHelpers
    {
        public static void RecycleBitmap(this ImageView imageView)
        {
            if (imageView == null)
            {
                return;
            }
            Drawable toRecycle = imageView.Drawable;
            if (toRecycle != null)
            {
                ((BitmapDrawable)toRecycle).Bitmap.Recycle();
            }
        }

        public static Bitmap LoadAndResizeBitmap(this string fileName, int width, int height)
        {
            BitmapFactory.Options options = new BitmapFactory.Options
            {
                InPurgeable = true,
                InJustDecodeBounds = true
            };

            Bitmap resizedBitmap;
            using (var fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open))
            {
                using (Bitmap bitmap = BitmapFactory.DecodeStream(fs, null, options))
                {
                    int outHeight = options.OutHeight;
                    int outWidth = options.OutWidth;
                    int inSampleSize = 1;
                    if (outHeight > height || outWidth > width)
                    {
                        inSampleSize = outWidth > outHeight ? outHeight / height : outWidth / width;
                    }

                    options.InSampleSize = inSampleSize;
                    options.InJustDecodeBounds = false;

                    resizedBitmap = BitmapFactory.DecodeFile(fileName, options);
                }
            }

            return resizedBitmap;
        }

        public static string BitmapToBase64(Bitmap bitmap)
        {
            byte[] byteArray;
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Compress(Bitmap.CompressFormat.Png, 100, ms);
                byteArray = ms.ToArray();
            }
            return Base64.EncodeToString(byteArray, Base64Flags.Default);
        }

        public static byte[] BitmapToByteArray(Bitmap bitmap)
        {
            byte[] byteArray;
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Compress(Bitmap.CompressFormat.Png, 100, ms);
                byteArray = ms.ToArray();
            }
            return byteArray;
        }

        public static Bitmap Base64ToBitmap(String base64String)
        {
            byte[] imageAsBytes = Base64.Decode(base64String, Base64Flags.Default);
            return BitmapFactory.DecodeByteArray(imageAsBytes, 0, imageAsBytes.Length);
        }

        public static Bitmap GetImageFromUrl(string url, string token)
        {
            try
            {
                var client = new RestClient(Common.UrlBase);
                var request = new RestRequest(url, Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("TokenKey", token);
                IRestResponse response = client.Execute(request);
                var content = response.Content;
                var responseObj = Newtonsoft.Json.JsonConvert.DeserializeObject<CommonModuleResponse>(content);
                return BitmapFactory.DecodeByteArray(responseObj.fileStream, 0, responseObj.fileStream.Length);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static IList<Bitmap> GetImageListFromUrlList(IList<string> urls, string token, Android.Content.Res.Resources res)
        {
            IList<Bitmap> bmpimglst;
            try
            {
                bmpimglst = new List<Bitmap>();
                foreach (var url in urls)
                {
                    try
                    {
                        var client = new RestClient(Common.UrlBase);
                        //var request = new RestRequest(url, Method.POST);
                        var request = new RestRequest(string.Format(Common.DownloadUrlPart, url), Method.POST);
                        request.AddHeader("Content-Type", "application/json");
                        request.AddHeader("TokenKey", token);
                        IRestResponse response = client.Execute(request);
                        var content = response.Content;
                        var responseObj = Newtonsoft.Json.JsonConvert.DeserializeObject<CommonModuleResponse>(content);
                        bmpimglst.Add(BitmapFactory.DecodeByteArray(responseObj.fileStream, 0, responseObj.fileStream.Length));
                    }
                    catch (Exception)
                    {
                        bmpimglst.Add(BitmapFactory.DecodeResource(res, Resource.Drawable.na));
                        continue;
                    }
                }
                return bmpimglst;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Bitmap GetBitmapFromResource(Android.Content.Res.Resources res, int resourceId)
        {
            try
            {
                return BitmapFactory.DecodeResource(res, resourceId);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetRealPathFromUri(Context context, Android.Net.Uri contentUri)
        {
            Android.Database.ICursor cursor = null;
            try
            {
                string[] proj = { MediaStore.Images.Media.InterfaceConsts.Data };
                cursor = context.ContentResolver.Query(contentUri, proj, null, null, null);
                int column_index = cursor.GetColumnIndexOrThrow(MediaStore.Images.Media.InterfaceConsts.Data);
                cursor.MoveToFirst();
                return cursor.GetString(column_index);
            }
            finally
            {
                if (cursor != null && !cursor.IsClosed)
                {
                    cursor.Close();
                }
            }
        }
    }
}