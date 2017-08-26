using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    float damage;
    float velocity;

    float range;
    float distanceTravelled;

    void Update()
    {
        float frameDistance = velocity * Time.deltaTime;

        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, frameDistance) && !hit.collider.isTrigger)
        {
            // !!! TODO on hit, apply damage to target

            Destroy(gameObject); // destroy self on hit
        }

        if(distanceTravelled > range)
        {
            Destroy(gameObject);
        }

        transform.position = transform.position + transform.forward * frameDistance;
        distanceTravelled += frameDistance;
    }

    public void Init(float _damage, float _velocity, float _range)
    {
        damage = _damage;
        velocity = _velocity;
        range = _range;

        distanceTravelled = 0f;
    }
}
