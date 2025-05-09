using UnityEngine;
using System.Collections.Generic;

public class ObjectLoader : MonoBehaviour
{
    public Camera cam;
    public int buffer = 5;

    [Tooltip("Ch? l?y object có tag này (?? tr?ng s? l?y t?t c?)")]
    public List<string> tagFilters = new List<string>();

    [HideInInspector]
    public List<GameObject> allObjects = new();
    private Dictionary<Vector2Int, List<GameObject>> objectsByCell = new();

    private Vector2Int lastMin, lastMax;
    private int cellSize = 1;

    public void ScanAndBuildGrid()
    {
        objectsByCell.Clear();

        foreach (var obj in allObjects)
        {
            Vector2Int cell = WorldToCell(obj.transform.position);
            if (!objectsByCell.ContainsKey(cell))
                objectsByCell[cell] = new List<GameObject>();

            objectsByCell[cell].Add(obj);
            obj.SetActive(false);
        }
    }

    void Start()
    {
        ScanAndBuildGrid();
    }

    void Update()
    {
        Vector3 min = cam.ViewportToWorldPoint(new Vector3(0, 0));
        Vector3 max = cam.ViewportToWorldPoint(new Vector3(1, 1));

        Vector2Int minCell = WorldToCell(min) - Vector2Int.one * buffer;
        Vector2Int maxCell = WorldToCell(max) + Vector2Int.one * buffer;

        if (minCell != lastMin || maxCell != lastMax)
        {
            HashSet<GameObject> visible = new();

            for (int x = minCell.x; x <= maxCell.x; x++)
            {
                for (int y = minCell.y; y <= maxCell.y; y++)
                {
                    Vector2Int cell = new(x, y);
                    if (objectsByCell.TryGetValue(cell, out var objList))
                    {
                        foreach (var obj in objList)
                        {
                            obj.SetActive(true);
                            visible.Add(obj);
                        }
                    }
                }
            }

            foreach (var kv in objectsByCell)
            {
                foreach (var obj in kv.Value)
                {
                    if (!visible.Contains(obj))
                        obj.SetActive(false);
                }
            }

            lastMin = minCell;
            lastMax = maxCell;
        }
    }

    Vector2Int WorldToCell(Vector3 pos)
    {
        return new Vector2Int(Mathf.FloorToInt(pos.x / cellSize), Mathf.FloorToInt(pos.y / cellSize));
    }
}
