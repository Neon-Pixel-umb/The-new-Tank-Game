using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankBarrelRotation : MonoBehaviour
{
    void Update()
    {
        float angle;

        // Check if the joystick is being used
        float rightStickHorizontal = Input.GetAxis("RightStickHorizontal");
        float rightStickVertical = Input.GetAxis("RightStickVertical");
        Vector2 stickInput = new Vector2(rightStickHorizontal, rightStickVertical);

        if (stickInput.magnitude > 0.1f) // Use joystick if it’s moved
        {
            angle = Mathf.Atan2(stickInput.y, stickInput.x) * Mathf.Rad2Deg;
        }
        else // Otherwise, use the mouse position
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mousePos - transform.position;
            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }

        // Set the barrel rotation
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f); // Adjust by -90 if your barrel sprite points up by default
    }
}
