using UnityEngine;

[RequireComponent (typeof(Light))]
public class LightToCheck : MonoBehaviour
{
    public Light Light { get; private set; }

    private void Awake()
    {
        Light = GetComponent<Light>();
    }
}
