using System.ComponentModel;
namespace AggieGlobal.WebApi.Common
{
    public enum ActionTaken
    {
        Added2ExtraData = 1,
        DuplicateColumn = 2,
        Ignored = 4,
        RaiseException = 8
    }

    public enum SSPCModule
    {
        [Description("None")]
        None = 0,
        [Description("Aggie")]
        Aggie = 1
       
    }
    public enum PCModule
    {
        [Description("None")]
        None = 0,
        [Description("Aggie")]
        Aggie = 1
     
       
    }

    public enum AuthReason
    {
        Success = 0,
        SubscriptionExpired = 1,
        NotApproved = 2,
        NotSubscribed = 3,
        ProjectDeleted = 4,
        NotInProjectTeam = 5,
        WrongProject = 6,
        ProjectShareExpired = 7,
        AccountClosed = 8,
        AccountInactive = 9,
        AccountOnhold = 10,
        TwoStepNotVerified = 11
    }
}
