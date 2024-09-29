using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class NPCPatrol : MonoBehaviour
{
    public float patrolSpeed = 0.5f; // 巡逻时的速度
    public List<GameObject> roomBounds; // NPC所在房间的多个BoxCollider
    private NavMeshAgent agent;
    private List<BoxCollider> roomColliders; // 缓存房间的BoxCollider列表

    private const float minPatrolDistance = 1f; // 巡逻点的最小距离

    // Raycast 相关参数
    public float detectionDistance = 2f; // 检测障碍物的距离
    public float rayOffsetAngle = 30f;   // 左右偏移的检测角度


    void Start()
    {

        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;

        // 缓存房间的BoxCollider
        roomColliders = new List<BoxCollider>();
        Debug.Log(roomBounds.Count);
        foreach (var room in roomBounds)
        {
            BoxCollider collider = room.GetComponent<BoxCollider>();
            if (collider != null)
            {
                roomColliders.Add(collider);
            }
        }

        // 设置第一个巡逻目标
        SetNewPatrolTarget();
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            SetNewPatrolTarget();
        }

        // 检测前方是否有障碍物
        if (IsObstacleAhead())
        {
            SetNewPatrolTarget();
        }
    }

    // 设置新的巡逻目标点
    void SetNewPatrolTarget()
    {
        if (roomColliders.Count == 0) return;

        Vector3 randomPoint;
        bool validPointFound = false;

        while (!validPointFound)
        {
            randomPoint = GetRandomPointInRoom();

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                randomPoint = hit.position;

                if (Vector3.Distance(transform.position, randomPoint) > minPatrolDistance)
                {
                    validPointFound = true;
                    agent.destination = randomPoint;
                }
            }
        }
    }

    // 在房间内生成一个随机点 (只考虑X和Z坐标)
    Vector3 GetRandomPointInRoom()
    {
        BoxCollider selectedCollider = roomColliders[Random.Range(0, roomColliders.Count)];

        Vector3 minBounds = selectedCollider.bounds.min;
        Vector3 maxBounds = selectedCollider.bounds.max;

        float randomX = Random.Range(minBounds.x, maxBounds.x);
        float randomZ = Random.Range(minBounds.z, maxBounds.z);

        // 使用当前对象的Y坐标，确保NPC保持在相同的高度
        return new Vector3(randomX, transform.position.y, randomZ);
    }

    // 检测多个方向是否有障碍物
    private bool IsObstacleAhead()
    {
        // 中心前方
        if (RaycastInDirection(transform.forward)) return true;

        // 左前方
        Vector3 leftForward = Quaternion.Euler(0, -rayOffsetAngle, 0) * transform.forward;
        if (RaycastInDirection(leftForward)) return true;

        // 右前方
        Vector3 rightForward = Quaternion.Euler(0, rayOffsetAngle, 0) * transform.forward;
        if (RaycastInDirection(rightForward)) return true;

        return false;
    }

    // 使用Raycast检测给定方向上的障碍物
    private bool RaycastInDirection(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, detectionDistance))
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                return true;
            }
        }
        return false;
    }

    // 碰撞检测并强制改变方向
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            // 停止当前的移动
            agent.isStopped = true;

            // 重新设置新的巡逻目标点
            SetNewPatrolTarget();

            // 在下一帧恢复移动
            agent.isStopped = false;
        }
    }
}