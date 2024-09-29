using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DoorInteractionWithUIPanel : MonoBehaviour
{
    public GameObject questionPanel;  // UI 面板
    public TMP_InputField answerInputField;  // 输入框
    public string correctAnswer;  // 正确答案（每个房间不同）
    public GameObject door;  // 门的对象
    public TextMeshProUGUI feedbackText;  // 显示结果的 TextMeshPro
    private bool playerNearby = false;  // 检测玩家是否接近
    private bool isDoorOpened = false;  // 判断门是否已经打开
    private bool isAnswerCorrect = false;  // 判断是否答案正确
    public float openAngle = -90f;  // 门打开时的旋转角度
    public float smoothSpeed = 2f;  // 开门速度

    private Quaternion initialRotation;  // 门关闭时的旋转
    private Quaternion targetRotation;  // 门打开时的目标旋转

    void Start()
    {
        // 初始化旋转角度
        initialRotation = door.transform.rotation;
        targetRotation = Quaternion.Euler(door.transform.eulerAngles.x, door.transform.eulerAngles.y + openAngle, door.transform.eulerAngles.z);

        // 确保问题面板在游戏开始时是隐藏的
        questionPanel.SetActive(false);

        // 确保反馈文本初始为空
        feedbackText.text = "";
    }

    void OnTriggerEnter(Collider other)
    {
        // 只在门未打开且未回答正确问题的情况下显示面板
        if (other.CompareTag("Player") && !isDoorOpened)
        {
            playerNearby = true;
            questionPanel.SetActive(true);  // 显示问题面板
            CameraFollowMouse.isUIActive = true;  // 禁用鼠标控制方向
            Cursor.lockState = CursorLockMode.None;  // 解锁鼠标
            Cursor.visible = true;  // 显示鼠标
        }
    }

    void OnTriggerExit(Collider other)
    {
        // 如果玩家离开了触发器且没有回答正确问题，隐藏面板
        if (other.CompareTag("Player") && !isAnswerCorrect)
        {
            playerNearby = false;
            questionPanel.SetActive(false);  // 隐藏问题面板
            feedbackText.text = "";  // 清空反馈文本
            CameraFollowMouse.isUIActive = false;  // 重新启用鼠标控制
            Cursor.lockState = CursorLockMode.Locked;  // 锁定鼠标
            Cursor.visible = false;  // 隐藏鼠标
        }
    }

    // 点击确认按钮时调用，验证答案
    public void OnConfirmButtonClick()
    {
        feedbackText.gameObject.SetActive(true);  // 确保反馈文本可见
        Debug.Log("Answer submitted: " + answerInputField.text);  // 打印输入的答案

        if (answerInputField.text == correctAnswer)
        {
            Debug.Log("Correct answer! Opening the door.");  // 打印确认答案正确
            feedbackText.text = "<color=green>CORRECT! Door Opened</color>";  // 显示回答正确的反馈
            isAnswerCorrect = true;  // 设置答案为正确，防止 OnTriggerExit 立即关闭面板
            StartCoroutine(WaitAndHidePanel());  // 开启协程，2秒后隐藏面板
            StartCoroutine(OpenDoorSmoothly());  // 平滑打开门
            isDoorOpened = true;  // 设置门为已打开，防止再次显示问题面板
        }
        else
        {
            Debug.Log("Wrong answer. Door remains closed.");  // 打印确认答案错误
            feedbackText.text = "<color=red>WRONG! Try Again！</color>";  // 显示回答错误的反馈
        }
    }

    // 点击取消按钮时调用，隐藏UI但可以再次靠近门显示
    public void OnCancelButtonClick()
    {
        questionPanel.SetActive(false);  // 隐藏问题面板
        CameraFollowMouse.isUIActive = false;  // 重新启用鼠标控制
        Cursor.lockState = CursorLockMode.Locked;  // 锁定鼠标
        Cursor.visible = false;  // 隐藏鼠标
        feedbackText.text = "";  // 隐藏反馈文本
        isAnswerCorrect = false;  // 重置答案状态
    }

    // 协程：等待2秒后隐藏面板
    IEnumerator WaitAndHidePanel()
    {
        yield return new WaitForSeconds(2);  // 等待2秒
        questionPanel.SetActive(false);  // 隐藏问题面板
        CameraFollowMouse.isUIActive = false;  // 重新启用鼠标控制
        Cursor.lockState = CursorLockMode.Locked;  // 锁定鼠标
        Cursor.visible = false;  // 隐藏鼠标
    }

    // 协程：平滑开门
    IEnumerator OpenDoorSmoothly()
    {
        float elapsedTime = 0f;
        float duration = 1f / smoothSpeed;  // 根据速度确定开门时间
        Quaternion startRotation = door.transform.rotation;

        while (elapsedTime < duration)
        {
            // 平滑插值
            door.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;  // 等待下一帧
        }

        // 确保门完全旋转到目标角度
        door.transform.rotation = targetRotation;
        Debug.Log("Door opened!");  // 打印调试信息，确认门已打开
    }
}
