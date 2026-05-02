using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    public static TooltipSystem Instance;

    [SerializeField] private TooltipUI _tooltipUI;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Show(ItemData item, RectTransform target)
    {
        _tooltipUI.SetData(item);
        _tooltipUI.Show();
        _tooltipUI.SetPosition(target);
    }

    public void Hide()
    {
        _tooltipUI.Hide();
    }
}
