using UnityEngine;

public class BlastAspect : MonoBehaviour
{
    public bool IsBlasting = false;
    public GameObject BlastProjectile;
    public Transform CurrentTarget;

    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] Transform _releasePosition;
    [SerializeField] int _linePoints = 25;
    [SerializeField] float _timeBetweenPoints = .1f;


    private void Awake()
    {
        SetUpBlastAspect();
    }

    private void Update()
    {
        if (IsBlasting)
        {
            DrawProjection();
        }
        else
        {
            _lineRenderer.enabled = false;
        }
    }

    public void BeginBlast()
    {
        IsBlasting = true;
    }

    public void EndBlast()
    {
        IsBlasting = false;
        
    }

    void DrawProjection()
    {
        _lineRenderer.enabled = true;
        _lineRenderer.positionCount = Mathf.CeilToInt(_linePoints / _timeBetweenPoints) + 1;
        print(_lineRenderer.positionCount);
        _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, CurrentTarget.position);
        Vector3 startPosition = _releasePosition.position;
        Vector3 startVelocity = transform.forward * 20;
        int counter = 0;
        _lineRenderer.SetPosition(counter, startPosition);
        for (float time = 0; time < _linePoints; time += _timeBetweenPoints)
        {
            counter++;
            Vector3 point = startPosition + time * startVelocity;
            point.y = startPosition.y + startVelocity.y * time + (Physics.gravity.y / 2f * time * time);
            _lineRenderer.SetPosition(counter, point);
        }
    }

    void SetUpBlastAspect()
    {
        
    }
}
