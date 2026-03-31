using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PlayerResourcesUI : MonoBehaviour
{
    [Header("资源参数")]
    public float maxCapacityPerResource = 300f;  // 修改为：每种资源的上限

    private Dictionary<ResourceType, float> resources = new Dictionary<ResourceType, float>()
    {
        { ResourceType.Wood, 0f },
        { ResourceType.Crop, 0f },
        { ResourceType.Stone, 0f },
        { ResourceType.Meat, 0f }
    };

    [Header("UI引用 - Sliders")]
    public Slider woodSlider;
    public Slider stoneSlider;
    public Slider cropSlider;
    public Slider meatSlider;

    [Header("UI引用 - TextMeshProUGUI（显示数量）")]
    public TextMeshProUGUI woodLabel;
    public TextMeshProUGUI stoneLabel;
    public TextMeshProUGUI cropLabel;
    public TextMeshProUGUI meatLabel;

    [Header("提示标签")]
    public TextMeshProUGUI collectPrompt;   // 采集提示
    public TextMeshProUGUI deliveryPrompt;  // 交货提示

    private Dictionary<ResourceType, Slider> sliderMap;
    private Dictionary<ResourceType, TextMeshProUGUI> labelMap;

    private void Start()
    {
        sliderMap = new Dictionary<ResourceType, Slider>()
        {
            { ResourceType.Wood, woodSlider },
            { ResourceType.Crop, cropSlider },
            { ResourceType.Stone, stoneSlider },
            { ResourceType.Meat, meatSlider }
        };

        labelMap = new Dictionary<ResourceType, TextMeshProUGUI>()
        {
            { ResourceType.Wood, woodLabel },
            { ResourceType.Crop, cropLabel },
            { ResourceType.Stone, stoneLabel },
            { ResourceType.Meat, meatLabel }
        };

        // 初始化所有Slider - 每种资源的maxValue都是300
        foreach (var slider in sliderMap.Values)
        {
            if (slider != null)
                slider.maxValue = maxCapacityPerResource;
        }
        resources[ResourceType.Wood] = 0f;
        resources[ResourceType.Crop] = 0f;
        resources[ResourceType.Stone] = 0f;
        resources[ResourceType.Meat] = 0f;

        if (collectPrompt != null)
            collectPrompt.gameObject.SetActive(false);
        if (deliveryPrompt != null)
            deliveryPrompt.gameObject.SetActive(false);
        UpdateUI();
    }

    /// <summary>
    /// 添加资源 - 每种资源独立有300的上限
    /// </summary>
    public void AddResource(ResourceType type, float amount)
    {
        float currentAmount = resources[type];
        float availableSpace = maxCapacityPerResource - currentAmount;

        float actualAmount = Mathf.Min(amount, availableSpace);
        resources[type] += actualAmount;

        UpdateUI();
    }

    /// <summary>
    /// 移除资源
    /// </summary>
    public void RemoveResource(ResourceType type, float amount)
    {
        resources[type] = Mathf.Max(0, resources[type] - amount);
        UpdateUI();
    }

    /// <summary>
    /// 获取指定类型的资源数量
    /// </summary>
    public float GetResource(ResourceType type)
    {
        return resources[type];
    }

    /// <summary>
    /// 获取总资源数量
    /// </summary>
    public float GetTotalResources()
    {
        float total = 0;
        foreach (var amount in resources.Values)
        {
            total += amount;
        }
        return total;
    }

    /// <summary>
    /// 清空所有资源
    /// </summary>
    public void ClearResources()
    {
        resources[ResourceType.Wood] = 0;
        resources[ResourceType.Crop] = 0;
        resources[ResourceType.Stone] = 0;
        resources[ResourceType.Meat] = 0;
        UpdateUI();
    }

    private void UpdateUI()
    {
        // 更新Slider
        if (woodSlider != null)
            woodSlider.value = resources[ResourceType.Wood];
        if (cropSlider != null)
            cropSlider.value = resources[ResourceType.Crop];
        if (stoneSlider != null)
            stoneSlider.value = resources[ResourceType.Stone];
        if (meatSlider != null)
            meatSlider.value = resources[ResourceType.Meat];

        // 更新标签（精确到个位数）
        if (woodLabel != null)
            woodLabel.text = Mathf.FloorToInt(resources[ResourceType.Wood]).ToString();
        if (cropLabel != null)
            cropLabel.text = Mathf.FloorToInt(resources[ResourceType.Crop]).ToString();
        if (stoneLabel != null)
            stoneLabel.text = Mathf.FloorToInt(resources[ResourceType.Stone]).ToString();
        if (meatLabel != null)
            meatLabel.text = Mathf.FloorToInt(resources[ResourceType.Meat]).ToString();
    }

    /// <summary>
    /// 显示/隐藏采集提示
    /// </summary>
    public void ShowCollectPrompt(bool show)
    {
        if (collectPrompt != null)
            collectPrompt.gameObject.SetActive(show);
    }

    /// <summary>
    /// 显示/隐藏交货提示
    /// </summary>
    public void ShowDeliveryPrompt(bool show)
    {
        if (deliveryPrompt != null)
            deliveryPrompt.gameObject.SetActive(show);
    }
}