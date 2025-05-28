using UnityEngine;
using UnityEngine.UI;
using UseCase.Stage;

namespace View.UI
{
    public class LoginView : MonoBehaviour
    {
        //--------------------VIEW-------------------------
        [Header("UI References")]
        [SerializeField] private GameObject loginPanel;
        [SerializeField] private GameObject desktopPanel;
        [SerializeField] private InputField usernameInput;
        [SerializeField] private InputField passwordInput;
        [SerializeField] private Text messageText;

        private void Start()
        {
            // Enterキーでのログインに対応
            usernameInput.onEndEdit.AddListener(OnEndEdit);
            passwordInput.onEndEdit.AddListener(OnEndEdit);

            ShowLoginUI();
        }

        private void OnEndEdit(string _)
        {
            // Enterが押されたときのみ発火
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                OnLoginClicked();
            }
        }

        private void ShowLoginUI()
        {
            loginPanel.SetActive(true);
            desktopPanel.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void OnLoginClicked()
        {
            string username = usernameInput.text;
            string password = passwordInput.text;

            //if (username == validUsername && password == validPassword)
            //{
            //    messageText.text = "";
            //    ShowDesktopUI();
            //}
            //else
            //{
            //    messageText.text = "ユーザー名またはパスワードが間違っています。";
            //}
        }

        private void ShowDesktopUI()
        {
            loginPanel.SetActive(false);
            desktopPanel.SetActive(true);
            // ゲーム内マウスを使うならここでカーソルをロック
            // Cursor.lockState = CursorLockMode.Locked;
            // Cursor.visible = false;
        }


        //--------------------PRESENTER------------------------------
        PCUseCase usecase;
        public void OnLogin(string objectId)
        {
            ShowDesktopUI();
        }
    }
}