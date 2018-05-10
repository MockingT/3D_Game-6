using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]

public class ActorController : MonoBehaviour {
    private Vector3 velocity;
    private float rotateSpeed = 15f;
    private float runSpeed = 7f;
    private Animator ani;
    private AnimatorStateInfo currentBaseState;
    private Rigidbody rig;

    // Use this for initialization
    void Start () {
        ani = GetComponent<Animator>();
        rig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate () {
        // the target dies
        if (!ani.GetBool("isLive"))
            return;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        ani.SetFloat("Speed", Mathf.Max(Mathf.Abs(x), Mathf.Abs(z)));
        ani.speed = 1 + ani.GetFloat("Speed") / 3;
        velocity = new Vector3(x, 0, z);
        if (x != 0 || z != 0)
        {
            Quaternion rotation = Quaternion.LookRotation(velocity);
            if (transform.rotation != rotation) transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.fixedDeltaTime * rotateSpeed);
        }
        // move the target
        this.transform.position += velocity * Time.fixedDeltaTime * runSpeed;
    }
    
    // check the entering
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Area"))
        {
            Publish publish = Publisher.getInstance();
            int patrolType = other.gameObject.name[other.gameObject.name.Length - 1] - '0';
            // publish the message it has entered
            publish.notify(ActorState.ENTER_AREA, patrolType, this.gameObject);
        }
    }
    
    // check the death
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Patrol") && ani.GetBool("isLive"))
        {
            ani.SetBool("isLive", false);
            ani.SetTrigger("toDie");
            // publish the death
            Publish publish = Publisher.getInstance();
            publish.notify(ActorState.DEATH, 0, null);
        }
    }
}
