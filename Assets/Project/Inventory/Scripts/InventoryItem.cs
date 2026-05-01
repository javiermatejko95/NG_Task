using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI")]
    public Image icon;

    private ItemData _item;
    private Canvas _canvas;

    private GameObject _dragIcon;

    public bool HasItem => _item != null;

    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
    }

    // SET ITEM
    public void SetItem(ItemData item)
    {
        _item = item;

        icon.sprite = item.Icon;
        icon.enabled = true;
    }

    // CLEAR SLOT
    public void Clear()
    {
        _item = null;

        icon.sprite = null;
        icon.enabled = false;
    }

    // -------------------
    // DRAG
    // -------------------

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!HasItem) return;

        _dragIcon = new GameObject("DragIcon");
        _dragIcon.transform.SetParent(_canvas.transform);

        Image img = _dragIcon.AddComponent<Image>();
        img.sprite = icon.sprite;
        img.raycastTarget = false;

        _dragIcon.transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_dragIcon == null) return;

        _dragIcon.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_dragIcon != null)
        {
            Destroy(_dragIcon);
        }
    }

    // -------------------
    // TOOLTIP
    // -------------------

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!HasItem) return;

        //TooltipSystem.Show(_item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //TooltipSystem.Hide();
    }
}
