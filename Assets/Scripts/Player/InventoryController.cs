using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private InventoryManagerSO _inventoryManagerSO;

    // GameEvent
    [SerializeField] private GameEvent onOpenInventory;
    [SerializeField] private GameEvent onCloseInventory;
    [SerializeField] private GameEvent onChangeSelectedSlot;
    [SerializeField] private GameEvent onAddItemToInventory;
    [SerializeField] private GameEvent onInventoryLoad;
    private void OnEnable()
    {
        _inputReader.playerActions.changeInventorySlotEvent += GetInputValueToChangeSlot;
        _inputReader.playerActions.openInventoryEvent += OpenInventory;
        _inputReader.uiActions.closeInventoryEvent += CloseInventory;
    }

    private void OnDisable()
    {
        _inputReader.playerActions.changeInventorySlotEvent -= GetInputValueToChangeSlot;
        _inputReader.playerActions.openInventoryEvent -= OpenInventory;
        _inputReader.uiActions.closeInventoryEvent -= CloseInventory;
    }

    private void OpenInventory()
    {
        onOpenInventory.Raise(this, ActionMap.UI);
    }
    private void CloseInventory()
    {
        onCloseInventory.Raise(this, ActionMap.Player);
    }

    public void SwitchActionMap(Component sender, object data)
    {
        ActionMap map = (ActionMap)data;
        _inputReader.SwitchActionMap(map);
    }
    private void GetInputValueToChangeSlot(int value, bool isKeyboard)
    {

        if (isKeyboard)
        {

            if (value != _inventoryManagerSO.selectedSlot)
            {
                onChangeSelectedSlot.Raise(this, value); // gui duy nhat newValue thoi
                
            }
            
        }
        else
        {
            int newValue = _inventoryManagerSO.selectedSlot + value;
            if (newValue > 8) newValue = 0;
            else if (newValue < 0) newValue = 8;
            
            onChangeSelectedSlot.Raise(this, newValue);
        }
    }

    public void AddItemWorldToInventory(ItemWorldControl itemWorldControl)
    {
        ItemWorld item = itemWorldControl.GetItemWorld();
        InventoryItem inventoryItem = new(item.Id, item.Item, 0, item.Quantity);
        _inventoryManagerSO.middleInventoryItem = inventoryItem;
        onAddItemToInventory.Raise(itemWorldControl, null);
    }



    public void StartToLoad(GameData gameData)
    {
        Debug.Log("load");
        _inventoryManagerSO.inventory = gameData.InventoryData;
        ItemDatabase.Instance.SetItem(_inventoryManagerSO.inventory.InventoryItemList);
        onInventoryLoad.Raise(this, null);
        _inventoryManagerSO.RefreshCurrentHoldingItem();
    }
    public void StartToSave(ref GameData gameData)
    {
        gameData.SetInventoryData(_inventoryManagerSO.inventory);
    }

}
