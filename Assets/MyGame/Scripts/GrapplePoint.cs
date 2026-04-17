using UnityEngine;
using System;

[RequireComponent(typeof(Collider))]
public class GrapplePoint : MonoBehaviour,IGrappleable
{
    private Collider m_collider;

    private void Awake()
    {
        m_collider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        if (Registries.Instance != null)
        {
            Registries.Instance.GrapplePointRegister(m_collider);
        }
        else
        {
            Registries.OnReady += Regist;
        }
    }

    public Transform GetGrapplePoint()
    {
        return this.transform;
    }

    void Regist()
    {
        Registries.OnReady -= Regist;
        Registries.Instance.GrapplePointRegister(m_collider);
    }
}
