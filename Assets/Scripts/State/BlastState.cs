using UnityEngine;
public class BlastState : IState
{
    AvatarAspect _avatarAspect;
    BlastAspect _blastAspect;
    bool _isStateDone;
    IState _nextState;
    Vector2 _aimInputVector;
    Vector2 _movementInputVector;

    public BlastState(IState nextState, AvatarAspect avatarAspect, BlastAspect blastAspect)
    {
        _nextState = nextState;
        _avatarAspect = avatarAspect;
        _blastAspect = blastAspect;
    }

    public void OnEnterState()
    {
        _isStateDone = false;
        _blastAspect.BeginBlast();
        _avatarAspect.IsBlasting = true;
    }

    public void OnUpdateState()
    {
        _avatarAspect.PerformMove(_movementInputVector);
        _blastAspect.IsGrounded = _avatarAspect.IsGrounded;
        _blastAspect.PerformRingMove(_aimInputVector);
    }

    public void OnExitState()
    {
        _blastAspect.EndBlast();
        _avatarAspect.IsBlasting = false;
        _isStateDone = false;
    }

    public void SetAimInputs(Vector2 inputVector)
    {
        _aimInputVector = inputVector;
    }

    public void SetMovementInputs(Vector2 inputVector)
    {
        _movementInputVector = inputVector;
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
