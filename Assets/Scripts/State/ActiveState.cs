using UnityEngine;

public class ActiveState : IState
{
    public bool IsJumping = false;

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
        HandleMovement();
    }

    public void OnExitState()
    {
        _isStateDone = true;
    }

    void HandleMovement()
    {
        if (IsJumping)
        {
            IsJumping = false;
            _avatarBody.PerformJump(_inputVector);
        }
        _avatarBody.PerformMove(_inputVector);
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
