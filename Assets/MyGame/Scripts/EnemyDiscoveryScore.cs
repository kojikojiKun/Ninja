using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DiscoveryScoreCoefficients
{
    public float LightExponent;
    public float DistanceExponent;
    public float ScoreReductionSpeed;
}

public class EnemyDiscoveryScore
{
    private Transform m_origin;
    private ViewProfile m_profile;
    private LightVisibilityEvaluator m_lightEvaluator;
    private DiscoveryScoreCoefficients m_coefficients;   
    private List<LightZone> m_lightZones = new List<LightZone>();
    private List<Light> m_lights = new List<Light>();
    private float m_brightness;
    private float m_totalScore;
    private const float MAX_VIEW_SCORE = 40f;

    public EnemyDiscoveryScore(ViewProfile profile, DiscoveryScoreCoefficients coefficients,Transform origin)
    {
        m_lightEvaluator = new LightVisibilityEvaluator();
        m_profile = profile;
        m_coefficients = coefficients;
        m_origin = origin;
    }

    public void RegistLights(List<LightToCheck> lightToChecks)
    {
        foreach (var check in lightToChecks)
        {
            Light light = check.Light;
            m_lights.Add(light);
        }
    }

    public void RegistLightZone(LightZone zone)
    {
        m_lightZones.Add(zone);
        m_lightEvaluator.CalkBrightness(m_lightZones);
    }

    public void UnRegistLightZone(LightZone zone)
    {
        m_lightZones.Remove(zone);
        m_lightEvaluator.CalkBrightness(m_lightZones);
    }

    public void CheckScore(Transform target, bool isSeeTarget)
    {
        float viewScore = m_lightEvaluator.EaseOfViewingScore(
                m_origin.position,
                target.position,
                m_brightness,
                m_profile.Distanece,
                m_coefficients.LightExponent,
                m_coefficients.DistanceExponent,
                m_lights
                );

        float delta = CalcScore(
            isSeeTarget,
            viewScore,
            Time.deltaTime
            );

        m_totalScore += delta;
        m_totalScore = Mathf.Clamp(m_totalScore, 0f, MAX_VIEW_SCORE);
    }

    public float CalcScore(bool isSee, float viewScore, float deltaTime)
    {
        if (isSee)
        {
            return viewScore * deltaTime;
        }
        else
        {
            return -m_coefficients.ScoreReductionSpeed * deltaTime;
        }
    } 

    public float GetScorePercentage()
    {
        return m_totalScore / MAX_VIEW_SCORE;
    }
    
    public float GetTotalScore()
    {
        return m_totalScore;
    }
}