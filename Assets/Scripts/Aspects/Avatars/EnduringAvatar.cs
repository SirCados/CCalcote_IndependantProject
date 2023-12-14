using System.Collections;
using UnityEngine;

public class EnduringAvatar : AvatarAspect
{
    private void Update()
    {
        if (!IsDashing && RemainingAirDashes == 0)
        {
            StartCoroutine(EnduringCharge());
        }
    }

    IEnumerator EnduringCharge()
    {
        IsInvulnerable = true;
        //play Invulnerable sound here
        //visual effect toggle here
        yield return new WaitForSecondsRealtime(1f);
        IsInvulnerable = false;
    }
}
