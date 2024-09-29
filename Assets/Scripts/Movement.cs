using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private float speed = 5f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical);
        transform.Translate(direction*speed*Time.deltaTime);
        // rig.MovePosition(direction*speed*Time.deltaTime);
        
    }
}