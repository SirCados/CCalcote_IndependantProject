using UnityEngine;

public class ActiveState : IState
{
    bool _isStateDone;
    AvatarAspect _avatarBody;
    Vector2 _inputVector;

    public ActiveState(AvatarAspect avatar)
    {
        _avatarBody = avatar;
    }

    public void OnEnterState()
    {
        _isStateDone = false;
    }

    public void OnUpdateState()
    {
        _avatarBody.PerformMove(_inputVector);
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
        get => null;
    }
}
