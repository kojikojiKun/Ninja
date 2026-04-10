using UnityEngine;
using System;
public class LightZoneTrigger : MonoBehaviour
{
    public event Action<LightZone> OnLightEnter;
    public event Action<LightZone> OnLightExit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out LightZone zone))
        {
            OnLightEnter?.Invoke(zone);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent(out LightZone zone))
        {
            OnLightExit?.Invoke(zone);
        }
    }
}
