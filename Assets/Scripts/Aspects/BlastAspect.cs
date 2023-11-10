using UnityEngine;

public class BlastAspect : MonoBehaviour
{
    public bool IsBlasting = false;
    public bool IsGrounded = false;
    public GameObject BlastProjectile;
    public Transform CurrentTarget;
    public Transform BlastAimingRing;

    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] Transform _releasePosition;
    [SerializeField] int _linePoints = 10;
    [SerializeField] float _timeBetweenPoints = .1f;


    private void Awake()
    {
        SetUpBlastAspect();
        BlastAimingRing.gameObject.SetActive(false);
        _lineRenderer.positionCount = 3;
    }

    private void Update()
    {
        if (IsBlasting)
        {
            DrawLine();
        }
    }

    public void BeginBlast()
    {
        IsBlasting = true;
        Vector3 initialPosition = new Vector3(CurrentTarget.position.x, 0, CurrentTarget.position.z);
        BlastAimingRing.position = initialPosition;
        BlastAimingRing.gameObject.SetActive(IsBlasting);
        _lineRenderer.enabled = true;

    }

    public void EndBlast()
    {
        IsBlasting = false;
        BlastAimingRing.gameObject.SetActive(IsBlasting);
        _lineRenderer.enabled = false;
    }

    void DrawLine()
    {
        //This is good enough for now
        Vector3 pointA = transform.position;
        Vector3 pointB = BlastAimingRing.position;
        Vector3 midPoint = (pointA + pointB) / 2;
        float distance = Vector3.Distance(pointA, pointB);
        midPoint.y = (IsGrounded)?distance/3 : midPoint.y;
        _lineRenderer.SetPosition(0, pointA);
        _lineRenderer.SetPosition(1, midPoint);
        _lineRenderer.SetPosition(2, pointB);

    }

    public void PerformRingMove(Vector2 inputVector)
    {
        float speed = .5f;
        Vector3 targetVelocity = transform.TransformDirection(new Vector3(inputVector.x, 0, inputVector.y) * speed);
        BlastAimingRing.Translate(targetVelocity);
    }

    void SetUpBlastAspect()
    {
        BlastAimingRing.parent = GetComponentInParent<PlayerController>().transform;
    }
}
