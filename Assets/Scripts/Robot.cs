using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : DRMSingleton<Robot>
{
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 当交易返回结果时调用此方法，随机播放 1-4 中的一个动画
    /// </summary>
    public void OnTransactionResult()
    {
        if (animator != null)
        {
            // Random.Range(1, 5) 会返回 1, 2, 3 或 4
            int randomIndex = Random.Range(1, 5);
            animator.SetInteger("index", randomIndex);
            
            // 如果需要触发播放，可能还需要 Trigger 或者重新进入状态
            // 这里假设 animator controller 是根据 index 值来切换状态的
        }
    }
}
