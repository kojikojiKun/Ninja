using UnityEngine;
public class DiscoveryScore
{
    private const float MAX_SCORE = 50;
    public float IncreaceScore(float totalScore,float ViewingScore)
    {
        totalScore += ViewingScore * Time.deltaTime;
        return Mathf.Min(totalScore, MAX_SCORE); ;
    }

    public float DecreaceScore(float totalScore)
    {
        totalScore = Mathf.MoveTowards(totalScore, 0f,30f* Time.deltaTime);
        return totalScore;
    }
}