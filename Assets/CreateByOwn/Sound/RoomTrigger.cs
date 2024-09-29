using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class RoomTrigger : MonoBehaviour
{
    public AudioClip pianoMusicClip;         // 要播放的钢琴音乐剪辑
    public AudioSource mainThemeAudioSource; 
    private AudioSource roomAudioSource;
    private BoxCollider boxCollider;
    private GameObject player;
    private bool isInTriggerArea = false;

    void Start()
    {
        // 获取 BoxCollider 组件
        boxCollider = GetComponent<BoxCollider>();

        // 获取或添加房间的 AudioSource 组件
        roomAudioSource = GetComponent<AudioSource>();
        if (roomAudioSource == null)
        {
            Debug.LogWarning("AudioSource is missing on " + gameObject.name + ". Adding one now.");
            roomAudioSource = gameObject.AddComponent<AudioSource>();
        }

        // 配置房间的 AudioSource
        roomAudioSource.clip = pianoMusicClip;
        roomAudioSource.playOnAwake = false; // 确保它不会自动播放
        roomAudioSource.loop = true;         // 如果需要，可以设置为循环播放

        // 获取玩家对象
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player object with tag 'Player' not found in the scene.");
        }

        // 检查主主题音乐的 AudioSource 是否已分配
        if (mainThemeAudioSource == null)
        {
            Debug.LogError("Main theme AudioSource is not assigned in the inspector.");
        }
    }

    void Update()
    {
        if (player != null)
        {
            Vector3 playerPosition = player.transform.position;

            // 检查玩家是否在 BoxCollider 范围内，包括 Y 轴的范围
            if (IsPlayerInBoxXYZ(playerPosition))
            {
                if (!isInTriggerArea)
                {
                    // 暂停主主题音乐
                    if (mainThemeAudioSource != null && mainThemeAudioSource.isPlaying)
                    {
                        mainThemeAudioSource.Pause();
                    }

                    // 播放钢琴音乐
                    if (!roomAudioSource.isPlaying)
                    {
                        roomAudioSource.Play();
                    }

                    isInTriggerArea = true;
                }
            }
            else
            {
                if (isInTriggerArea)
                {
                    // 停止钢琴音乐
                    if (roomAudioSource.isPlaying)
                    {
                        roomAudioSource.Stop();
                    }

                    // 恢复主主题音乐
                    if (mainThemeAudioSource != null)
                    {
                        mainThemeAudioSource.UnPause();
                    }

                    isInTriggerArea = false;
                }
            }
        }
    }

    private bool IsPlayerInBoxXYZ(Vector3 playerPosition)
    {
        Vector3 minBounds = boxCollider.bounds.min;
        Vector3 maxBounds = boxCollider.bounds.max;

        // 检查玩家的 X, Z 坐标是否在 BoxCollider 的范围内，Y 坐标是否在 0 到 2 之间
        return playerPosition.x >= minBounds.x && playerPosition.x <= maxBounds.x &&
               playerPosition.z >= minBounds.z && playerPosition.z <= maxBounds.z &&
               playerPosition.y >= 0 && playerPosition.y <= 2;
    }
}