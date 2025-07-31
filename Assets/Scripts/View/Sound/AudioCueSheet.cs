using Presenter.Sound;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioCueSheet", menuName = "Scriptable Objects/AudioCueSheet")]
public class AudioCueSheet : ScriptableObject
{
    [Serializable]
    public class Cue
    {
        public AudioId Id;
        public AudioClip Clip;
        public AudioCategory Category;
    }

    [SerializeField] Cue[] _cues;

    readonly Dictionary<AudioId, Cue> _map = new();

    void OnEnable()
    {
        _map.Clear();
        foreach (var cue in _cues)
            _map[cue.Id] = cue;
    }

    public Cue this[AudioId id] => _map[id];
}
