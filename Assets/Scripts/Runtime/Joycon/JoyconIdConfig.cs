using UnityEngine;

[CreateAssetMenu]
public class JoyconIdConfig : ScriptableObject
{
    [field: SerializeField] public int CenterJoyconId { get; private set; }
    [field: SerializeField] public int LeftJoyconId { get; private set; }
    [field: SerializeField] public int RightJoyconId { get; private set; }

    public int GetMaxId => Mathf.Max(new int[]{ CenterJoyconId, LeftJoyconId, RightJoyconId });
}