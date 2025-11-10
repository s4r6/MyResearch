using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;
using UniRx;
using System;
using UnityEngine.UI;
using Domain.Stage;
using UnityEngine.InputSystem;

namespace View.UI
{
    public class DetailWindowManager : MonoBehaviour, IDisposable
    {
        [SerializeField]
        List<GameObject> DetailWindows = new();
        [SerializeField]
        ResultInputProvider inputProvider;

        [SerializeField]
        PlayerInput input;

        int currentPage = 0;

        SurmmaryDetailDTO DisplayData;
        bool IsDisplaying = false;

        CompositeDisposable _disposables = new();

        public Subject<Unit> onClosed = new();

        void Start()
        {
            foreach (GameObject window in DetailWindows)
            {
                window.GetComponentInChildren<Button>().onClick.AddListener(CloseDetail);
                window.SetActive(false);
            }

            inputProvider.OnPageChangeButtonPressed
                .Where(_ => IsDisplaying)
                .Subscribe(x =>
                {
                    if(x > 0)
                    {
                        NextPage();
                    }
                    else if(x < 0)
                    {
                        PrevPage();
                    }
                }).AddTo(_disposables);
        }

        public void DisplayDetail(SurmmaryDetailDTO data)
        {
            input.SwitchCurrentActionMap("Result");

            currentPage = 0;
            DisplayData = data;
            IsDisplaying = true;

            foreach(var window in DetailWindows)
            {
                window.GetComponent<IDetailWindow>().SetData(data);
            }

            DetailWindows[currentPage].SetActive(true);
        }

        public void CloseDetail()
        {
            IsDisplaying = false;
            DetailWindows[currentPage].SetActive(false);
            onClosed.OnNext(default);
        }

        public void NextPage()
        {
            DetailWindows[currentPage].SetActive(false);
            currentPage++;
            currentPage = Mathf.Clamp(currentPage, 0, DetailWindows.Count-1);
            DetailWindows[currentPage].SetActive(true);
        }

        public void PrevPage()
        {
            DetailWindows[currentPage].SetActive(false);
            currentPage--;
            currentPage = Mathf.Clamp(currentPage, 0, DetailWindows.Count-1);
            DetailWindows[currentPage].SetActive(true);
        }

        void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}