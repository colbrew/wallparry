using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public GameObject player;
    public GameObject fracturedWall;
    public float speed;
    private Quaternion targetRotation;
    private Vector3 spawnOrigin;
    private float startTime;
    private float journeyLength;


    void Start()
    {
        spawnOrigin = transform.position;

        if (speed == 0.0)
        {
            speed = 1.0f;
        }

        SetOrientation();
        SetTrajectory();

    }

    void FixedUpdate()
    {
        Vector3 targetOrigin = new Vector3(0, 0, 0);
        if (player != null)
        {
            targetOrigin = player.transform.position;
        }

        if (targetOrigin != null && spawnOrigin != null && speed != 0)
        {
            float distCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distCovered / journeyLength;

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 70);
            transform.position = Vector3.Lerp(spawnOrigin, targetOrigin, fractionOfJourney);
        }

    }

    public void SetOrientation()
    {
        Vector3 directionToFace = player.transform.position - transform.position;
        //Debug.DrawRay(transform.position, directionToFace, Color.red);

        targetRotation = Quaternion.LookRotation(new Vector3(directionToFace.x, 0, directionToFace.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 70);
    }

    public void SetTrajectory()
    {
        startTime = Time.time;
        journeyLength = Vector3.Distance(spawnOrigin, player.transform.position);
    }

    public void TakeDamage(Vector3 impactLocation)
    {
        GameObject fracturedCrateObj = Instantiate(fracturedWall, transform.position, Quaternion.identity) as GameObject;
        Rigidbody[] allRigidBodies = fracturedCrateObj.GetComponentsInChildren<Rigidbody>();
        if (allRigidBodies.Length > 0)
        {
            foreach (var body in allRigidBodies)
            {
                body.AddExplosionForce(500, transform.position, 1);
            }

        }

        Destroy(this.gameObject);
    }
}
