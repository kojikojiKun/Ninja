using UnityEngine;

public class LightVisibilityEvaluator
{
    [SerializeField] private LayerMask m_shadowBlock;
    private LightToCheck[] m_lightsToCheck;
    private Light[] m_lights;

    public LightVisibilityEvaluator(LightToCheck[] lightToChecks)
    {
        m_lightsToCheck=lightToChecks;
        m_lights = new Light[lightToChecks.Length];
        for(int i = 0; i < lightToChecks.Length; i++)
        {
            m_lights[i] = m_lightsToCheck[i].GetComponent<Light>();
        }
    }

    //ターゲットのオブジェクトの周囲の明るさを発見しやすさのスコアとして返す.
    public float GetLightScore(Vector3 targetPos)
    {
        float total = 0;
        foreach(var light in m_lights)
        {
            if(!light.enabled)
                continue;

            Vector3 dir = targetPos- light.transform.position;
            float distSqr = dir.sqrMagnitude;
            float rangeSqr = light.range * light.range;

            if(distSqr > rangeSqr) 
                continue;

            float dist=Mathf.Sqrt(distSqr);

            if (IsWithinShadow(light, targetPos))
                continue;

            float atten = 1.0f - dist / light.range;
            total += light.intensity * atten;
        }
        return total;
    }

    //ライトからRayを射出し、Rayにターゲット以外のオブジェクトが触れれば影の中と判定する.
    bool IsWithinShadow(Light light,Vector3 targetPos)
    {
        Vector3 dir = targetPos - light.transform.position;
        float dist=dir.magnitude;

        if(Physics.Raycast(
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
