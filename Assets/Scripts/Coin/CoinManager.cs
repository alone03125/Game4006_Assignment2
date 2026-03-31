using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    [Header("UI")]
    public TextMeshProUGUI coinCountLabel;

    private int totalCoins = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateUI();
    }

    /// <summary>
    /// 添加金币
    /// </summary>
    public void AddCoin()
    {
        totalCoins++;
        UpdateUI();
    }

    /// <summary>
    /// 获取金币数量
    /// </summary>
    public int GetCoinCount()
    {
        return totalCoins;
    }

    private void UpdateUI()
    {
        if (coinCountLabel != null)
            coinCountLabel.text = "Coins: " + totalCoins.ToString();
    }
}