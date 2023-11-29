using UnityEngine;

public class BlastExplosion : MonoBehaviour
{
    public float ScaleLimit;
    public float ExpansionRate = .1f;
    Transform _explosionSphere;

    [SerializeField] int _damage;
    [SerializeField] int _stabilityDamage;

    [SerializeField] BlastAreaOfEffect _area;

    private void Awake()
    {
        _area.SetUpAreaOfEffect(_damage, _stabilityDamage);
        _explosionSphere = _area.gameObject.transform;
    }

    private void Start()
    {
        Invoke("DestroySelf", 1f);
    }

    void Update()
    {
        if (gameObject.activeSelf)
        {
            transform.Rotate(0, 500 * Time.deltaTime, 0);
            ScaleSphere();
        }
    }

    void ScaleSphere()
    {
        if (_explosionSphere.localScale.magnitude < ScaleLimit)
        {
            _explosionSphere.localScale = Vector3.Lerp(_explosionSphere.localScale, _explosionSphere.localScale * 7, ExpansionRate * Time.deltaTime);
        }
        else
        {
            _explosionSphere.gameObject.SetActive(false);
        }
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}
