using System;
using Cysharp.Threading.Tasks;
using Domain.Game;
using UnityEngine;
using View.UI;

namespace UseCase.Game
{
    public class DocumentUseCase
    {
        DocumentView view;
        DocumentEntity entity;

        Action OnComplete;
        public DocumentUseCase(DocumentView view, DocumentEntity entity)
        {
            this.view = view;
            this.entity = entity;
        }

        public void OpenDocument(Action onComplete)
        {
            if (entity.IsOpen) return;

            OnComplete = onComplete;

            entity.Open();
            view.DisplayDocument(() => CloseDocument());
        }

        public void CloseDocument()
        {
            if (!entity.IsOpen) return;

            entity.Close();
            view.HideDocument().Forget();

            OnComplete?.Invoke();
            OnComplete = null;
        }
    }
}