using System.Collections;
using UnityEngine;

public class FireSpread : MonoBehaviour
{
    public Transform[] firePoint;         // 火焰点位置数组
    public float spreedInterval = 5f;     // 火焰扩散的时间间隔
    public Transform player;              // 玩家对象
    public Transform castle;              // 城堡对象（可以是中心点）
    public float disableDistance = 50f;   // 玩家离开城堡后火焰停止的距离

    private int index = 0;
    private bool isSpreading = false;     // 判断火焰是否正在扩散

    private void Start()
    {
        // 可以选择在Start方法中开始扩散
        // FireSpreadStart();
    }

    /// <summary>
    /// 火焰扩散
    /// </summary>
    void FireSpreadStart()
    {
        if (!isSpreading)   // 防止重复启动扩散协程
        {
            isSpreading = true;
            StartCoroutine(FireSpreadCoroutine());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FireSpreadStart();
        }
    }

    private IEnumerator FireSpreadCoroutine()
    {
        while (true)
        {
            // 检查玩家是否离城堡太远
            float distanceToCastle = Vector3.Distance(player.position, castle.position);

            if (distanceToCastle > disableDistance)
            {
                StopFireSpread();
                yield break;   // 退出协程，停止火焰扩散
            }

            if (index >= firePoint.Length)
            {
                yield break;   // 如果所有火焰点都激活，则退出
            }

            // 激活下一个火焰点
            firePoint[index].gameObject.SetActive(true);
            index++;

            // 等待设定的扩散间隔时间
            yield return new WaitForSeconds(spreedInterval);
        }
    }

    // 停止火焰扩散和火焰点的渲染
    private void StopFireSpread()
    {
        StopAllCoroutines();  // 停止所有正在进行的协程

        // 禁用所有火焰点
        foreach (Transform point in firePoint)
        {
            point.gameObject.SetActive(false);
        }

        index = 0;   // 重置火焰扩散索引
        isSpreading = false;
    }

    private void Update()
    {
        // 实时监控玩家与城堡的距离，自动停止火焰扩散
        float distanceToCastle = Vector3.Distance(player.position, castle.position);
        if (distanceToCastle > disableDistance)
        {
            StopFireSpread();
        }
    }
}
