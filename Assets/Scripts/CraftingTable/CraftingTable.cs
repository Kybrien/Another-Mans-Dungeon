using UnityEngine;

public class CraftingTable : MonoBehaviour
{
    public GameObject craftingUI; // Drag and drop your CraftingUI GameObject in the Inspector
    private bool isCraftingUIOpened = false;

    void Update()
    {
        // Check if the player presses the "E" key and is near the crafting table
        if (Input.GetKeyDown(KeyCode.E) && IsPlayerNear())
        {
            ToggleCraftingUI();
        }
    }

    private bool IsPlayerNear()
    {
        // Add logic here to check if the player is near the crafting table.
        // This could be a simple distance check or a more complex system.
        // For simplicity, we assume the player is always near.
        return true;
    }

    private void ToggleCraftingUI()
    {
        isCraftingUIOpened = !isCraftingUIOpened;
        craftingUI.SetActive(isCraftingUIOpened);

        // Optionally, lock/unlock the cursor and pause/unpause the game
        if (isCraftingUIOpened)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f; // Pause the game
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f; // Unpause the game
        }
    }
}
