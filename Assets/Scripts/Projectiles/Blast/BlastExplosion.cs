using UnityEngine;

public class BlastExplosion : MonoBehaviour
{
    public float ScaleLimit;
    public float ExpansionRate = .1f;
    public Transform ExplosionSphere;

    [SerializeField] int _damage;
    [SerializeField] int _stabilityDamage;

    ParticleSystem _particles;

    private void Awake()
    {
        _particles = GetComponentInChildren<ParticleSystem>();
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
        if (ExplosionSphere.localScale.magnitude < 12)
        {
            ExplosionSphere.localScale = Vector3.Lerp(ExplosionSphere.localScale, ExplosionSphere.localScale * 7, ExpansionRate * Time.deltaTime);
        }
        else
        {
            ExplosionSphere.gameObject.SetActive(false);
        }
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Avatar")
        {
            AvatarAspect target = other.gameObject.GetComponentInParent<AvatarAspect>();
            GiveDamage(target);
        }
    }

    void GiveDamage(AvatarAspect avatar)
    {
        avatar.TakeHit(_damage, _stabilityDamage);
    }
}