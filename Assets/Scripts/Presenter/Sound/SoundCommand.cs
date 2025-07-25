using UnityEngine;

namespace Presenter.Sound
{
    public struct SoundCommand
    {
        public AudioId AudioId;
        public AudioCategory Category;
        public bool Loop;   // BGM ‚Ì‚Æ‚«‚Ì‚Ý true
        public float Volume; // 0-1
    }
}