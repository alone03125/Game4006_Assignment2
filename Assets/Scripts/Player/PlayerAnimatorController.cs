using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    public Animator animator;
    private string currentAnimationState = "Idle";

    private void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 播放移动或奔跑动画
    /// </summary>
    public void PlayMove(bool isRunning)
    {
        string targetState = isRunning ? "Run" : "Walk";
        if (currentAnimationState != targetState)
        {
            animator.CrossFade(targetState, 0.1f);
            currentAnimationState = targetState;
        }
    }

    /// <summary>
    /// 播放闲置动画
    /// </summary>
    public void PlayIdle()
    {
        if (currentAnimationState != "Idle")
        {
            animator.CrossFade("Idle", 0.1f);
            currentAnimationState = "Idle";
        }
    }

    /// <summary>
    /// 播放采集动画
    /// </summary>
    public void PlayCollect(ResourceType type)
    {
        string animName = type.ToString() + "_Collect";
        animator.CrossFade(animName, 0.1f);
        currentAnimationState = animName;
    }

    /// <summary>
    /// 恢复到闲置状态
    /// </summary>
    public void ResetToIdle()
    {
        PlayIdle();
    }
}