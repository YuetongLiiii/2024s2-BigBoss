using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPackUI : MonoBehaviour
{
    public PlayerPack playerPack; // 引用 PlayerPack 脚本
    public GameObject packUI;     // 背包 UI 面板

    void Start()
    {
        packUI.SetActive(false); // 初始化时隐藏 UI
    }

    void Update()
    {
        // 检查是否按下 Z 键打开/关闭背包 UI
        if (Input.GetKeyDown(KeyCode.Z))
        {
            TogglePackUI();
        }
    }

    // 打开/关闭背包 UI 并更新内容
    void TogglePackUI()
    {
        bool isActive = packUI.activeSelf;
        packUI.SetActive(!isActive);

        if (!isActive) // 如果背包界面被打开，更新UI
        {
            UpdatePackUI();
        }
    }

    // 更新 UI 中显示的物品
    void UpdatePackUI()
    {
        Debug.Log("Updating pack UI...");

        // 根据 PlayerPack 中的物品更新显示
        for (int i = 0; i < playerPack.items.Count; i++)
        {
            Debug.Log("Updating item " + i + ": " + playerPack.items[i]);
        }
    }
}
