using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{

    void Start()
    {
        Debug.Log("Shield exists");
    }


    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collided");

        Wall thisWall = collision.gameObject.GetComponent<Wall>();
        thisWall.TakeDamage(transform.position);
        //EntityList.EntityType parsed_enum;
        //if (collision.gameObject.tag != "Untagged")
        //{
        //    parsed_enum = (EntityList.EntityType)System.Enum.Parse(typeof(EntityList.EntityType), collision.gameObject.tag);
        //    targetTag = parsed_enum;
        //    //Debug.Log("parsed_enum = " + parsed_enum);
        //    targetEnt = collision.gameObject;
        //    BombDetonate();
        //}
        //else
        //{
        //    Debug.Log("tag not found: " + collision.gameObject);
        //    collision.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        //}

    }
}
