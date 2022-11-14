using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class UserRelationManager : IUserRelationManager
{
    public IUserRelationStore UserRelationStore { get; }

    public UserRelationManager(IUserRelationStore userRelationStore)
    {
        UserRelationStore = userRelationStore;
    }

}