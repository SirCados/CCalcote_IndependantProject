
public class NeutralState : IState
{
    AvatarAspect _avatarBody;
    bool IState.IsStateDone => throw new System.NotImplementedException();

    public IState NextState { get => null; }

    public NeutralState( AvatarAspect avatar)
    {
        _avatarBody = avatar;
    }

    public void OnEnterState()
    {
        
    }

    public void OnUpdateState()
    {
        _avatarBody.PerformNeutral();
    }

    public void OnExitState()
    {
        
    }
}
