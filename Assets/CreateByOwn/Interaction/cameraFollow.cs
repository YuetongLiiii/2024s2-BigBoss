using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;            // 角色的Transform
    public Vector3 offset = new Vector3(0, 2, -5); // 相机相对于角色的偏移量
    public float smoothSpeed = 0.125f;  // 平滑速度
    public float minDistance = 2f;      // 相机与角色的最小距离
    public float maxDistance = 5f;      // 相机与角色的最大距离
    public LayerMask collisionLayers;   // 相机碰撞检测的层
    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        Vector3 desiredPosition = target.position + target.rotation * offset;

        RaycastHit hit;
        if (Physics.Raycast(target.position, desiredPosition - target.position, out hit, maxDistance, collisionLayers))
        {
            float distance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
            desiredPosition = target.position + target.rotation * offset.normalized * distance;
        }

        // 平滑移动相机位置
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // 更新相机的位置
        transform.position = smoothedPosition;

        // 锁定相机的Y轴旋转角度
        Vector3 targetDirection = target.position + target.forward * 5f - transform.position;
        targetDirection.y = 0; // 锁定Y轴
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed);
    }

}