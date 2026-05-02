using TMPro;
using UnityEngine;

public class TooltipUI : UIPanel
{
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _description;

    public void SetData(ItemData itemData)
    {
        _title.text = itemData.ItemName;
        _description.text = itemData.TooltipDescription;
    }

    public void SetPosition(RectTransform target)
    {
        Vector3[] corners = new Vector3[4];
        target.GetWorldCorners(corners);

        Vector3 position = corners[2];

        _root.transform.position = position + new Vector3(10f, 10f, 0f);
    }
}
