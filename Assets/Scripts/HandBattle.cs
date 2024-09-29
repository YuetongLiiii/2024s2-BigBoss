using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandBattle : MonoBehaviour
{
    public Animator animator;  // 指向包含Animator组件的对象

    void Update()
    {
        if (Input.GetMouseButtonDown(0))  // 当鼠标左键被按下
        {
            Attack();
        }
    }

    void Attack()

    {
        Debug.Log("beat");

        Debug.Log("Current Animator Controller: " + animator.runtimeAnimatorController.name);

        animator.SetBool("hit1", true);
        animator.SetBool("hit2", true);// 设置Animator的attack1参数为true，开始攻击动画
        StartCoroutine(ResetAttack());
    }

    IEnumerator ResetAttack()
    {
        yield return null;  // 等待直到下一帧
        animator.SetBool("hit1", false);
        animator.SetBool("hit2", false);// 将attack1重置为false，确保动画可以再次触发
    }
}
