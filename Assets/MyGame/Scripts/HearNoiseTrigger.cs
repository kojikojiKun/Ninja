using UnityEngine;
using System;

public class HearNoiseTrigger : MonoBehaviour
{
    public Action OnHearNoise;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Noise"))
        {
            OnHearNoise?.Invoke();
        }
    }
}
