using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFire : MonoBehaviour
{
    public Transform player;
    private Animator animator; // 敌人的动画控制器
    public float attackDistance = 1.0f;
    public GameObject Effect;
    public Transform fire;
    public float fireRate = 1.03f; // 发射频率
    private float nextFireTime = 0f; // 下次发射子弹的时间

    public float detectionRadius = 3f; // 检测范围
    public float viewAngle = 120f; // 敌人的可见角度
    private bool isPlayerInSight = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        DetectPlayerInView();

        float distance = Vector3.Distance(transform.position, player.position);
        
        if (isPlayerInSight && distance <= attackDistance)
        {
            animator.SetBool("IsAttack", true);
            if (Time.time >= nextFireTime)
            {
                Instantiate(Effect, fire.position, transform.rotation);
                nextFireTime = Time.time + fireRate;
            }
        }
        else
        {
            animator.SetBool("IsAttack", false);
        }
    }

    void DetectPlayerInView()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= detectionRadius)
        {
            Vector3 forward = transform.forward;
            float angleToPlayer = Vector3.Angle(forward, directionToPlayer);

            if (angleToPlayer <= viewAngle / 2f) // 检查玩家是否在视角范围内
            {
                isPlayerInSight = true;
                return;
            }
        }
        isPlayerInSight = false;
    }
}
