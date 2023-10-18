using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _RedBulletScript : MonoBehaviour
{
    float _time;
    public float OnShottime = 1.15f;
    public float DestroyTime = 9f;
    float Speed = 125f;
    public Rigidbody _rb;
    public BoxCollider _trigger;
    private void Start()
    {
        _time = Time.time+ OnShottime+Random.Range(0f,0.095f);
        //Destroy(transform, DestroyTime);
        Destroy(transform.gameObject, DestroyTime);
    }
    private void FixedUpdate()
    {
        if (Time.time > _time)
        {
            _rb.velocity = transform.forward * Speed * Time.fixedDeltaTime;
        }
    }

}
