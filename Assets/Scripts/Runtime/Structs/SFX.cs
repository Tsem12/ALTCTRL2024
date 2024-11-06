using IIMEngine.SFX;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class SFX
{
    [SerializeField] private Transform _source;
    [SerializeField] private string _name;
    public UnityEvent OnPlay;
    private SFXInstance _currentSFXInstance;

    public void SetSource(Transform source) => _source = source;
    public Transform Source => _source;

    public void PlaySfx()
    {
        if (_currentSFXInstance != null)
        {
            Debug.LogWarning($"Currently playing sound: <b>{_name}</b> has been stop to replay it");
            StopSfx();
        } 
        _currentSFXInstance = SFXsManager.Instance.PlaySound(_name, _source);
        OnPlay?.Invoke();
    }

    public void StopSfx() => _currentSFXInstance.AudioSource.Stop();

}
