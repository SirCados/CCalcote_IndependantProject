using UnityEngine;

public class DashState : IState
{
    bool _isStateDone;
    IState _nextState;
    AvatarAspect _avatarAspect;
    Vector2 _inputVector;

    public DashState(IState nextState, AvatarAspect avatar)
    {
        _nextState = nextState;
        _avatarAspect = avatar;

    }
    public void OnEnterState()
    {
        _isStateDone = false;
        HandleAirDash();
    }

    public void OnUpdateState()
    {
        _avatarAspect.CheckIfDashIsDone();

        if (!_avatarAspect.IsDashing)
        {
            _isStateDone = true;
        }
    }

    public void OnExitState()
    {
        _isStateDone = false;
    }

    void HandleAirDash()
    {
        _avatarAspect.PerformAirDash(_inputVector);
    }

    public void SetInputs(Vector2 inputVector)
    {
        _inputVector = inputVector;
        Debug.Log("_input vector: " + _inputVector);

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
