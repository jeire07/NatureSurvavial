using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    public Button button;
    public Image icon;
    public TextMeshProUGUI quatityText;
    private ItemSlot _curSlot;
    private Outline _outline;

    public int index;
    public bool equipped;

    private void Awake()
    {
        _outline = GetComponent<Outline>();
    }

    private void OnEnable()
    {
        _outline.enabled = equipped;
    }

    public void Set(ItemSlot slot)
    {
        _curSlot = slot;
        icon.gameObject.SetActive(true);
        icon.sprite = slot.Item.icon;
        quatityText.text = slot.Quantity > 1 ? slot.Quantity.ToString() : string.Empty;

        if (_outline != null)
        {
            _outline.enabled = equipped;
        }
    }

    public void Clear()
    {
        _curSlot = null;
        icon.gameObject.SetActive(false);
        quatityText.text = string.Empty;
    }

    public void OnButtonClick()
    {
        Inventory.Instance.SelectItem(index);
    }
}
