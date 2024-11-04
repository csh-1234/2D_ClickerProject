using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ScrollMap : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float scrollAmount;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private Vector3 moveDirection;

    void Update()
    {
        //transform.position += moveDirection * moveSpeed * Time.deltaTime;
        //transform.position += moveDirection * moveSpeed * Time.deltaTime;
        //if(transform.position.x <= -scrollAmount)
        //{
        //    transform.position = target.position - moveDirection * scrollAmount;
        //}
    }
    
    void moveScroll()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        if (transform.position.x <= -scrollAmount)
        {
            transform.position = target.position - moveDirection * scrollAmount;
        }
    }
}
