using UnityEngine;
public class BlastState : IState
{
    AvatarAspect _avatarAspect;
    BlastAspect _blastAspect;
    bool _isStateDone;
    IState _nextState;
    Vector2 _inputVector;

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

    }

    public void OnUpdateState()
    {
        if (_blastAspect.IsBlasting)
        {
            _avatarAspect.PerformMove(_inputVector);
        }
        else
        {
            _isStateDone = true;
        }
    }

    public void OnExitState()
    {
        _blastAspect.EndBlast();
        _isStateDone = false;
    }

    public void SetInputs(Vector2 inputVector)
    {
        _inputVector = inputVector;
        Debug.Log(_inputVector);
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
