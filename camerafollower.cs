using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camerafollower : MonoBehaviour
{
    private Vector3 offset = new Vector3(0f,0f, -60f);
    private float smoothTime = 0.25f;
    private Vector3 veloctiy = Vector3.zero;

    [SerializeField] private Transform target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref veloctiy, smoothTime);
    }
}
