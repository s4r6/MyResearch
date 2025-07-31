using UnityEngine;
using UnityEngine.InputSystem;

public class CursorController : MonoBehaviour
{
    // 現在の状態
    private bool isUIActive = false;

    // PlayerInputを使っている場合、参照すると便利
    [SerializeField] private PlayerInput playerInput;

    void Start()
    {
        SetGameplayMode(); // 初期はゲームプレイ状態とする
    }

    void Update()
    {
        // ESCでUI切替の例（本番ではUIManagerと連携させたほうがいい）
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isUIActive)
                SetGameplayMode();
            else
                SetUIMode();
        }
    }

    /// <summary>
    /// UI操作モード（マウスカーソルON、入力OFF）
    /// </summary>
    public void SetUIMode()
    {
        Cursor.lockState = CursorLockMode.None;    // カーソル自由
        Cursor.visible = true;                     // カーソル表示
        isUIActive = true;

        if (playerInput != null)
            playerInput.enabled = false;           // プレイヤー入力を無効化
    }

    /// <summary>
    /// ゲームプレイモード（マウスカーソルOFF、入力ON）
    /// </summary>
    public void SetGameplayMode()
    {
        Cursor.lockState = CursorLockMode.Locked;  // カーソルを中央に固定
        Cursor.visible = false;                    // カーソル非表示
        isUIActive = false;

        if (playerInput != null)
            playerInput.enabled = true;            // プレイヤー入力を有効化
    }

    /// <summary>
    /// 完全に入力を止めたいとき（Pause用）
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
