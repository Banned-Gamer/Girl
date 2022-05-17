using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public bool IsMove;


    [SerializeField] private float _moveSpeed;


    private Rigidbody2D _myRigidbody2D;
    private Animator _myAnimator;


    private const string _moveCommand = "IsWalk";

    void Start()
    {
        _myAnimator = GetComponent<Animator>();
        _myRigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            Move(Input.GetAxisRaw("Horizontal"));
        }
        else
        {
            StopMove();
        }
    }

    public void Move(float direction)
    {
        Vector3 myVelocity = Vector3.right * direction * Time.deltaTime * _moveSpeed;
        transform.position += myVelocity;
        _myAnimator.SetBool(_moveCommand, true);
    }

    public void StopMove()
    {
        _myAnimator.SetBool(_moveCommand,false);
    }
}