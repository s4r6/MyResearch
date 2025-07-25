using System;
using System.Globalization;
using UniRx;
using UnityEngine;

namespace Presenter.Sound
{
    public class SoundPresenter : IDisposable
    {
        readonly CompositeDisposable _disposables = new();
        readonly ISoundView _view;

        public SoundPresenter(ISoundView view)
        {
            _view = view;
        }

        public void PlaySE(AudioId id, float vol = 1f)
        {
            _view.PlaySE(id, vol);
        }

        public void PlayBGM(AudioId id, bool loop, float vol = 1f)
        {
            _view.PlayBGM(id, loop, vol);
        }

        public void Dispose() => _disposables.Dispose();
    }

}