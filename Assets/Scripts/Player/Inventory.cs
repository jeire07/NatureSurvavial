using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ItemSlot
{
    public ItemData Item;
    public int Quantity;
}

public class Inventory : MonoBehaviour
{
    public ItemSlotUI[] UiSlots;
    public ItemSlot[] Slots;

    public GameObject InventoryWindow;
    public Transform DropPosition;

    [Header("Selected Item")]
    private ItemSlot _selectedItem;
    private int _selectedItemIndex;
    public TextMeshProUGUI SelectedItemName;
    public TextMeshProUGUI SelectedItemDescription;
    public TextMeshProUGUI SelectedItemStatNames;
    public TextMeshProUGUI SelectedItemStatValues;
    public GameObject UseButton;
    public GameObject EquipButton;
    public GameObject UnEquipButton;
    public GameObject DropButton;

    private int _curEquipIndex;

    private PlayerController _controller;
    private PlayerConditions _condition;

    [Header("Events")]
    public UnityEvent OnOpenInventory;
    public UnityEvent OnCloseInventory;

    public static Inventory Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }

        _controller = GetComponent<PlayerController>();
        _condition = GetComponent<PlayerConditions>();
    }
    private void Start()
    {
        InventoryWindow.SetActive(false);
        Slots = new ItemSlot[UiSlots.Length];

        for (int i = 0; i < Slots.Length; i++)
        {
            Slots[i] = new ItemSlot();
            UiSlots[i].index = i;
            UiSlots[i].Clear();
        }

        ClearSeletecItemWindow();
    }

    public void OnInventoryButton(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.phase == InputActionPhase.Started)
        {
            Toggle();
        }
    }


    public void Toggle()
    {
        if (InventoryWindow.activeInHierarchy)
        {
            InventoryWindow.SetActive(false);
            OnCloseInventory?.Invoke();
            _controller.ToggleCursor(false);
        }
        else
        {
            InventoryWindow.SetActive(true);
            OnOpenInventory?.Invoke();
            _controller.ToggleCursor(true);
        }
    }

    public bool IsOpen()
    {
        return InventoryWindow.activeInHierarchy;
    }

    public void AddItem(ItemData item)
    {
        if (item.canStack)
        {
            ItemSlot slotToStackTo = GetItemStack(item);
            if (slotToStackTo != null)
            {
                slotToStackTo.Quantity++;
                UpdateUI();
                return;
            }
        }

        ItemSlot emptySlot = GetEmptySlot();

        if (emptySlot != null)
        {
            emptySlot.Item = item;
            emptySlot.Quantity = 1;
            UpdateUI();
            return;
        }

        ThrowItem(item);
    }

    void ThrowItem(ItemData item)
    {
        Instantiate(item.dropPrefab, DropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360f));
    }

    void UpdateUI()
    {
        for (int i = 0; i< Slots.Length; i++)
        {
            if (Slots[i].Item != null)
                UiSlots[i].Set(Slots[i]);
            else
                UiSlots[i].Clear();
        }
    }

    ItemSlot GetItemStack(ItemData item)
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            if (Slots[i].Item == item && Slots[i].Quantity < item.maxStackAmount)
                return Slots[i];
        }

        return null;
    }

    ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            if (Slots[i].Item == null)
                return Slots[i];
        }

        return null;
    }

    public void SelectItem(int index)
    {
        if (Slots[index].Item == null)
            return;

        _selectedItem = Slots[index];
        _selectedItemIndex = index;

        SelectedItemName.text = _selectedItem.Item.displayName;
        SelectedItemDescription.text = _selectedItem.Item.description;

        SelectedItemStatNames.text = string.Empty;
        SelectedItemStatValues.text = string.Empty;

        for (int i = 0; i< _selectedItem.Item.consumables.Length; i++)
        {
            SelectedItemStatNames.text += _selectedItem.Item.consumables[i].type.ToString() + "\n";
            SelectedItemStatValues.text += _selectedItem.Item.consumables[i].value.ToString() + "\n";
        }

        UseButton.SetActive(_selectedItem.Item.type == ItemType.Consumable);
        EquipButton.SetActive(_selectedItem.Item.type == ItemType.Equipable && !UiSlots[index].equipped);
        UnEquipButton.SetActive(_selectedItem.Item.type == ItemType.Equipable && UiSlots[index].equipped);
        DropButton.SetActive(true);
    }

    private void ClearSeletecItemWindow()
    {
        _selectedItem = null;
        SelectedItemName.text = string.Empty;
        SelectedItemDescription.text = string.Empty;

        SelectedItemStatNames.text = string.Empty;
        SelectedItemStatValues.text = string.Empty;

        UseButton.SetActive(false);
        EquipButton.SetActive(false);
        UnEquipButton.SetActive(false);
        DropButton.SetActive(false);
    }

    public void OnUseButton()
    {
        if (_selectedItem.Item.type == ItemType.Consumable)
        {
            for (int i = 0; i < _selectedItem.Item.consumables.Length; i++)
            {
                switch (_selectedItem.Item.consumables[i].type)
                {
                    case ConsumableType.Health:
                        _condition.Heal(_selectedItem.Item.consumables[i].value); break;
                    case ConsumableType.Hunger:
                        _condition.Eat(_selectedItem.Item.consumables[i].value); break;
                }
            }
        }
        RemoveSelectedItem();
    }

    public void OnEquipButton()
    {
        Debug.Log("아이템 장착 버튼");
    }

    void UnEquip(int index)
    {
        Debug.Log("아이템 장착 해제");
    }

    public void OnUnEquipButton()
    {
        Debug.Log("아이템 장착 해제 버튼");
    }

    public void OnDropButton()
    {
        ThrowItem(_selectedItem.Item);
        RemoveSelectedItem();
    }

    private void RemoveSelectedItem()
    {
        _selectedItem.Quantity--;

        if (_selectedItem.Quantity <= 0)
        {
            if (UiSlots[_selectedItemIndex].equipped)
            {
                UnEquip(_selectedItemIndex);
            }

            _selectedItem.Item = null;
            ClearSeletecItemWindow();
        }

        UpdateUI();
    }

    public void RemoveItem(ItemData item)
    {
        Debug.Log("아이템 제거");
    }

    public bool HasItems(ItemData item, int quantity)
    {
        Debug.Log("아이템 없음");
        return false;
    }
}
