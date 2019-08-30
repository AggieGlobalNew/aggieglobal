using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AggieGlobal.WebApi.Infrastructure
{
    public class SortOrder
    {
        public static int GetProjectSortOrder(int orderBy, string strSortBy, string strSortOrder)
        {
            switch (strSortBy)
            {
                case "ProjectNumber":
                    orderBy = (strSortOrder == "0") ? 2 : 3;
                    break;
                case "ProjectName":
                    orderBy = (strSortOrder == "0") ? 0 : 1;
                    break;
                case "ProjectStartDate":
                    orderBy = (strSortOrder == "0") ? 6 : 7;
                    break;
                case "ProjectCity":
                    orderBy = (strSortOrder == "0") ? 8 : 9;
                    break;
                case "ProjectSize":
                    orderBy = (strSortOrder == "0") ? 10 : 11;
                    break;
                case "ProjectOwner":
                    orderBy = (strSortOrder == "0") ? 12 : 13;
                    break;
            }
            return orderBy;
        }

        public static int GetContactSortOrder(int orderBy, string strSortBy, string strSortOrder)
        {
            switch (strSortBy)
            {
                case "ContactFirstName":
                    orderBy = (strSortOrder == "0") ? 0 : 1;
                    break;
                case "ContactLastName":
                    orderBy = (strSortOrder == "0") ? 2 : 3;
                    break;
                case "ContactCompany":
                    orderBy = (strSortOrder == "0") ? 4 : 5;
                    break;
                case "ContactCity":
                    orderBy = (strSortOrder == "0") ? 6 : 7;
                    break;
            }
            return orderBy;
        }

        public static int GetUserSortOrder(int orderBy, string strSortBy, string strSortOrder)
        {
            switch (strSortBy)
            {
                case "UserFirstName":
                    orderBy = (strSortOrder == "0") ? 0 : 1;
                    break;
                case "UserLastName":
                    orderBy = (strSortOrder == "0") ? 2 : 3;
                    break;
                case "UserCompany":
                    orderBy = (strSortOrder == "0") ? 4 : 5;
                    break;
            }
            return orderBy;
        }

        public static int GetPunchSortOrder(int orderBy, string strSortBy, string strSortOrder)
        {
            switch (strSortBy)
            {
                case "1":
                    orderBy = (strSortOrder == "0") ? 0 : 1;
                    break;
                case "2":
                    orderBy = (strSortOrder == "0") ? 2 : 3;
                    break;
                case "3":
                    orderBy = (strSortOrder == "0") ? 4 : 5;
                    break;
                case "4":
                    orderBy = (strSortOrder == "0") ? 6 : 7;
                    break;
            }
            return orderBy;
        }

        public static int GetRFISortOrder(int orderBy, string strSortBy, string strSortOrder)
        {
            switch (strSortBy)
            {
                case "1":
                    orderBy = (strSortOrder == "0") ? 0 : 1;
                    break;
            }
            return orderBy;
        }

        public static int GetShareHistorySortOrder(int orderBy, string strSortBy, string strSortOrder)
        {
            switch (strSortBy)
            {
                case "CreateDate":
                    orderBy = (strSortOrder == "0") ? 0 : 1;
                    break;
                case "Email":
                    orderBy = (strSortOrder == "0") ? 2 : 3;
                    break;
                case "ProjectName":
                    orderBy = (strSortOrder == "0") ? 4 : 5;
                    break;
                default:
                    break;
            }
            return orderBy;
        }

        /*public static int GetShareProjectSortOrder(int orderBy, string strSortBy, string strSortOrder)
        {
            switch (strSortBy)
            {
                case "CreateDate":
                    orderBy = (strSortOrder == "0") ? 0 : 1;
                    break;
                case "ProjectName":
                    orderBy = (strSortOrder == "0") ? 2 : 3;
                    break;
                default:
                    break;
            }
            return orderBy;
        }*/

        public static int GetAccountBillingSortOrder(int orderBy, string strSortBy, string strSortOrder)
        {
            switch (strSortBy)
            {
                case "BillDate":
                    orderBy = (strSortOrder == "0") ? 0 : 1;
                    break;
                case "BillStartDate":
                    orderBy = (strSortOrder == "0") ? 2 : 3;
                    break;
                case "NetAmount":
                    orderBy = (strSortOrder == "0") ? 4 : 5;
                    break;
                default:
                    break;
            }
            return orderBy;
        }

        public static int GetSubmittalSortOrder(int orderBy, string strSortBy, string strSortOrder)
        {
            switch (strSortBy)
            {
                case "1":
                    orderBy = (strSortOrder == "0") ? 0 : 1;
                    break;
            }
            return orderBy;
        }

        public static int GetOutlookSortOrder(int orderBy, string strSortBy, string strSortOrder)
        {
            switch (strSortBy)
            {
                case "Date":
                    orderBy = (strSortOrder == "0") ? 0 : 1;
                    break;
                case "To":
                    orderBy = (strSortOrder == "0") ? 2 : 3;
                    break;
                case "Subject":
                    orderBy = (strSortOrder == "0") ? 4 : 5;
                    break;
                case "From":
                    orderBy = (strSortOrder == "0") ? 6 : 7;
                    break;                
            }
            return orderBy;
        }
    }
}
