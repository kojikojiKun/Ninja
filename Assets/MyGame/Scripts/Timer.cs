using UnityEngine;

public struct Timer
{
    public float m_time;

    public bool IsOutOfDuration(float duration)
    {
        //Œo‰ßŽžŠÔŒv‘ª.
        m_time += Time.deltaTime;
        if (m_time >= duration)
        {
            m_time = 0;
            return true;
        }
        return false;
    }

    public void Reset() => m_time = 0;
}
