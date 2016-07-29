using System;
using UnityEngine;

/// <summary>
/// 建筑事件
/// </summary>
public class BuildingEvent : MonoBehaviour
{
    public Transform hot;

    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayerEquipment()
    {
        animator.SetTrigger("IsPlayer");
    }

    void OnDisable()
    {

    }

    void Update()
    {
     
    }
}
