using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerY : MonoBehaviour
{
    [SerializeField] private float dampTime = 0.5f;
    
    [SerializeField] private Transform target;
    [SerializeField] private Camera camera;

    [SerializeField] private float maxY;
    [SerializeField] private float minY;

    private Vector3 velocity = Vector3.zero;

    private void Update()
    {
        if (target)
        {
            Vector3 point = camera.WorldToViewportPoint(target.position);
            Vector3 delta = target.position - camera.ViewportToWorldPoint(new Vector3(point.x, 0.5f, point.z));
            Vector3 destination = transform.position + delta;
            Vector3 final = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);

            if (final.y < minY)
            {
                transform.position = new Vector3(transform.position.x, minY, transform.position.z);
            }
            else if (final.y > maxY)
            {
                transform.position = new Vector3(transform.position.x, maxY, transform.position.z);
            }
            else
            {
                transform.position = final;
            }
        }
    }
}
