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
            // Enter�L�[�ł̃��O�C���ɑΉ�
            usernameInput.onEndEdit.AddListener(OnEndEdit);
            passwordInput.onEndEdit.AddListener(OnEndEdit);

            ShowLoginUI();
        }

        private void OnEndEdit(string _)
        {
            // Enter�������ꂽ�Ƃ��̂ݔ���
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
            //    messageText.text = "���[�U�[���܂��̓p�X���[�h���Ԉ���Ă��܂��B";
            //}
        }

        private void ShowDesktopUI()
        {
            loginPanel.SetActive(false);
            desktopPanel.SetActive(true);
            // �Q�[�����}�E�X���g���Ȃ炱���ŃJ�[�\�������b�N
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