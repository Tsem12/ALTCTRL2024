using UnityEngine;

namespace IIMEngine.Music
{
    [CreateAssetMenu(fileName = "MusicsBank", menuName= "IIMEngine/Music/MusicsBank")]
    public class MusicsBank : ScriptableObject
    {
        [SerializeField] private MusicData[] _musicDatas;
        
        public MusicData[] MusicDatas => _musicDatas;
    }
}