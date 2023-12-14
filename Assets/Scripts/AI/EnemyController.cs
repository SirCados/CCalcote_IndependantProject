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
    AvatarAspect _enemyAvatar;

    private Coroutine MovementCoroutine;

    public Vector2 InputVector;

    [SerializeField] GameObject[] _WaypointObjects = new GameObject[4];

    [SerializeField] Waypoint[] _Waypoints = new Waypoint[4];

    Vector3 _moveToPosition;

    public float MyProximity = 0;

    private void Awake()
    {
        SetUpEnemyController();
    }

    private void Start()
    {
        PopulateWaypoints();
    }

    private void FixedUpdate()
    {
        if(!ManifestedAvatar.IsKnockedDown && !ManifestedAvatar.IsGettingUp)
        {
            ManifestedAvatar.PerformMove(InputVector);
        }
    }

    private void HandleGainSight(Transform target)
    {
        if (MovementCoroutine != null)
        {
            StopCoroutine(MovementCoroutine);
        }
        //Get Current Target by Raycast in VisionSensor
        CurrentTarget = target;        
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
                                print(direction);
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

    void PopulateWaypoints()
    {
        for(int i = 0; i < _WaypointObjects.Length; i++)
        {   
            _Waypoints[i] = new Waypoint(_WaypointObjects[i].transform.position);
        }

        //StartCoroutine(CalculateDistances());
    }

    void CalculateDistances()
    {
        Waypoint closest = _Waypoints[0];
        for (int i = 0; i < _Waypoints.Length; i++)
        {
            _Waypoints[i].MyProximity = Vector3.Distance(ManifestedAvatar.transform.position, _Waypoints[i].Location);
            _Waypoints[i].TargetProximity = Vector3.Distance(CurrentTarget.transform.position, _Waypoints[i].Location);
            _Waypoints[i].Weight = _Waypoints[i].TargetProximity - _Waypoints[i].MyProximity;//Chase
            //_Waypoints[i].Weight = _Waypoints[i].MyProximity - _Waypoints[i].TargetProximity;
            if (i > 0 && _Waypoints[i].Weight < closest.Weight)
            {
                closest = _Waypoints[i];
            }
        }
        MyProximity = closest.MyProximity;

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

    private void OnEnable()
    {
        Eyes.OnGainSight += HandleGainSight;
        Eyes.OnLoseSight += HandleLoseSight;
    }

    private void OnDisable()
    {
        Eyes.OnGainSight -= HandleGainSight;
        Eyes.OnLoseSight -= HandleLoseSight;
    }

    void SetUpEnemyController()
    {
        ManifestedAvatar = GetComponentInChildren<AvatarAspect>();        
        Agent = ManifestedAvatar.GetComponent<NavMeshAgent>();
        Agent.enabled = true;
    }
}

