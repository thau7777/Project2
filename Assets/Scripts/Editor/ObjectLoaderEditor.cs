using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(ObjectLoader))]
public class ObjectLoaderEditor : Editor
{
    private string[] allTags;
    private Dictionary<string, bool> tagToggles = new();

    public override void OnInspectorGUI()
    {
        ObjectLoader loader = (ObjectLoader)target;

        allTags = UnityEditorInternal.InternalEditorUtility.tags;

        if (tagToggles.Count == 0)
        {
            foreach (string tag in allTags)
                tagToggles[tag] = loader.tagFilters.Contains(tag);
        }

        loader.cam = (Camera)EditorGUILayout.ObjectField("Camera", loader.cam, typeof(Camera), true);
        loader.buffer = EditorGUILayout.IntField("Buffer", loader.buffer);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Choose Tag to scan:", EditorStyles.boldLabel);

        foreach (var tag in allTags)
        {
            tagToggles[tag] = EditorGUILayout.ToggleLeft(tag, tagToggles[tag]);
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Scan Object in Scene"))
        {
            loader.tagFilters.Clear();
            foreach (var tag in tagToggles)
            {
                if (tag.Value)
                    loader.tagFilters.Add(tag.Key);
            }

            ScanObjectsByTag(loader);
            loader.ScanAndBuildGrid();
            EditorUtility.SetDirty(loader);
        }

        EditorGUILayout.LabelField("Object scanned: " + loader.allObjects.Count);
    }

    void ScanObjectsByTag(ObjectLoader loader)
    {
        loader.allObjects.Clear();
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (!obj.scene.IsValid()) continue;
            if (obj == loader.gameObject) continue;
            if (obj.GetComponent<ObjectLoader>() != null) continue;
            if (!loader.tagFilters.Contains(obj.tag)) continue;

            loader.allObjects.Add(obj);
        }

        Debug.Log($"Scanned {loader.allObjects.Count} object by {loader.tagFilters.Count} tag.");
    }
}
