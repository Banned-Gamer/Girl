using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    public bool IsMove;
    public int current = -1;
    //public Text nowText;


    [SerializeField] private float _moveSpeed;

    private GameObject _player;
    private Rigidbody2D _myRigidbody2D;
    private Animator _myAnimator;


    private const string _moveCommand = "IsWalk";

    void Start()
    {
        _player = this.gameObject;
        _myAnimator = GetComponent<Animator>();
        _myRigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (current == 1)
        {
            //nowText.text = "right";
            Move(1);
        }

        else if (current == 0)
        {
            //nowText.text = "left";
            Move(-1);
        }
        else
        {
            //nowText.text = "other";
            StopMove();
        }
    }

    public void Move(float direction)
    {

        Vector3 myDirection = new Vector3(direction * 0.15f, 0.15f, 1);

        Vector3 myVelocity = Vector3.right * _moveSpeed * direction*Time.deltaTime;

        transform.position += myVelocity;

        _myAnimator.SetBool(_moveCommand, true);

        transform.localScale = myDirection;
    }

    public void StopMove()
    {
        Debug.Log("stop");
        _myAnimator.SetBool(_moveCommand, false);
        _myRigidbody2D.velocity = Vector2.zero;
    }
}