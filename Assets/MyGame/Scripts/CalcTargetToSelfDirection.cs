using UnityEngine;

public class CalcTargetToSelfDirection
{
    public AssassinateDirection GetTargetToSelfDirection(Transform target, Transform self)
    {
        Vector2 dir = CalcDirection(target, self);

        if (Mathf.Abs(dir.y) > Mathf.Abs(dir.x))
        {
            if (dir.y > 0)
                return AssassinateDirection.Front;
            else
                return AssassinateDirection.Back;
        }
        else
        {
            if (dir.x > 0)
                return AssassinateDirection.Right;
            else
                return AssassinateDirection.Left;
        }
    }

    private Vector2 CalcDirection(Transform target, Transform self)
    {
        Vector3 diff = self.position - target.position;

        //targetŠî€‚É•ÏŠ·.
        Vector3 targetLocal = target.InverseTransformDirection(diff);

        return new Vector2(targetLocal.x, targetLocal.z);
    }
}