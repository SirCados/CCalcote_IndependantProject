using UnityEngine;

public class BlastAspect : MonoBehaviour
{
    public bool IsBlasting = false;
    public GameObject BlastProjectile;

    private void Awake()
    {
        SetUpBlastAspect();
    }

    public void BeginBlast()
    {
        IsBlasting = true;
    }

    public void EndBlast()
    {
        IsBlasting = false;
    }

    void SetUpBlastAspect()
    {

    }
}
