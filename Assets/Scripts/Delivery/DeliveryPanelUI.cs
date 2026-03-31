using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DeliveryPanelUI : MonoBehaviour
{
    public Slider requirementBar0, requirementBar1, requirementBar2, requirementBar3;
    public Slider deliveryBar0, deliveryBar1, deliveryBar2, deliveryBar3;
    public TextMeshProUGUI[] requirementLabels = new TextMeshProUGUI[4];
    public TextMeshProUGUI timerLabel;

    private Slider[] requirementBars;
    private Slider[] deliveryBars;

    private void Awake()
    {
        requirementBars = new[] { requirementBar0, requirementBar1, requirementBar2, requirementBar3 };
        deliveryBars = new[] { deliveryBar0, deliveryBar1, deliveryBar2, deliveryBar3 };
        foreach (var bar in requirementBars) if (bar != null) bar.maxValue = 300;
        foreach (var bar in deliveryBars) if (bar != null) bar.maxValue = 300;
    }


    public void ShowTask(DeliveryTask task)
    {
        gameObject.SetActive(true);

        for (int i = 0; i < 4; i++)
        {
            if (requirementBars[i] != null)
                requirementBars[i].value = task.requirements[i];
            if (requirementLabels[i] != null)
                requirementLabels[i].text = task.requirements[i].ToString();

            if (deliveryBars[i] != null)
                deliveryBars[i].value = 0;   // 重置已交货进度条
        }
        if (timerLabel != null)
            timerLabel.text = "0s";
    }

    /// <summary>
    /// 更新指定资源类型的交货进度条（也可全部更新，但推荐只更新当前step）
    /// </summary>
    public void UpdateDeliveryProgress(DeliveryTask task, int step)
    {
        if (deliveryBars[step] != null)
            deliveryBars[step].value = task.delivered[step];
    }

    public void UpdateTimer(float elapsedTime)
    {
        if (timerLabel != null)
            timerLabel.text = Mathf.FloorToInt(elapsedTime) + "s";
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}