using UnityEngine;

[CreateAssetMenu]
public class JoyconIdConfig : ScriptableObject
{
    [field: SerializeField] public int _centerJoyconId { get; private set; }
    [field: SerializeField] public int _leftJoyconId { get; private set; }
    [field: SerializeField] public int _rightJoyconId { get; private set; }

    public int GetMaxId => Mathf.Max(new int[]{ _centerJoyconId, _leftJoyconId, _rightJoyconId });
}