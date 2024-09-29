using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 30f;
    [SerializeField] private float runSpeed = 60f;
    [SerializeField] private float turnSpeed = 20f;
    public Animator animator;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 获取水平和垂直方向的输入
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // 检查玩家是否按下加速键（例如左Shift键）
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        // 根据是否按下Shift键来决定移动速度
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        // 计算移动向量
    
        Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ;
        Vector3 move = moveDirection.normalized * currentSpeed;
        // 使用MovePosition移动角色 (仅在这里使用 Time.deltaTime)
        rb.MovePosition(transform.position + move * Time.deltaTime);

        // 设置动画的Speed参数
        float speed = move.magnitude;
        animator.SetFloat("Speed", speed);

        // 如果在跑步，将动画的"Speed"参数设置为更高的值以触发跑步动画
        if (speed > 0)
        {
            animator.SetFloat("Speed", speed);
        }
        else
        {
            animator.SetFloat("Speed", 0);
        }
    }
}
