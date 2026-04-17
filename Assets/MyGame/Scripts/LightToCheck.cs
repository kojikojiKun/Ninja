using UnityEngine;
using System;

[RequireComponent (typeof(Light))]
public class LightToCheck : MonoBehaviour
{
    public Light Light { get; private set; }

    private void Awake()
    {
        Light = GetComponent<Light>();
    }

    private void OnEnable()
    {
        if (Registries.Instance != null)
        {
            Registries.Instance.LightRegister(this);
        }
        else
        {
            Registries.OnReady += Regist;
        }
    }

    void Regist()
    {
        Registries.OnReady -= Regist;
        Registries.Instance.LightRegister(this);
    }
}
