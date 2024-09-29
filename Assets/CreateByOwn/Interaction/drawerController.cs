using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawerController : MonoBehaviour
{
    public Transform drawer; // 应该在Inspector中被分配
    public float openPositionY = 1.0f;
    public float closePositionY = 0.0f;
    public float smoothSpeed = 2.0f;
    private bool isOpen = false;

    void Update()
    {
        if (drawer == null)
        {
            Debug.LogError("Drawer is not assigned in the Inspector.");
            return;
        }

        Vector3 targetPosition = isOpen ? new Vector3(drawer.localPosition.x, openPositionY, drawer.localPosition.z) :
                                          new Vector3(drawer.localPosition.x, closePositionY, drawer.localPosition.z);
        drawer.localPosition = Vector3.Lerp(drawer.localPosition, targetPosition, smoothSpeed * Time.deltaTime);
    }

    public void ToggleDrawer()
    {
        isOpen = !isOpen;
    }
}
