using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public bool IsMove;


    [SerializeField] private float _moveSpeed;


    private Rigidbody2D _myRigidbody2D;


    void Start()
    {
        _myRigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
    }
}