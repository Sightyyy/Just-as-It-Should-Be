using UnityEngine;

public sealed class GameCursorController : MonoBehaviour
{
    private void Awake()
    {
        HideCursor();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            HideCursor();
        }
    }

    private void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private static void HideCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
    }
}
