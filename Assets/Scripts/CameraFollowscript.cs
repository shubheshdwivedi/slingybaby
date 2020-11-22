using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowscript : MonoBehaviour{
   
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        Vector3 playerPosition = player.position;
        transform.position = new Vector3 (playerPosition.x + offset.x, playerPosition.y + offset.y, offset.z); 
    }
}
