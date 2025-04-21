using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CraftingSystemManager : Singleton<CraftingSystemManager>
{
    public List<GameObject> listOutputSlot;
    public GameObject outputSlot;
    public List<Recipe> recipes;
    public GameObject itemDropPrefab;

    private UI_InventoryItem[,] grid = new UI_InventoryItem[3, 3];

    private void OnEnable()
    {
        UI_InventoryItem.OnCraftingSlotAdded += AddItemToGrid;
        UI_InventoryItem.ItemOnDrag += RemoveItemToGrid;
    }

    private void OnDisable()
    {
        UI_InventoryItem.OnCraftingSlotAdded -= AddItemToGrid;
        UI_InventoryItem.ItemOnDrag -= RemoveItemToGrid;
    }

    public void AddItemToGrid(int i, int j, UI_InventoryItem item)
    {
        grid[i, j] = item;
        CheckRecipe();
    }

    public void RemoveItemToGrid(int i, int j, Item item)
    {
        grid[i, j] = null;
        UI_InventoryItem uI_InventoryItem = outputSlot.GetComponentInChildren<UI_InventoryItem>();
        if (uI_InventoryItem != null) Destroy(uI_InventoryItem.gameObject);
    }

    public void CheckRecipe()
    {
        if (grid == null) return;

        foreach (Recipe recipe in recipes)
        {
            bool completeRecipe = true;

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    var slotItem = grid[i, j];
                    var recipeItem = recipe.GetItem(i, j);

                    if ((slotItem == null && recipeItem != null) ||
                        (slotItem != null && recipeItem == null) ||
                        (slotItem != null && recipeItem != null &&
                        slotItem.InventoryItem.Item.itemName != recipeItem.itemName))
                    {
                        completeRecipe = false;
                        break;
                    }
                }

                if (!completeRecipe) break;
            }

            if (completeRecipe)
            {
                CreateItem(recipe.itemOutput);
                return;
            }
        }
    }

    public void CreateItem(Item item)
    {
        if (item == null) return;
       
        if (outputSlot.transform.childCount > 0) return;

        UI_InventorySlot slot = outputSlot.GetComponent<UI_InventorySlot>();
        InventoryItem inventoryItem = new InventoryItem(System.Guid.NewGuid().ToString(), item, slot.slotIndex);
        GameObject newItem = Instantiate(itemDropPrefab, outputSlot.transform);
        UI_InventoryItem inventoryItemUI = newItem.GetComponent<UI_InventoryItem>();
        inventoryItemUI.InitialiseItem(inventoryItem);
        inventoryItemUI.IsItemCreated(true);
    }

    public void TakeOffItem()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j] != null)
                {
                    if (grid[i, j].InventoryItem.Quantity > 1)
                    {
                        grid[i, j].InventoryItem.DecreaseQuantity(1);
                        grid[i, j].RefreshCount();
                    }
                    else if (grid[i, j].InventoryItem.Quantity == 1)
                    {
                        InventoryManager.Instance.RemoveItemById(grid[i, j].InventoryItem);
                        Destroy(grid[i, j].gameObject);
                    }
                }
                else continue;
            }
        }
    }

    public void SetNumCraftingTable(int num)
    {
        this.outputSlot = listOutputSlot[num];
    }
}