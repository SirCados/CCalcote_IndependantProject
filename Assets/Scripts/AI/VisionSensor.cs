using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(SphereCollider))]
public class VisionSensor : MonoBehaviour
{
    [SerializeField] LayerMask DetectionMask = ~0;
    [SerializeField][Range(.01f, .5f)] float _VisionRefreshRate = .2f;
    public Transform CurrentTarget;

    AvatarAspect _enemyAvatar;

    private void Start()
    {
        StartCoroutine(GetEnemyAvatar());
    }

    IEnumerator GetEnemyAvatar()
    {
        yield return new WaitForEndOfFrame();
        if (CurrentTarget.GetComponentInChildren<AvatarAspect>() != null)
            _enemyAvatar = CurrentTarget.GetComponentInChildren<AvatarAspect>();
        if (_enemyAvatar != null)
        {
            StartCoroutine(LookForTarget());
            StopCoroutine(GetEnemyAvatar());
        }
    }

    IEnumerator LookForTarget()
    {
        while (true)
        {
            yield return new WaitForSeconds(_VisionRefreshRate);
            if (CanSeeTarget())
            {
                print("I see you.");
            }

        }
    }

    bool CanSeeTarget()
    {
        if (_enemyAvatar != null)
        {   
            RaycastHit hit;
            Vector3 myLocation = transform.position;
            Vector3 targetLocation = _enemyAvatar.transform.position;
            Ray ray = new Ray(myLocation, targetLocation - myLocation);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.CompareTag("Avatar"))
                {
                    Debug.DrawLine(myLocation, targetLocation, Color.red, _VisionRefreshRate);
                    print(hit.transform.name);
                    return true;
                }
            }
        }

        return false;
    }
}
