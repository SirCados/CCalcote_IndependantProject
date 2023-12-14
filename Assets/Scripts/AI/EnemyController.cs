using System.Collections;
using UnityEngine;
using UnityEngine.AI;

class Waypoint
{   
    public Vector3 Location;
    public float MyProximity;
    public float TargetProximity;
    public float Weight;

    public Waypoint(Vector3 location)
    {
        Location = location;
    }
}

public class EnemyController : MonoBehaviour, IController
{

    public string CurrentState;

    [SerializeField] LayerMask DetectionMask = ~0;
    public VisionSensor Eyes;
    public NavMeshAgent Agent;

    [Tooltip("Lower values find better hiding spots")]
    [Range(-1, 1)] public float HideSensitivity = 0;
    [SerializeField][Range(0,1)] float _LookTime = .2f;

    [Range(1, 10)]
    public float MinPlayerDistance = 5f;
    [Range(0, 5f)]
    public float MinObstacleHeight = 1.25f;

    [SerializeField] int _NextCheckDistance = 2;
    [SerializeField] Collider[] _colliders = new Collider[8];

    public Transform CurrentTarget;

    public AvatarAspect ManifestedAvatar;
    public BarrageAspect ManifestedBarrage;
    public BlastAspect ManifestedBlast;

    IState _currentState = new EmptyState();
    ActiveState _activeState;
    BarrageState _barrageState;
    BlastState _blastState;
    DashState _dashState;
    DownState _downState;
    GetUpState _getUpState;

    AvatarAspect _enemyAvatar;

    private Coroutine MovementCoroutine;

    public Vector2 InputVector;

    [SerializeField] GameObject[] _WaypointObjects = new GameObject[17];

    [SerializeField] Waypoint[] _Waypoints = new Waypoint[17];

    Vector3 _moveToPosition;

    public float Weight = 0;
    public float TargetProximity = 0;
    public float MyProximity = 0;

    public GameObject[] Avatars;

    bool _isAiming;
    public enum AvatarType
    {
        BALANCED,
        HEAVY,
        FLOATY,
        SWIFT
    }

    public AvatarType AvatarToManifest;

    public PlayerController Player;

    private void Awake()
    {
    }

    private void Start()
    {
        PopulateWaypoints();
        StartCoroutine(GetTargetAvatar());
    }

    private void FixedUpdate()
    {
        if (!ManifestedAvatar.IsGameOver)
        {
            //change to switch?
            if (_currentState != _downState && ManifestedAvatar.IsKnockedDown)
            {
                print("knocked down");
                ChangeState(_downState);
            }
            else if (!ManifestedAvatar.IsInHitStun)
            {
                StateControllerUpdate();
                if (MyProximity > 2f)
                {
                    GetInputsForMovement();
                }
            }
        }
    }

    void GetInputsForMovement()
    {
        Vector2 inputs = (_currentState == _activeState && !ManifestedBarrage.IsRecovering) ? InputVector : Vector2.zero;
        if (_currentState == _activeState)
            _activeState.SetInputs(inputs);
    }

    public void StateControllerUpdate()
    {
        if (_currentState != null)
        {
            _currentState.OnUpdateState();
            CurrentState = _currentState.ToString();
        }

        if (_currentState.NextState != null && _currentState.IsStateDone)
        {
            ChangeState(_currentState.NextState);
        }
    }

    public void ChangeState(IState newState)
    {
        if (_currentState != null)
        {
            _currentState.OnExitState();
        }

        _currentState = newState;
        _currentState.OnEnterState();
    }


    private void HandleGainSight(Transform target)
    {
        if (MovementCoroutine != null)
        {
            StopCoroutine(MovementCoroutine);
        }
        //Get Current Target by Raycast in VisionSensor
        CurrentTarget = target;
        if (_currentState == _activeState)
            MovementCoroutine = StartCoroutine(Hide(CurrentTarget));
    }

    private void HandleLoseSight(Transform target)
    {
        if (MovementCoroutine != null)
        {
            StopCoroutine(MovementCoroutine);
        }
        //Get Current Target by Raycast in VisionSensor
        CurrentTarget = target;
        if(_currentState == _activeState)
            MovementCoroutine = StartCoroutine(Hide(CurrentTarget));
    }

    IEnumerator Hide(Transform target)
    {
        WaitForSeconds Wait = new WaitForSeconds(_LookTime);

        while (true)
        {
            CalculateDistances();
            System.Array.Sort(_colliders, ColliderArraySortComparer);
            int cornersTouched = 1;

            for (int i = 0; i < _colliders.Length; i++)
            {
                if (NavMesh.SamplePosition(_colliders[i].transform.position, out NavMeshHit hit, 2f, Agent.areaMask))
                {
                    if (NavMesh.SamplePosition(_colliders[i].transform.position - (target.position - hit.position).normalized, out NavMeshHit hit2, 2f, Agent.areaMask))
                    {
                        if (!NavMesh.FindClosestEdge(hit2.position, out hit2, Agent.areaMask))
                        {
                            Debug.LogError($"Unable to find edge close to {hit2.position} (second attempt)");
                        }

                        float dot = Vector3.Dot(hit2.normal, (target.position - hit2.position).normalized);

                        if (dot < HideSensitivity)
                        {                            
                            var path = new NavMeshPath();

                            Agent.CalculatePath(_moveToPosition, path);

                            for (int counter = 0; counter < path.corners.Length; counter++)
                            {
                                Debug.DrawLine(path.corners[counter], path.corners[counter] + Vector3.up, Color.red, _LookTime);
                                Vector3 directionZ = path.corners[counter] - ManifestedAvatar.transform.position;
                                Vector2 inputZ = new Vector2(directionZ.x, directionZ.z).normalized;                                
                            }

                            if (path.corners.Length > 1)
                            {
                                if (Vector3.Distance(ManifestedAvatar.transform.position, path.corners[cornersTouched]) < .5f)
                                {
                                    cornersTouched++;
                                    if (cornersTouched > path.corners.Length)
                                    {
                                        cornersTouched = path.corners.Length;
                                    }
                                }
                                Vector3 direction = (path.corners[cornersTouched] - ManifestedAvatar.transform.position).normalized;
                                Vector2 input = new Vector2(direction.x, direction.z);
                                InputVector = input;
                            }
                            break;
                        }
                    }
                }
                //else
                //{
                //    Debug.LogError($"Unable to find NavMesh near object {_colliders[i].name} at {_colliders[i].transform.position}");
                //}
            }
            yield return Wait;
        }
    }

    IEnumerator AttackPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(_LookTime * 5);
            if (Eyes.CanSeeTarget() && !ManifestedBarrage.IsBarraging)
            {
                Barrage();
            }
        }
    }

    void Barrage()
    {
        if (_currentState == _activeState && !ManifestedBarrage.IsRecovering)
        {
            ChangeState(_barrageState);
            ManifestedAvatar.StopJumpVelocity();
        }
    }

    void Blast()
    {
        if (_currentState == _activeState && !ManifestedBarrage.IsRecovering && !ManifestedBlast.IsProjectileActive && !ManifestedAvatar.IsDashing)
        {
            _isAiming = true;
            ChangeState(_blastState);
            ManifestedAvatar.StopJumpVelocity();
            ManifestedAvatar.SlowMoveVelocity();
        }
    }

    void PopulateWaypoints()
    {
        for(int i = 0; i < _WaypointObjects.Length; i++)
        {   
            _Waypoints[i] = new Waypoint(_WaypointObjects[i].transform.position);
        }
    }

    void CalculateDistances()
    {
        Waypoint closest = _Waypoints[0];
        for (int i = 0; i < _Waypoints.Length; i++)
        {
            _Waypoints[i].MyProximity = Vector3.Distance(ManifestedAvatar.transform.position, _Waypoints[i].Location);
            _Waypoints[i].TargetProximity = Vector3.Distance(CurrentTarget.transform.position, _Waypoints[i].Location);

            //Chase
            //_Waypoints[i].Weight = _Waypoints[i].TargetProximity - _Waypoints[i].MyProximity;
            //if (i > 0 && _Waypoints[i].Weight < closest.Weight)
            //{
            //    closest = _Waypoints[i];
            //}

            //Semi-evasive
            _Waypoints[i].Weight = _Waypoints[i].TargetProximity + _Waypoints[i].MyProximity;
            if (i > 0 && _Waypoints[i].Weight > closest.Weight && _Waypoints[i].MyProximity < _Waypoints[i].TargetProximity)
            {
                closest = _Waypoints[i];
            }

            //less evasive, but seeks closest better
            //_Waypoints[i].Weight = _Waypoints[i].TargetProximity / _Waypoints[i].MyProximity;
            //if (i > 0 && _Waypoints[i].Weight > closest.Weight)
            //{
            //    closest = _Waypoints[i];
            //}

        }
        Weight = closest.Weight;
        MyProximity = closest.MyProximity;
        TargetProximity = closest.TargetProximity;

        _moveToPosition = closest.Location;
    }

    public int ColliderArraySortComparer(Collider A, Collider B)
    {
        if (A == null && B != null)
        {
            return 1;
        }
        else if (A != null && B == null)
        {
            return -1;
        }
        else if (A == null && B == null)
        {
            return 0;
        }
        else
        {
            return Vector3.Distance(Agent.transform.position, A.transform.position).CompareTo(Vector3.Distance(Agent.transform.position, B.transform.position));
        }
    }

    public Transform Target
    {
        get => CurrentTarget;
        set => CurrentTarget = value;
    }

    //private void OnDisable()
    //{
    //    Eyes.OnGainSight -= HandleGainSight;
    //    Eyes.OnLoseSight -= HandleLoseSight;
    //}

    GameObject ManifestAvatar()
    {
        AvatarType avatar = AvatarToManifest;
        GameObject manifestedAvatar;
        switch (avatar)
        {
            case AvatarType.BALANCED:
                manifestedAvatar = Instantiate(Avatars[0], transform);
                return manifestedAvatar;
            case AvatarType.HEAVY:
                manifestedAvatar = Instantiate(Avatars[1], transform);
                return manifestedAvatar;
            case AvatarType.FLOATY:
                manifestedAvatar = Instantiate(Avatars[2], transform);
                return manifestedAvatar;
            case AvatarType.SWIFT:
                manifestedAvatar = Instantiate(Avatars[3], transform);
                return manifestedAvatar;
        }
        return null;
    }

    IEnumerator GetTargetAvatar()
    {
        ManifestedAvatar = ManifestAvatar().GetComponent<AvatarAspect>();
        yield return new WaitForEndOfFrame();
        if (Player.GetComponentInChildren<AvatarAspect>() != null)
            CurrentTarget = Player.GetComponentInChildren<AvatarAspect>().transform;
        if (CurrentTarget != null)
        {
            SetUpEnemyController();
            StopCoroutine(GetTargetAvatar());
        }
        else
            StartCoroutine(GetTargetAvatar());
    }

    void SetUpEnemyController()
    {        
        ManifestedBarrage = ManifestedAvatar.GetComponentInChildren<BarrageAspect>();
        ManifestedBlast = ManifestedAvatar.GetComponentInChildren<BlastAspect>();
        CurrentTarget = Player.GetComponentInChildren<AvatarAspect>().transform;
        ManifestedAvatar.CurrentTarget = CurrentTarget;
        ManifestedBarrage.CurrentTarget = CurrentTarget;
        ManifestedBlast.CurrentTarget = CurrentTarget;

        Eyes = ManifestedAvatar.GetComponentInChildren<VisionSensor>();
        Eyes.CurrentTarget = CurrentTarget;

        _activeState = new ActiveState(ManifestedAvatar);
        ChangeState(_activeState);
        _barrageState = new BarrageState(_activeState, ManifestedBarrage);
        _blastState = new BlastState(_activeState, ManifestedAvatar, ManifestedBlast);
        _dashState = new DashState(_activeState, ManifestedAvatar);
        _getUpState = new GetUpState(_activeState, ManifestedAvatar);
        _downState = new DownState(_getUpState, ManifestedAvatar);
        
        Agent = ManifestedAvatar.GetComponent<NavMeshAgent>();
        Agent.enabled = true;

        Eyes.OnGainSight += HandleGainSight;
        Eyes.OnLoseSight += HandleLoseSight;

        StartCoroutine(AttackPlayer());
    }
}

