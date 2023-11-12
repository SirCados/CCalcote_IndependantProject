using System.Collections;
using UnityEngine;

public class TenaciousAvatar : AvatarAspect
{
    private void Update()
    {
        if(IsDashing && RemainingAirDashes == 0)
        {
            StartCoroutine(TenaciousDash());
        }
    }
    public override IEnumerator RegainStability()
    {
        if (IsKnockedDown)
        {
            yield return new WaitForSecondsRealtime(3);
            GetUpSequence();
        }
        yield return new WaitForSecondsRealtime(3);
        //Play recovery sound here
        while (CurrentStability < _maximumStability && !IsKnockedDown)
        {
            CurrentStability += 1;
            if (CurrentHealth < _maximumHealth)
            {
                CurrentHealth += 2;
                if(CurrentHealth > _maximumHealth)
                {
                    CurrentHealth = _maximumHealth;
                }
            }
            yield return new WaitForSecondsRealtime(1f);
        }
    }

    IEnumerator TenaciousDash()
    {
        IsSturdy = true;
        //play sturdy sound here
        //visual effect toggle here
        yield return new WaitForSecondsRealtime(.5f);
        IsSturdy = false;
    }
}
