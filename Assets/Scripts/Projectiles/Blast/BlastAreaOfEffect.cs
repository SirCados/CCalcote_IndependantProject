using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastAreaOfEffect : MonoBehaviour
{
    int _damage;
    int _stabilityDamage;

    public void SetUpAreaOfEffect(int damage, int stabilityDamage)
    {
        _damage = damage;
        _stabilityDamage = stabilityDamage;
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
