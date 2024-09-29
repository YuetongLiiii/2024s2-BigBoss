using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorController : MonoBehaviour
{
    public float openAngle = 90; // 欧拉角
    public float closeAngle = 0;
    public float smoothSpeed = 2; // 运动时候的平滑速度

    private Quaternion openRotation; // 开门的角度
    private Quaternion closeRotation; // 关门的角度 四元角
    public bool isOpen; // 门当前开关状态
    private SphereCollider sc;
    private BoxCollider mc;

    void Start()
    {
        openRotation = Quaternion.Euler(0, openAngle, 0);
        closeRotation = Quaternion.Euler(0, closeAngle, 0);
        // 添加碰撞器并设置为触发器  
        sc = gameObject.AddComponent<SphereCollider>();
        sc.isTrigger = true;
        gameObject.tag = "Door";
        if (sc.radius <= 1f || sc.radius >= 1.2f)
        {
            sc.radius = 1.2f;
        }

        mc = GetComponentInChildren<BoxCollider>();
        if (mc == null)
        {
            Debug.LogError("No MeshCollider found on the door or its children!");
        }
    }
    
    void Update()
    {
        if (isOpen && Quaternion.Angle(transform.localRotation, openRotation) > 0.01f)
        {
            // 当前值与目标值之间的差值，每次旋转一点，最终旋转到目标值
            transform.localRotation = Quaternion.Slerp(transform.localRotation, openRotation, smoothSpeed * Time.deltaTime);
        }
        else if (!isOpen && Quaternion.Angle(transform.localRotation, closeRotation) > 0.01f)
        {
            // 当前值与目标值之间的差值，每次旋转一点，最终旋转到目标值
            transform.localRotation = Quaternion.Slerp(transform.localRotation, closeRotation, smoothSpeed * Time.deltaTime);
        }
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
        if (mc != null)
        {
            mc.enabled = !isOpen;
        }
        else
        {
            Debug.LogWarning("MeshCollider is missing, cannot toggle its enabled state.");
        }
    }
}

