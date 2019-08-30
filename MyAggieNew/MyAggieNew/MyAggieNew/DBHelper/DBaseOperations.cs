using Android.Util;
using SQLite;
using System.Collections.Generic;
using System.IO;

namespace MyAggieNew
{
    public class DBaseOperations
    {
        string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        
        public bool isDBExists()
        {
            try
            {
                bool x = System.IO.File.Exists(System.IO.Path.Combine(folder, "UserLoginInfo.db"));
                return x;
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public bool createDatabase()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "UserLoginInfo.db")))
                {
                    connection.CreateTable<UserLoginInfo>();
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public bool insertIntoTable(UserLoginInfo uinfo)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "UserLoginInfo.db")))
                {
                    connection.Insert(uinfo);
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public List<UserLoginInfo> selectTable()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "UserLoginInfo.db")))
                {
                    return connection.Table<UserLoginInfo>().ToList();
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }

        public bool selectTableFlat()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "UserLoginInfo.db")))
                {
                    connection.Query<UserLoginInfo>("SELECT TOP 1 * FROM UserLoginInfo");
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public bool updateTable(UserLoginInfo uinfo)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "UserLoginInfo.db")))
                {
                    connection.Query<UserLoginInfo>("UPDATE UserLoginInfo set EmailId=?, GoodName=?, Password=?, IsAdmin=?, AuthToken=?, ProfilePicture=? Where Id=?", uinfo.EmailId, uinfo.GoodName, uinfo.Password, uinfo.IsAdmin, uinfo.AuthToken, uinfo.ProfilePicture, uinfo.Id);
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public bool removeTable(UserLoginInfo uinfo)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "UserLoginInfo.db")))
                {
                    connection.Delete(uinfo);
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }  

        public bool selectTable(int Id)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "UserLoginInfo.db")))
                {
                    connection.Query<UserLoginInfo>("SELECT * FROM UserLoginInfo Where Id=?", Id);
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }
    }
}