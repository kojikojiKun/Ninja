using UnityEngine;
public class DiscoveryScore
{
    private float m_maxScore;

    public DiscoveryScore(float maxScore)
    {
        m_maxScore = maxScore;
    }

    public float IncreaceScore(float totalScore, float ViewingScore)
    {
        totalScore += ViewingScore * Time.deltaTime;
        return Mathf.Min(totalScore, m_maxScore); ;
    }

    public float DecreaceScore(float totalScore)
    {
        totalScore = Mathf.MoveTowards(totalScore, 0f, 30f * Time.deltaTime);
        return totalScore;
    }
}