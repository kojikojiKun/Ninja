using System.Collections.Generic;
using UnityEngine;

public class LightVisibilityEvaluator
{
    private LayerMask m_shadowBlock;
    private const float DEFAULT_DARK_VALUE = 0.2f;
    private const float MIN_LIGHT_SCORE = 1;

    public float EaseOfViewingScore(Vector3 evalTarget,
        Vector3 targetPos, 
        float brightness,
        float viewDistance
        ,float lightExp,
        float distanceExp,
        List<Light> lights
        )
    {
        if (brightness == 0)
            brightness = 0.2f;

        //0除算回避のためMathf.Max(distance,0.1f)を使用.
        float distance = Mathf.Max(Vector3.Distance(evalTarget, targetPos), 0.1f);

        float lightScore = GetLightScore(targetPos, lights);

        //targetとの距離を1から10の割合で返す.
        float distanceScore = Mathf.Lerp(0.1f, 1f, (distance / viewDistance));

        float totalScore = Mathf.Pow((brightness * lightScore), lightExp) / Mathf.Pow(distanceScore, distanceExp);

        return totalScore;
    }

    public float CalkBrightness(List<LightZone> lightZones)
    {
        //一つも接触していないとき.
        if (lightZones.Count == 0)
            return DEFAULT_DARK_VALUE;

        float max = 0f;

        //複数のLightZone(Collider)に接触した場合最もBrightnessの値が大きいLightZoneのBrightnessを参照する.
        foreach (var zone in lightZones)
        {
            max = Mathf.Max(max, zone.Brightness);
        }

        return max;
    }

    //Lightのrangeの中心に近いほど高いスコアを返す.
    public float GetLightScore(Vector3 targetPos,List<Light> lights)
    {
        float total = MIN_LIGHT_SCORE;
        foreach (var light in lights)
        {
            if (!light.enabled)
                continue;

            Vector3 dir = targetPos - light.transform.position;
            float distSqr = dir.sqrMagnitude;
            float rangeSqr = light.range * light.range;

            if (distSqr > rangeSqr)
                continue;

            float dist = Mathf.Sqrt(distSqr);

            if (IsWithinShadow(light, targetPos))
                continue;

            float atten = 1.0f - dist / light.range;
            total += light.intensity * atten;
        }
        return total;
    }

    //ライトからRayを射出し、Rayにターゲット以外のオブジェクトが触れれば影の中と判定する.
    bool IsWithinShadow(Light light, Vector3 targetPos)
    {
        Vector3 dir = targetPos - light.transform.position;

        if (Physics.Raycast(
            light.transform.position,
            dir.normalized,
            out RaycastHit hit,
            light.range,
            m_shadowBlock
            ))
        {
            return true;
        }

        return false;
    }
}
