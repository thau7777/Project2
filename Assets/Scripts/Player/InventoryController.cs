using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    //private Inventory inventory;
    //[SerializeField] private InputReader _inputReader;
    //[SerializeField] private GameEvent onChangeSelectedSlot;

    //private void OnEnable()
    //{
    //    _inputReader.OnInventorySlotChange += GetInputValueToChangeSlot;
    //}

    //private void GetInputValueToChangeSlot(int value, bool isKeyboard)
    //{

    //    if (isKeyboard)
    //    {
    //        if (value != selectedSlot)
    //        {
    //            InventoryManager.Instance.ChangeSelectedSlot(selectedSlot, value);
    //            selectedSlot = value;
    //        }
    //        //object data = new object({selectedSlot,value});
    //        // onChangeSelectedSlot.Raise(this,data);
    //    }
    //    else
    //    {
    //        int newValue = selectedSlot + value;
    //        if (newValue > 8) newValue = 0;
    //        else if (newValue < 0) newValue = 8;
    //        InventoryManager.Instance.ChangeSelectedSlot(selectedSlot, newValue);
    //        selectedSlot = newValue;
    //    }
    //}

}
