using UnityEngine;

public class ItemCollect : MonoBehaviour
{
    public GameObject player; // 玩家对象
    public string itemName; // 物品的名称
    public float pickupDistance = 2.0f; // 允许拾取的距离

    private void Update()
    {
        // 计算玩家与物品之间的距离
        float distance = Vector3.Distance(player.transform.position, transform.position);

        // 检查玩家是否在拾取范围内
        if (distance <= pickupDistance)
        {
            Debug.Log("You can pick up " + itemName + " by pressing Q.");

            // 如果玩家按下 Q 键，尝试拾取物品
            if (Input.GetKeyDown(KeyCode.Q))
            {
                PlayerPack pack = player.GetComponent<PlayerPack>();
                if (pack != null)
                {
                    if (!pack.IsFull())
                    {
                        // 将物品添加到玩家的背包
                        pack.AddItem(itemName);
                        Destroy(gameObject); // 拾取后销毁物品
                    }
                    else
                    {
                        Debug.Log("Cannot pick up " + itemName + ". pack is full.");
                    }
                }
            }
        }
    }
}
