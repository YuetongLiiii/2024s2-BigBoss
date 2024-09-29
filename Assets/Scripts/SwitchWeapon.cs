using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchWeapon : MonoBehaviour
{
    public RuntimeAnimatorController fistController;  // 打拳的动画控制器
    public RuntimeAnimatorController axeController;   // 拿斧头的动画控制器
    public GameObject axe;  // 斧头模型

    private Animator animator;  // 用于引用 Animator 组件

    void Start()
    {
        animator = GetComponent<Animator>();  // 获取 Animator 组件
        SwitchToFist();  // 默认开始使用拳头
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))  // 检测按键是否被按下
        {
            if (animator.runtimeAnimatorController == fistController)
            {
                SwitchToAxe();  // 切换到斧头
            }
            else
            {
                SwitchToFist();  // 切换到拳头
            }
        }
    }

    void SwitchToFist()
    {
        animator.runtimeAnimatorController = fistController;  // 设置为打拳的控制器
        axe.SetActive(false);  // 隐藏斧头模型
    }

    void SwitchToAxe()
    {
        animator.runtimeAnimatorController = axeController;  // 设置为拿斧头的控制器
        axe.SetActive(true);  // 显示斧头模型
    }
}
