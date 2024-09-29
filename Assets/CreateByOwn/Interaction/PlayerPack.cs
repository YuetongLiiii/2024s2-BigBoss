using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPack : MonoBehaviour
{
    public GameObject packUI; // 背包 UI 面板
    public int capacity = 6; // 背包容量
    public List<string> items; // 用来存储物品的列表
    public List<GameObject> slots; // 背包的格子（Slot）列表，每个格子应该包含一个 Image 组件
    public Dictionary<string, Sprite> itemSprites; // 用来存储物品名称与图片的映射

    void Start()
    {
        items = new List<string>(capacity); // 初始化背包
        packUI.SetActive(false); // 初始化时隐藏 UI
        itemSprites = new Dictionary<string, Sprite>();
    }
    void Update()
    {
        // 检查是否按下 Z 键打开/关闭背包 UI
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TogglePackUI();
        }
    }

    // 添加物品到背包
    public bool AddItem(string item)
    {
        if (items.Count < capacity)
        {
            items.Add(item);
            Debug.Log("Added " + item + " to pack.");
            itemSprites[item] = Resources.Load<Sprite>("Item Images/" + item);
            if (itemSprites[item] == null)
            {
                Debug.LogWarning("No sprite found for item: " + item);
            }
            UpdatePackUI(item);
            return true;
        }
        else
        {
            Debug.Log("pack is full!");
            return false;
        }
    }
    void TogglePackUI()
    {
        bool isActive = packUI.activeSelf;
        packUI.SetActive(!isActive);
    }

    // 更新 UI 中显示的物品
    void UpdatePackUI(string item)
    {
        // 查找该 item 对应的图片
        if (itemSprites.ContainsKey(item))
        {
            // 遍历 slots 列表，找到第一个空的格子（没有图片的格子）
            foreach (GameObject slot in slots)
            {
                Image slotImage = slot.GetComponent<Image>();

                // 如果该 slot 的 Image 还没有图片，说明是空的
                if (slotImage.sprite == null)
                {
                    Color newColor = slotImage.color;
                    newColor.a = 1f; // 设置透明度为 1
                    slotImage.color = newColor;
                    slotImage.sprite = itemSprites[item]; // 插入 item 对应的图片
                    Debug.Log("Inserted " + item + " into slot.");
                    break; // 插入后跳出循环
                }
            }
        }
        else
        {
            Debug.LogWarning("No sprite found for item: " + item);
        }
    }

    // 检查背包是否已满
    public bool IsFull()
    {
        return items.Count >= capacity;
    }
}
