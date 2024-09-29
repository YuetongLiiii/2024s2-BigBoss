using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraMove : MonoBehaviour
{

    public float sensitivityMouse = 40f;
    //旋转大小
    public float rotateSpeed = 0.1f;

    public float mX = 0.0F;
    public float mY = 35F;
    //角度限制  
    private float MinLimitY = -50;
    private float MaxLimitY = 85;

    //旋转速度
    public CinemachineVirtualCamera virtualCamera;

    [HideInInspector]public CinemachinePOV cinemachinePov;


    // Use this for initialization
    void Start()
    {
        cinemachinePov = virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        cinemachinePov.m_HorizontalAxis.m_InputAxisName = "";
        cinemachinePov.m_VerticalAxis.m_InputAxisName = "";
        //RefreshCamera();
        RefreshCamera();

        // 隐藏并锁定鼠标光标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;  // 隐藏光标
    }
    


    // Update is called once per frame
    void Update()
    {
        RefreshCamera();
    }

    void RefreshCamera()
    {
        //获取鼠标输入  
        mX += Input.GetAxis("Mouse X") * sensitivityMouse * rotateSpeed;
        mY -= Input.GetAxis("Mouse Y") * sensitivityMouse * rotateSpeed;
        cinemachinePov.m_HorizontalAxis.Value = mX;
        cinemachinePov.m_VerticalAxis.Value = mY;
    }


}
