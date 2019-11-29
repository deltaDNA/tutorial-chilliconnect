using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bodypart : MonoBehaviour {

	

    private void OnCollisionEnter(Collision collision)
    {
        Collider myCollider = collision.contacts[0].thisCollider;
        //Debug.Log("BP-Col " + myCollider.name);
    }

    void OnTriggerEnter(Collider c)
    {
        //Debug.Log("BP-Trig " + c.name);
    }
            
                
}