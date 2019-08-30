using SQLite;

namespace MyAggieNew
{
    public class UserLoginInfo
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [NotNull, Unique]
        public string EmailId { get; set; }
        public string GoodName { get; set; }
        [NotNull]
        public string Password { get; set; }
        [NotNull]
        public int IsAdmin { get; set; }
        [NotNull]
        public string AuthToken { get; set; }
        public string ProfilePicture { get; set; }
    }
}