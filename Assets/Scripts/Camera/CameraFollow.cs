using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform tank; // Drag the tank GameObject here
    public float smoothSpeed = 0.125f;
   public Vector3 offset = new Vector3(0, 0, -10); // Adjust this in the inspector for close zoom

    void FixedUpdate()
{
    Vector3 desiredPosition = tank.position + offset;
    Debug.Log("Camera Target Position: " + desiredPosition); // Check if this is correct
    Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
    transform.position = smoothedPosition;
}

}
