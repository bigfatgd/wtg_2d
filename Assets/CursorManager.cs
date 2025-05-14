using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D clickCursor;
    [SerializeField] private Vector2 customHotspot = new Vector2(1260, 130);

    void Start()
    {
        // Set initial cursor
        Cursor.SetCursor(defaultCursor, customHotspot, CursorMode.ForceSoftware);
    }

    void Update()
    {
        // Change cursor while left mouse button is held
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.SetCursor(clickCursor, customHotspot, CursorMode.ForceSoftware);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Cursor.SetCursor(defaultCursor, customHotspot, CursorMode.ForceSoftware);
        }
    }
}