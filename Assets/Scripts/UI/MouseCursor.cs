using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseCursor : Singleton<MouseCursor>
{
    public Image rend;
    public Sprite sprite;

    void Start()
    {
        Cursor.visible = false;
        rend = GetComponent<Image>();
    }

    void Update()
    {
        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = cursorPos;
    }
}