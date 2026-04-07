using UnityEngine;

[RequireComponent (typeof(Light))]
public class LightToCheck : MonoBehaviour
{
    public Light Light{get; private set;}

    private void OnEnable()
    {
        Registries.Instance.LightRegister(this);
    }

    private void OnDisable()
    {
        Registries.Instance.LightUnRegister(this);
    }
}
