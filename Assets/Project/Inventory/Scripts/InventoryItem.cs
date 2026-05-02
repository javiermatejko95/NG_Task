using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDropHandler
{
    [Header("UI")]
    [SerializeField] private Image _imgIcon;
    [SerializeField] private Button _btnRemove;

    private ItemData _item;
    private Canvas _canvas;

    private GameObject _dragIcon;

    private bool _isDragging;

    public bool HasItem => _item != null;

    public static InventoryItem DraggedItem;

    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();        
    }

    private void OnEnable()
    {
        _btnRemove.onClick.AddListener(HandleOnItemRemoved);
        _btnRemove.gameObject.SetActive(HasItem);
    }

    private void OnDisable()
    {
        _btnRemove.onClick.RemoveListener(HandleOnItemRemoved);
    }

    // SET ITEM
    public void SetItem(ItemData item)
    {
        if(item != null)
        {
            _item = item;
            _imgIcon.sprite = item.Icon;
            _imgIcon.enabled = true;
            _btnRemove.gameObject.SetActive(true);
            return;
        }

        Clear();
    }

    // GET ITEM
    public ItemData GetItem()
    {
        return _item;
    }

    // CLEAR SLOT
    public void Clear()
    {
        _item = null;

        _imgIcon.sprite = null;
        _imgIcon.enabled = false;

        _btnRemove.gameObject.SetActive(HasItem);
    }

    // -------------------
    // DRAG
    // -------------------

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!HasItem) return;

        _isDragging = true;
        DraggedItem = this;

        _dragIcon = new GameObject("DragIcon");
        _dragIcon.transform.SetParent(_canvas.transform);

        Image img = _dragIcon.AddComponent<Image>();
        img.sprite = _imgIcon.sprite;
        img.raycastTarget = false;

        _dragIcon.transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // TODO: Fix bug where the dragged icon stays on the screen after closing the inventory window with I key
        if (_dragIcon == null) return;

        _dragIcon.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_dragIcon != null)
        {
            Destroy(_dragIcon);
        }

        DraggedItem = null;
        _isDragging = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!HasItem || _isDragging || DraggedItem != null) return;

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            UseItem();
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (DraggedItem == null || DraggedItem == this) return;

        InventoryEvents.RaiseOnItemSwapped(DraggedItem, this);
    }

    // -------------------
    // TOOLTIP
    // -------------------

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!HasItem) return;

        TooltipSystem.Instance.Show(_item, (RectTransform)transform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.Instance.Hide();
    }

    private void HandleOnItemRemoved()
    {
        InventoryEvents.RaiseOnItemRemoved(this);
    }

    private void UseItem()
    {
        InventoryEvents.RaiseOnItemUsed(this);
    }
}
