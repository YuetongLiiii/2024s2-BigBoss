using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    public Transform camera;

    void Start()
    {
        // 如果没有手动设置摄像机，则自动查找主摄像机
        if (camera == null)
        {
            // 获取场景中的主摄像机（包括 Cinemachine 控制的摄像机）
            camera = Camera.main.transform;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // 保证物体面向摄像机的方向
        if (camera != null)
        {
            transform.LookAt(camera.position + camera.forward);
        }
    }
}
