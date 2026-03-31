using UnityEngine;
using System;
using UnityEngine.SocialPlatforms.Impl;

[System.Serializable]
public class DeliveryTask
{
    public int[] requirements = new int[4];  // 四种资源的需求量
    public int[] delivered = new int[4];     // 四种资源的已交量
    public float taskStartTime;
    public int currentStep = 0;              // 当前交付步骤(0-3)
    public bool isCompleted = false;

    // 移除构造函数中的Random调用
    public DeliveryTask()
    {
        for (int i = 0; i < 4; i++)
        {
            requirements[i] = 0;  // 初始化为0
            delivered[i] = 0;
        }
        taskStartTime = 0f;
    }

    /// <summary>
    /// 初始化任务（应在Start或Awake中调用）
    /// </summary>
    public void Initialize()
    {
        for (int i = 0; i < 4; i++)
        {
            requirements[i] = UnityEngine.Random.Range(50, 201);
            delivered[i] = 0;
        }
        taskStartTime = Time.time;
        currentStep = 0;
        Debug.Log("进行了一次任务初始化");
        isCompleted = false;
    }

    /// <summary>
    /// 计算得分
    /// </summary>
    public int CalculateScore()
    {
        int timeCost = Mathf.FloorToInt(Time.time - taskStartTime);
        int absDiff = 0;

        for (int i = 0; i < 4; i++)
        {
            absDiff += Mathf.Abs(requirements[i] - delivered[i]);
        }

        int score = Mathf.Max(0, 600 - timeCost - absDiff);
        Debug.Log("玩家分数"+score);
        return score;
    }

    /// <summary>
    /// 获取任务进行时间
    /// </summary>
    public float GetElapsedTime()
    {
        return Time.time - taskStartTime;
    }
}