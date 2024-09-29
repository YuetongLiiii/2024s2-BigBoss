using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class NPCChase : MonoBehaviour
{
    public Transform player; // 玩家
    public float chaseSpeed = 3f; // 追逐时的速度
    public float patrolSpeed = 1f; // 巡逻时的速度
    public float detectionRadius = 3f; // 触发范围
    public float viewAngle = 120f; // NPC的可见角度
    public List<GameObject> roomBounds; // 包含玩家和NPC的房间边界，通常是多个BoxCollider
    private NavMeshAgent agent;
    private Animator animator; // 动画控制器
    private bool isChasing = false; // 是否在追逐状态

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); // 获取Animator组件
    }

    void Update()
    {
        DetectPlayerInView();

        if (isChasing && IsPlayerWithinBounds())
        {
            GetComponent<NPCPatrol>().enabled = false; // 停止巡逻
            agent.destination = player.position;
            agent.speed = chaseSpeed;
            animator.SetBool("IsChasing", true);
        }
        else
        {
            GetComponent<NPCPatrol>().enabled = true; // 重新开启巡逻
            agent.speed = patrolSpeed; // 将速度改为巡逻速度
            animator.SetBool("IsChasing", false);
            isChasing = false; // 确保停止追逐
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
                isChasing = true;
                return;
            }
            
        }
        isChasing = false;
    }

    bool IsPlayerWithinBounds()
    {
        foreach (GameObject room in roomBounds)
        {
            BoxCollider collider = room.GetComponent<BoxCollider>();
            if (collider != null)
            {
                Vector3 playerPosition = player.position;

                // 获取BoxCollider在X和Z轴上的最小和最大边界
                Vector3 minBounds = collider.bounds.min;
                Vector3 maxBounds = collider.bounds.max;

                // 检查玩家的X和Z坐标是否在这些边界内
                if (playerPosition.x >= minBounds.x && playerPosition.x <= maxBounds.x &&
                    playerPosition.z >= minBounds.z && playerPosition.z <= maxBounds.z)
                {
                    return true; // 玩家在这个房间的范围内
                }
            }
        }
        return false; // 玩家不在任何房间的XZ平面范围内
    }



    // 在编辑器中绘制检测区域的Gizmos
    // private void OnDrawGizmosSelected()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(transform.position, detectionRadius);

    //     Vector3 forward = transform.forward * detectionRadius;
    //     Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * forward;
    //     Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * forward;

    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
    //     Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
    //     Gizmos.DrawWireSphere(transform.position + forward, 0.1f);

    //     Gizmos.color = new Color(1, 1, 0, 0.2f);
    //     Gizmos.DrawMesh(CreateConeMesh(detectionRadius, viewAngle), transform.position, Quaternion.LookRotation(forward));
    // }

    // private Mesh CreateConeMesh(float radius, float angle)
    // {
    //     Mesh mesh = new Mesh();
    //     int segments = 20;
    //     float angleStep = angle / segments;

    //     Vector3[] vertices = new Vector3[segments + 2];
    //     int[] triangles = new int[segments * 3];

    //     vertices[0] = Vector3.zero;
    //     for (int i = 0; i <= segments; i++)
    //     {
    //         float currentAngle = -angle / 2 + angleStep * i;
    //         vertices[i + 1] = new Vector3(Mathf.Sin(currentAngle * Mathf.Deg2Rad), 0, Mathf.Cos(currentAngle * Mathf.Deg2Rad)) * radius;
    //     }

    //     for (int i = 0; i < segments; i++)
    //     {
    //         triangles[i * 3] = 0;
    //         triangles[i * 3 + 1] = i + 1;
    //         triangles[i * 3 + 2] = i + 2;
    //     }

    //     mesh.vertices = vertices;
    //     mesh.triangles = triangles;
    //     mesh.RecalculateNormals();

    //     return mesh;
    // }
}