using UnityEngine;

public class AutoAttack : MonoBehaviour
{
    // 枚举表示 NPC 的攻击类型
    public enum NPCType { HandAttack, AxeAttack }
    public NPCType npcType;  // 当前NPC的攻击类型

    public Transform player;  // 玩家的Transform
    public Transform playerSpine;  // 玩家身体部位（例如头部或脊椎）
    public float handAttackDistance = 1.0f;  // 手攻击距离
    public float axeAttackDistance = 2.0f;  // 斧头攻击距离
    public float detectionRadius = 3f; // NPC的检测范围
    public float viewAngle = 120f; // NPC的可见角度
    public float attackCooldown = 2.0f; // 攻击冷却时间
    public int handDamage = 10; // 手攻击伤害
    public int axeDamage = 30; // 斧头攻击伤害
    public GameObject bloodEffectPrefab; // 溅血效果预制件

    private float attackTimer = 0f; // 攻击计时器
    private Animator animator; // 动画控制器
    private bool isAttacking = false; // 是否正在攻击
    private float currentAttackDistance;  // 当前攻击距离
    private int currentDamage;  // 当前攻击伤害

    void Start()
    {
        animator = GetComponent<Animator>(); // 获取动画控制器

        // 根据 NPC 类型设置攻击距离和伤害
        if (npcType == NPCType.HandAttack)
        {
            currentAttackDistance = handAttackDistance;
            currentDamage = handDamage;
        }
        else if (npcType == NPCType.AxeAttack)
        {
            currentAttackDistance = axeAttackDistance;
            currentDamage = axeDamage;
        }

        // 如果未在 Inspector 中设置 playerSpine，尝试自动找到玩家的脊椎部位
        if (playerSpine == null && player != null)
        {
            playerSpine = player.transform.Find("Root/Hips/Spine_01/Spine_02");
            if (playerSpine == null)
            {
                Debug.LogError("Player's spine not found! Please set 'playerSpine' in the Inspector.");
            }
        }
    }

    void Update()
    {
        attackTimer -= Time.deltaTime; // 更新攻击计时器

        if (IsPlayerInAttackRange())
        {
            if (attackTimer <= 0f)
            {
                AttackPlayer(); // 开始攻击动画
                attackTimer = attackCooldown; // 重置攻击计时器
            }
        }
        else
        {
            animator.SetBool("IsAttack", false); // 停止攻击动画
        }
    }

    // 检查玩家是否在攻击范围内
    bool IsPlayerInAttackRange()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= detectionRadius)
        {
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            if (angleToPlayer <= viewAngle / 2f)
            {
                if (distanceToPlayer <= currentAttackDistance)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // 播放攻击动画，具体伤害在动画事件触发
    void AttackPlayer()
    {
        animator.SetBool("IsAttack", true); // 播放攻击动画
        isAttacking = true; // 标记为攻击状态

        // HealthManager playerHealth = player.GetComponent<HealthManager>();
        // if (playerHealth != null)
        // {
        //     playerHealth.TakeDamage(handDamage); // 直接扣血
        //     PlayBloodEffect(); // 播放溅血效果
        // }

        if (npcType == NPCType.HandAttack)
        {
            // 如果是手攻击，直接对玩家造成伤害
            HealthManager playerHealth = player.GetComponent<HealthManager>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(handDamage); // 直接扣血
                PlayBloodEffect(); // 播放溅血效果
            }
        }
    }

    // 播放溅血效果在玩家的指定部位
    void PlayBloodEffect()
    {
        if (bloodEffectPrefab != null && playerSpine != null)
        {
            // 在玩家指定部位生成溅血效果
            Vector3 hitPointPosition = playerSpine.position;

            GameObject bloodEffect = Instantiate(bloodEffectPrefab, hitPointPosition, Quaternion.identity);
            Destroy(bloodEffect, 2.0f); // 在2秒后销毁效果
        }
        else
        {
            Debug.LogError("Blood effect prefab or player spine reference is missing!");
        }
    }

    // 斧头的触发器检测玩家是否被攻击到
    private void OnTriggerEnter(Collider other)
    {
        if (npcType == NPCType.AxeAttack && other.CompareTag("Player"))
        {
            // 只有斧头攻击使用碰撞检测
            HealthManager playerHealth = other.GetComponent<HealthManager>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(axeDamage); // 对玩家造成斧头伤害
                PlayBloodEffect(); // 播放溅血效果
            }
        }
    }
}
