using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraRotation : MonoBehaviour {

    public float speedH = 2.0f;
    private float yaw = 270f;
    [SerializeField] private Transform target;

    private void Update()
    {
        yaw += speedH * Input.GetAxis("MouseX");

        transform.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, yaw, transform.rotation.eulerAngles.z);

        transform.position = target.position;
    }
    
    public void FixCamera()
    {
        target = transform;
    }
}
