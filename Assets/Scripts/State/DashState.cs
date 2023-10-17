using UnityEngine;

public class DashState : IState
{
    bool _isStateDone;
    IState _nextState;
    AvatarAspect _avatarBody;
    Vector2 _inputVector;

    public DashState(IState nextState, AvatarAspect avatar)
    {
        _nextState = nextState;
        _avatarBody = avatar;

    }
    public void OnEnterState()
    {
        _isStateDone = false;
    }

    public void OnUpdateState()
    {

    }

    public void OnExitState()
    {
        _isStateDone = true;
    }

    public void SetInputs(Vector2 inputVector)
    {
        _inputVector = inputVector;
    }

    public bool IsStateDone
    {
        get => _isStateDone;
    }

    public IState NextState
    {
        get => _nextState;
    }
}
