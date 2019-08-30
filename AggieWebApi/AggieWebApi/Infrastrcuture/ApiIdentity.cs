using AggieGlobal.Models.Client;
using System;
using System.Security.Principal;

namespace AggieGlobal.WebApi.Infrastructure
{
    public class ApiIdentity : IIdentity
    {
        public Account User { get; private set; }

        public ApiIdentity(Account user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            this.User = user;
        }

        public int UserId
        {
            get { return this.User.UserId; }
        }

        public string AuthenticationType
        {
            get { return "Basic"; }
        }

        public bool IsAuthenticated
        {
            get { return true; }
        }

        public string Name
        {
            get { return this.User.FirstName; }
        }
    }

}