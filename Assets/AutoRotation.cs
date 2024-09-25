using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotation : MonoBehaviour
{
    [SerializeField] private Vector3 _speed = Vector3.one;

    void Update()
    {
        transform.Rotate(_speed * Time.deltaTime);   
    }
}
