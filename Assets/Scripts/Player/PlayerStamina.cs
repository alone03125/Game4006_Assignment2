using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    [Header("体力参数")]
    public float maxStamina = 100f;
    public float restoreRate = 15f;  // 每秒恢复
    public float depleteRate = 20f;  // 每秒消耗

    private float currentStamina;
    private bool isExhausted = false;

    [Header("UI")]
    public Image staminaBar;
    public CanvasGroup staminaCanvasGroup;

    private void Start()
    {
        currentStamina = maxStamina;
        if (staminaBar != null)
            staminaBar.fillAmount = 1f;
    }

    /// <summary>
    /// 更新体力值
    /// </summary>
    public void UpdateStamina(bool isRunning)
    {
        if (isRunning && !isExhausted)
        {
            currentStamina -= depleteRate * Time.deltaTime;
        }
        else
        {
            currentStamina += restoreRate * Time.deltaTime;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        // 更新UI
        if (staminaBar != null)
        {
            staminaBar.fillAmount = currentStamina / maxStamina;
        }

        // 判断疲劳状态
        if (currentStamina <= 0)
        {
            isExhausted = true;
        }
        if (isExhausted && currentStamina >= maxStamina)
        {
            isExhausted = false;
        }
    }

    /// <summary>
    /// 检查是否可以继续奔跑
    /// </summary>
    public bool CanRun()
    {
        return !isExhausted;
    }

    /// <summary>
    /// 获取移动速度系数（疲劳时70%）
    /// </summary>
    public float GetSpeedMultiplier()
    {
        return isExhausted ? 0.7f : 1f;
    }
}