using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour {

    public int vel = 3;
    public GameObject stone;
    

	void Update () {

        if (Input.GetKeyDown("e"))
        {
            GameObject clone = Instantiate(stone, transform.position, transform.rotation);
            clone.GetComponent<Rigidbody>().AddForce(transform.forward * 500 * vel);
            Destroy(clone, 5);
        }

    }
}
