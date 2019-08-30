/* ========================================================================
 * Includes    : FormatterConfig.cs
 * Website     : http://www.atlassoft.com
 * Create Date : May, May 08, 2019 by Sudipta Sarkar
 * Details     : Implements various data type formatter registration logics for AggieGlobal.WebApi.
 * Copyright © 2019 Atlas Software Technologies All rights reserved
 * ========================================================================
 * Release History
 * ---------------
 * DESCRIPTION					CREATED / MODIFIED BY
 * -----------                  ---------------------
 * Initial Implementation		Sudipta Sarkar
 * ========================================================================
 */

using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace AggieGlobal.WebApi.Infrastructure
{
    public class WebApiException : Exception
    {
        public ErrorList Error { get; private set; }
        public HttpStatusCode Status { get; private set; }
        public string ReturnData { get; private set; }

        public WebApiException(ErrorList error, HttpStatusCode code = HttpStatusCode.NotFound)
            : base(new ErrorObject(error).ToJson())
        {
            Error = error;
            Status = code;
        }

        public WebApiException(ErrorList error, string returnData, HttpStatusCode code = HttpStatusCode.NotFound)
            : base(new ErrorObject(error,returnData).ToJson())
        {
            Error = error;
            Status = code;
            ReturnData = returnData;
        
        }
    }

    public class BusinessException : Exception
    {
        public ErrorList Error { get; private set; }
        public HttpStatusCode Status { get; private set; }

        public BusinessException(ErrorList error, HttpStatusCode code = HttpStatusCode.NotFound)
            : base(new ErrorObject(error).ToJson())
        {
            Error = error;
            Status = code;
        }
    }

    public class CannotCreateDataException : Exception
    {
        public CannotCreateDataException(string message)
            : base(message)
        {
        }
    }

    public class ErrorObject
    {
        public ErrorObject(ErrorList error)
        {
            ErrorCode = error.GetHashCode();
            //ErrorMessage = LanguageStrings.ResourceManager.GetString(error.ToString());
           
        }
        public ErrorObject(ErrorList error, string returnData)
        {
            ErrorCode = error.GetHashCode();
            ErrorMessage = returnData;
        }

        public int ErrorCode { get; private set; }
        public string ErrorMessage { get; private set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public HttpContent ToContent()
        {
            return new ObjectContent<ErrorObject>(this, FormatterConfig.JsonFormatter);
        }
    }

    public enum ErrorList
    {
        UnknownException = 0,
        InvalidToken = 1,
        EmptyToken = 2,
        InvalidCredential = 3,
        SubscriptionExpired = 4,
        LoginIdUnavailable = 5,
        UnableToProcess=6,
        EmptyArgument=7,
        PermissionDenied=8,
        AccountNotApproved=9
    }
}
