using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public int maxHealth = 200;
    private int currentHealth;
    public GameObject deathEffectPrefab; // 死亡粒子效果预制件
    public HealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        healthBar.SetHealth(currentHealth); // 更新血条UI

        if (currentHealth <= 0)
        {
            Die();
        }
        Debug.Log("current health is :" + currentHealth);
    }

    void Die()
    {
        Destroy(gameObject); 
        // 播放死亡粒子效果
        if (deathEffectPrefab != null)
        {
            GameObject obj = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            Destroy(obj, 5f);
        }
    }
}
