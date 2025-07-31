using UnityEngine;
using UnityEngine.InputSystem;

public class CursorController : MonoBehaviour
{
    // ���݂̏��
    private bool isUIActive = false;

    // PlayerInput���g���Ă���ꍇ�A�Q�Ƃ���ƕ֗�
    [SerializeField] private PlayerInput playerInput;

    void Start()
    {
        SetGameplayMode(); // �����̓Q�[���v���C��ԂƂ���
    }

    void Update()
    {
        // ESC��UI�ؑւ̗�i�{�Ԃł�UIManager�ƘA�g�������ق��������j
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isUIActive)
                SetGameplayMode();
            else
                SetUIMode();
        }
    }

    /// <summary>
    /// UI���샂�[�h�i�}�E�X�J�[�\��ON�A����OFF�j
    /// </summary>
    public void SetUIMode()
    {
        Cursor.lockState = CursorLockMode.None;    // �J�[�\�����R
        Cursor.visible = true;                     // �J�[�\���\��
        isUIActive = true;

        if (playerInput != null)
            playerInput.enabled = false;           // �v���C���[���͂𖳌���
    }

    /// <summary>
    /// �Q�[���v���C���[�h�i�}�E�X�J�[�\��OFF�A����ON�j
    /// </summary>
    public void SetGameplayMode()
    {
        Cursor.lockState = CursorLockMode.Locked;  // �J�[�\���𒆉��ɌŒ�
        Cursor.visible = false;                    // �J�[�\����\��
        isUIActive = false;

        if (playerInput != null)
            playerInput.enabled = true;            // �v���C���[���͂�L����
    }

    /// <summary>
    /// ���S�ɓ��͂��~�߂����Ƃ��iPause�p�j
    /// </summary>
    public void DisableInput()
    {
        if (playerInput != null)
            playerInput.enabled = false;
    }

    public void EnableInput()
    {
        if (playerInput != null)
            playerInput.enabled = true;
    }
}
