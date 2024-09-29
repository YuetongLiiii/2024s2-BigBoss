using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class start : MonoBehaviour
{
    public void LoadGameScene()
    {
        SceneManager.LoadScene("GameScene"); // 确保场景名称与你的游戏场景名称匹配
    }
}