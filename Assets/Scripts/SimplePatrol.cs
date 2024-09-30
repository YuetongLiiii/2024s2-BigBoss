using UnityEngine;

public class SimplePatrol : MonoBehaviour
{
    public Transform pointA; // 巡逻点A
    public Transform pointB; // 巡逻点B
    public float speed = 2.0f; // 移动速度
    public Animator animator;  // 引用Animator组件
    public LayerMask groundLayer; // 地面图层

    private Vector3 target; // 当前的目标位置

    void Start()
    {
        // 将巡逻点A和B的Y轴调整到地面高度
        pointA.position = new Vector3(pointA.position.x, GetGroundHeight(pointA.position), pointA.position.z);
        pointB.position = new Vector3(pointB.position.x, GetGroundHeight(pointB.position), pointB.position.z);

        // 初始目标位置为点A
        target = pointA.position;
    }
    void CreateFX_GC_L()
    {
        Transform leftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
        Vector3 offset = new Vector3(0, 0.1f, 0);
        GameObject fx = Instantiate(Resources.Load<GameObject>("FX_GroundCrack"), leftFoot.position+offset, Quaternion.identity);
        fx.transform.localRotation=Quaternion.Euler(90,0,0);
        GameObject.Destroy(fx, 2.0f);
        Debug.Log("CreateFX_GC_L");
    }
    void CreateFX_GC_R()
    {
        Transform rightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot);
        Vector3 offset = new Vector3(0, 0.1f, 0);
        GameObject fx = Instantiate(Resources.Load<GameObject>("FX_GroundCrack"), rightFoot.position+offset, Quaternion.identity);
        fx.transform.localRotation=Quaternion.Euler(90,0,0);
        GameObject.Destroy(fx, 2.0f);
        Debug.Log("CreateFX_GC_R");
    }
    void Update()
    {
        // 移动怪物到目标点
        Vector3 newPosition = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // 调整怪物的 Y 轴位置，使其始终贴地
        newPosition.y = GetGroundHeight(newPosition);

        // 更新怪物位置
        transform.position = newPosition;

        // 计算当前的速度，用于控制动画
        float movementSpeed = Vector3.Distance(transform.position, target);

        // 如果怪物在移动，设置动画的 Speed 参数为非零值
        animator.SetFloat("Speed", movementSpeed);

        // 如果到达目标点，切换到另一个目标
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            // 切换目标点
            target = (target == pointA.position) ? pointB.position : pointA.position;
        }

        // 调整朝向：计算怪物面向目标点的方向
        Vector3 direction = target - transform.position;

        // 保持Y轴不变，只旋转水平朝向
        direction.y = 0; // 确保不在Y轴旋转

        if (direction != Vector3.zero)
        {
            // 让怪物朝向目标点，使用平滑旋转
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f); // 平滑旋转
        }
    }

    // 使用射线检测获取地面的高度
    float GetGroundHeight(Vector3 position)
    {
        RaycastHit hit;
        // 从位置向下发射射线，检测地面碰撞
        if (Physics.Raycast(position + Vector3.up * 1.0f, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            return hit.point.y; // 返回检测到的地面高度
        }
        return position.y; // 如果没有检测到地面，则保持当前高度
    }
}
