using UnityEngine;

public class TutorialHighlightTarget : MonoBehaviour
{
    [SerializeField] private string id;                   // チュートリアル側から指定するID
    [SerializeField] private Renderer[] renderers;        // モデルのRenderer（単一なら1個でOK）
    [SerializeField] private Material highlightMaterial;  // アウトライン付きマテリアル

    private Material[][] _originalMaterials;              // 元のマテリアル群

    public string Id => id;

    void Awake()
    {
        // Rendererが未設定なら自動取得（任意）
        if (renderers == null || renderers.Length == 0)
        {
            renderers = GetComponentsInChildren<Renderer>();
        }

        // 元のマテリアルをキャッシュ
        _originalMaterials = new Material[renderers.Length][];
        for (int i = 0; i < renderers.Length; i++)
        {
            _originalMaterials[i] = renderers[i].materials;
        }
    }

    public void SetHighlight(bool enabled)
    {
        if (renderers == null || renderers.Length == 0) return;

        if (enabled)
        {
            // 全Rendererをハイライトマテリアルに差し替え
            for (int i = 0; i < renderers.Length; i++)
            {
                var mats = renderers[i].materials;
                for (int j = 0; j < mats.Length; j++)
                {
                    mats[j] = highlightMaterial;
                }
                renderers[i].materials = mats;
            }
        }
        else
        {
            // 元のマテリアルに戻す
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].materials = _originalMaterials[i];
            }
        }
    }
}
