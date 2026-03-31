using UnityEngine;

public class Coin : MonoBehaviour
{
    public float lifeTime = 60f;
    private float spawnTime;

    private void Start()
    {
        spawnTime = Time.time;
    }

    private void Update()
    {
        // 60秒后销毁
        if (Time.time - spawnTime > lifeTime)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CoinManager.Instance.AddCoin();
            Destroy(gameObject);
        }
    }
}