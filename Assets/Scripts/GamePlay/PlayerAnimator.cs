/*
 * Author: CharSui
 * Created On: 2024.08.31
 * Description: 
 */

using System;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField]
    private GameObject _character;
    
    private Animator _animator;

    // 用于获取移动信息
    private RigidBodyOrbitMove _bodyOrbit;
    private static readonly int s_IsGround = Animator.StringToHash("is_ground");
    private static readonly int s_Speed = Animator.StringToHash("speed");

    private void Awake()
    {
        _animator = _character.GetComponent<Animator>();
        _bodyOrbit = GetComponent<RigidBodyOrbitMove>();
    }

    private void Update()
    {
        SyncToAnimator();
    }

    private void SyncToAnimator()
    {
        var vector = _bodyOrbit.GetVelocity();
        var isGround = _bodyOrbit.isGround;

        var speed = vector.magnitude;
        _animator.SetFloat(s_Speed,speed);
        _animator.SetBool(s_IsGround,isGround);
    }
}
