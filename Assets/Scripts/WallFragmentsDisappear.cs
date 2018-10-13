using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallFragmentsDisappear : MonoBehaviour {

    float waitBegging = 10f;
    float maxTime = 20f;
    float waitToDestroy = 4f;
    float waitToGoDown = 0;
    float gravityValue = 0.25f;
    float actualTime = 0;
    MeshCollider meshCollider;
    Rigidbody rb;
	// Use this for initialization
	void Awake () {
        meshCollider = GetComponent<MeshCollider>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        waitToGoDown = Random.value * maxTime;
        StartCoroutine(WaitStart());
        
    }

    // Update is called once per frame
    void Update () {
		
	}

    IEnumerator WaitStart()
    {
        yield return new WaitForSeconds(waitBegging);
        StartCoroutine(WaitToGoDown());

    }

    IEnumerator WaitToGoDown()
    {
        yield return new WaitForSeconds(waitToGoDown);
        StartCoroutine(WaitDestroy());

    }

    IEnumerator WaitDestroy()
    {
        Destroy(meshCollider);
        for (; actualTime < waitToDestroy; )
        {
            actualTime += Time.deltaTime;
            rb.AddForce(transform.InverseTransformVector(-transform.up * gravityValue / Time.deltaTime), ForceMode.Acceleration);
            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "player")
        {
            rb.AddExplosionForce(10000f, collision.contacts[0].point, 10.25f);
        }
    }

}

