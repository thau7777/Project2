using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.IO;

public class TilemapExporter : MonoBehaviour
{
    [MenuItem("Tools/Export Tilemap to MapLayerData")]
    public static void ExportSelectedTilemap()
    {
        Tilemap tilemap = Selection.activeGameObject?.GetComponent<Tilemap>();
        if (tilemap == null)
        {
            EditorUtility.DisplayDialog("L?i", "Vui l�ng ch?n m?t GameObject c� Tilemap trong Scene.", "OK");
            return;
        }

        string path = EditorUtility.SaveFilePanelInProject(
            "Save MapLayerData",
            tilemap.name + "_LayerData",
            "asset",
            "Ch?n n?i l?u ScriptableObject");

        if (string.IsNullOrEmpty(path))
            return;

        List<TileInfo> tileList = new List<TileInfo>();

        tilemap.CompressBounds();
        BoundsInt bounds = tilemap.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            if (tile != null)
            {
                tileList.Add(new TileInfo
                {
                    position = pos,
                    tile = tile
                });
            }
        }

        MapLayerData mapLayer = ScriptableObject.CreateInstance<MapLayerData>();
        mapLayer.layerName = tilemap.name;
        mapLayer.tiles = tileList.ToArray();

        AssetDatabase.CreateAsset(mapLayer, path);
        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("Ho�n t?t", "Export th�nh c�ng " + tileList.Count + " tiles.", "OK");
    }
}
