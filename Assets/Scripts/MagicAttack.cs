using UnityEngine;

public class MagicAttack : MonoBehaviour
{
    public int damage = 20;  // 魔法攻击的伤害
    public GameObject bloodEffectPrefab;  // 溅血效果预制件
    private Transform playerSpine;  // 玩家头部骨骼的位置
    private GameObject player;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        // 如果未在Inspector中设置playerSpine，则尝试自动获取
        if (playerSpine == null)
        {
            if (player != null)
            {
                // 尝试找到玩家的头部骨骼（根据实际的骨骼路径修改）
                playerSpine = player.transform.Find("Root/Hips/Spine_01/Spine_02");
                if (playerSpine == null)
                {
                    Debug.LogError("Player's head bone not found! Please set 'playerSpine' in the Inspector.");
                }
                else
                {
                    Debug.Log("Successfully found player's spine: " + playerSpine.name);
                    Debug.Log("Player spine position: " + playerSpine.position);
                }
            }
            else
            {
                Debug.LogError("Player not found! Please ensure the player has the 'Player' tag.");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 对玩家造成伤害
            HealthManager playerHealth = other.GetComponent<HealthManager>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            // 播放溅血效果
            PlayBloodEffectAtSpine();

            // 销毁魔法攻击特效
            Destroy(gameObject);
        }
    }

    // 播放溅血粒子效果在玩家的头部位置
    void PlayBloodEffectAtSpine()
    {
        if (bloodEffectPrefab != null && playerSpine != null)
        {
            // 使用头部骨骼位置生成溅血效果
            Vector3 hitPoint = playerSpine.position;

            Debug.Log("Playing blood effect at player's head position: " + hitPoint);
            
            // 在头部生成溅血效果
            GameObject bloodEffect = Instantiate(bloodEffectPrefab, hitPoint, Quaternion.identity);

            // 将溅血效果设为玩家的子对象
            // bloodEffect.transform.SetParent(player.transform);

            // 使溅血效果朝向外部（即从击中点向NPC外溅）
            Vector3 directionToNPC = (transform.position - hitPoint).normalized;
            bloodEffect.transform.rotation = Quaternion.LookRotation(directionToNPC); // 使粒子效果的方向朝向NPC

            // 2秒后销毁粒子系统
            Destroy(bloodEffect, 2.0f);
        }
        else
        {
            Debug.LogError("Blood effect prefab or player head reference is missing!");
        }
    }
}
