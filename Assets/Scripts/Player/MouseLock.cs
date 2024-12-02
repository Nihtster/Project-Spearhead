using UnityEngine;

public class MouseLock : MonoBehaviour
{
    public void Start()
    {
        LockCursor();
    }

    public void Update()
    {
        // Example: Unlock the cursor when pressing the Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnlockCursor();
        }
        else if (Input.GetMouseButtonDown(0)) // Relock cursor if left mouse button is clicked
        {
            LockCursor();
        }
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked; // Locks the cursor to the center of the screen
        Cursor.visible = false;                  // Hides the cursor
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;  // Frees the cursor
        Cursor.visible = true;                   // Shows the cursor
    }
}
