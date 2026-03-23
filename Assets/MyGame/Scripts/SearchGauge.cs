using UnityEngine;
public class SearchGauge
{
    public float CalcLeftFillValue(float score)
    {
        return Mathf.Lerp(0f, 10f, Mathf.InverseLerp(0, 50f, score));
    }

    public float CalcRightFillValue(float score)
    {
        return Mathf.Lerp(10f, 0f, Mathf.InverseLerp(0, 50f, score));
    }

    public float CalcAngleBetweenEnemyAndPlayer(Transform enemy,Transform cam)
    {
        Vector3 dir = enemy.position - cam.position;
        dir.y = 0;

        return Vector3.SignedAngle(cam.transform.forward, dir, Vector3.up);
    }
}
