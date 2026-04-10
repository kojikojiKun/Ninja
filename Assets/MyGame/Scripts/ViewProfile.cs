using UnityEngine;

[CreateAssetMenu(menuName =("Game/ViewProfile"))]
public class ViewProfile : ScriptableObject
{
    public LayerMask Mask;
    public string TargetTagName;
    public float Angle;
    public float Distanece;
    public float HeightOffset;
}
