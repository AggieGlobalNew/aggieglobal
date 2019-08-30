namespace MyAggieNew
{
    public class Account : ModelBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int UserTypeId { get; set; }
        public string password { get; set; }
        public string Address { get; set; }
        public int UserId { get; set; }
        public bool IsExpired { get; set; }
        public string EmailId { get; set; }
        public int FarmId { get; set; }
        public int optMode { get; set; }
        public string UserDeviceId { get; set; }
        public string LoginTokenKey { get; set; }
        public int AuthenticationSuccessmode { get; set; }
        public int AuthenticatedUserID { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class AccountResponse : ModelBase
    {
        public string AuthToken { get; set; }
    }
}