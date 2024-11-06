using UnityEngine;

namespace IIMEngine.SFX
{
    [CreateAssetMenu(fileName = "SFXsBanks", menuName = "IIMEngine/SFX/SFXsBanks")]
    public class SFXsBank : ScriptableObject
    {
        [SerializeField] private SFXData[] _datasList;

        public SFXData[] SFXDatasList => _datasList;
    }
}