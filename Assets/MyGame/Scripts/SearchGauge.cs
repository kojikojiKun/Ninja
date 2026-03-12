using UnityEngine;
public class SearchGauge
{
    public float CalcFillValue(float score)
    {
        Debug.Log(score);
        return Mathf.Lerp(4.4f, 10f, Mathf.InverseLerp(0, 50f, score));
    }
}
