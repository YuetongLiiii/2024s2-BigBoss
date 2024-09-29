using UnityEngine;

public class Laser : MonoBehaviour
{
    public float laserScale = 1;
    public Color laserColor = new Vector4(1, 1, 1, 1);
    public GameObject HitEffect;
    public GameObject FlashEffect;
    public float HitOffset = 0;

    public float MaxLength;

    private bool updateOnce = false;  // 确保特效只播放一次
    private ParticleSystem laserPS;
    private ParticleSystem[] Flash;
    private ParticleSystem[] Hit;
    private Material laserMat;
    private int particleCount;
    private ParticleSystem.Particle[] particles;
    private Vector3[] particlesPositions;
    private float dissolveTimer = 0;
    private bool startDissolve = false;

    void Start()
    {
        laserPS = GetComponent<ParticleSystem>();
        laserMat = GetComponent<ParticleSystemRenderer>().material;
        Flash = FlashEffect.GetComponentsInChildren<ParticleSystem>();
        Hit = HitEffect.GetComponentsInChildren<ParticleSystem>();
        laserMat.SetFloat("_Scale", laserScale);
    }

    void Update()
    {
        if (laserPS != null && !updateOnce)
        {
            // 设置起点
            laserMat.SetVector("_StartPoint", transform.position);

            // 通过Raycast检测碰撞
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, MaxLength))
            {
                particleCount = Mathf.RoundToInt(hit.distance / (2 * laserScale));
                if (particleCount < hit.distance / (2 * laserScale))
                {
                    particleCount += 1;
                }

                particlesPositions = new Vector3[particleCount];
                AddParticles();

                laserMat.SetFloat("_Distance", hit.distance);
                laserMat.SetVector("_EndPoint", hit.point);

                // 播放击中特效和闪光特效，只播放一次
                if (Hit != null && !updateOnce)
                {
                    HitEffect.transform.position = hit.point + hit.normal * HitOffset;
                    HitEffect.transform.LookAt(hit.point);
                    foreach (var hitPS in Hit)
                    {
                        if (!hitPS.isPlaying) hitPS.Play();
                    }
                    foreach (var flashPS in Flash)
                    {
                        if (!flashPS.isPlaying) flashPS.Play();
                    }
                }

                // 确保只更新一次
                updateOnce = true;
            }
            else
            {
                // 激光未碰撞时，设置最大长度
                var EndPos = transform.position + transform.forward * MaxLength;
                var distance = Vector3.Distance(EndPos, transform.position);
                particleCount = Mathf.RoundToInt(distance / (2 * laserScale));
                if (particleCount < distance / (2 * laserScale))
                {
                    particleCount += 1;
                }

                particlesPositions = new Vector3[particleCount];
                AddParticles();

                laserMat.SetFloat("_Distance", distance);
                laserMat.SetVector("_EndPoint", EndPos);

                // 如果没有碰撞，停止击中特效
                if (Hit != null && updateOnce)
                {
                    foreach (var hitPS in Hit)
                    {
                        if (hitPS.isPlaying) hitPS.Stop();
                    }
                }
            }
        }

        if (startDissolve)
        {
            dissolveTimer += Time.deltaTime;
            laserMat.SetFloat("_Dissolve", dissolveTimer * 5);
        }
    }

    void AddParticles()
    {
        particles = new ParticleSystem.Particle[particleCount];

        for (int i = 0; i < particleCount; i++)
        {
            particlesPositions[i] = new Vector3(0f, 0f, 0f) + new Vector3(0f, 0f, i * 2 * laserScale);
            particles[i].position = particlesPositions[i];
            particles[i].startSize3D = new Vector3(0.001f, 0.001f, 2 * laserScale);
            particles[i].startColor = laserColor;
        }
        laserPS.SetParticles(particles, particles.Length);
    }

    public void DisablePrepare()
    {
        transform.parent = null;
        dissolveTimer = 0;
        startDissolve = true;
        updateOnce = true;
        if (Flash != null && Hit != null)
        {
            foreach (var hitPS in Hit)
            {
                if (hitPS.isPlaying) hitPS.Stop();
            }
            foreach (var flashPS in Flash)
            {
                if (flashPS.isPlaying) flashPS.Stop();
            }
        }
    }
}
