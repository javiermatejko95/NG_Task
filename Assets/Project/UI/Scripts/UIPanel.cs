using UnityEngine;
using UnityEngine.UI;

public class UIPanel : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private Button _btnClose;

    private void Awake()
    {
        _btnClose.onClick.AddListener(OnCloseButtonClicked);
    }

    public virtual void Show()
    {
        _root.SetActive(true);
    }

    public void Hide()
    {
        _root.SetActive(false);
    }

    protected virtual void OnCloseButtonClicked()
    {
        Hide();
    }
}
