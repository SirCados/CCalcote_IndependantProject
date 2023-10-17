using UnityEngine;

public class MoveState : IState
{
    bool _isStateDone;
    IState _nextState;
    AvatarAspect _avatarBody;
    Vector2 _inputVector;

    public MoveState(IState nextState, AvatarAspect avatar)
    {
        _nextState = nextState;
        _avatarBody = avatar;
    }

    public void OnEnterState()
    {

    }

    public void OnUpdateState()
    {
        _avatarBody.PerformMove(_inputVector);
    }

    public void OnExitState()
    {

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
        set => _nextState = value;
    }
}
