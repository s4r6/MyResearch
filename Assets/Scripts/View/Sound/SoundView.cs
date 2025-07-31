using System.Collections.Generic;
using Presenter.Sound;
using UnityEngine;

namespace View.Sound
{
    public class SoundView : MonoBehaviour, ISoundView
    {
        [SerializeField] AudioCueSheet _cueSheet;
        [Header("AudioSource Settings")]
        [SerializeField] AudioSource _bgmSource;
        [SerializeField] AudioSource _sePrefab;
        [SerializeField] int _initialSePoolSize = 4;

        readonly Queue<AudioSource> _sePool = new();

        void Awake()
        {
            DontDestroyOnLoad(this);
            for (int i = 0; i < _initialSePoolSize; i++)
            {
                var src = Instantiate(_sePrefab, transform);
                src.playOnAwake = false;
                _sePool.Enqueue(src);
            }
        }

        public void PlaySE(AudioId id, float volume = 1f)
        {
            var clip = _cueSheet[id].Clip;
            var src = GetSeSource();
            src.clip = clip;
            src.volume = volume;
            src.loop = false;
            src.Play();
            StartCoroutine(ReturnSeSource(src));
        }

        public void PlayBGM(AudioId id, bool loop = true, float volume = 1f)
        {
            var clip = _cueSheet[id].Clip;
            if (_bgmSource.clip == clip && _bgmSource.isPlaying)
                return;

            _bgmSource.clip = clip;
            _bgmSource.volume = volume;
            _bgmSource.loop = loop;
            _bgmSource.Play();
        }

        public void StopBGM() => _bgmSource.Stop();

        AudioSource GetSeSource()
        {
            if (_sePool.Count > 0)
                return _sePool.Dequeue();
            return Instantiate(_sePrefab, transform);
        }

        System.Collections.IEnumerator ReturnSeSource(AudioSource src)
        {
            yield return new WaitWhile(() => src.isPlaying);
            _sePool.Enqueue(src);
        }
    }
}