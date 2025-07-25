using UnityEngine;

namespace Presenter.Sound
{
    public interface ISoundView
    {
        void PlaySE(AudioId id, float volume = 1f);
        void PlayBGM(AudioId id, bool loop = true, float volume = 1f);
        void StopBGM();
    }
}