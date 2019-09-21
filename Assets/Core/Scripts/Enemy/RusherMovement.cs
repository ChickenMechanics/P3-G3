using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.AI.NavMeshAgent;

public class RusherMovement : MonoBehaviour
{
    private Transform player;
    public NavMeshAgent agent;

    // Start is called before the first frame update
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0);
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 playerPos = player.position;
        var position  = transform.position;
        var rotation  = transform.rotation;

        var lookPosition = playerPos - position;

        var distanceToPlayer = lookPosition.magnitude;
        lookPosition.y = 0;

        var lookRotation = Quaternion.LookRotation(lookPosition);

        transform.rotation = Quaternion.Slerp(rotation, lookRotation, 0.1f);

        agent.SetDestination(playerPos);

        if (distanceToPlayer < 4)
        {
#if DEBUG
            GetComponent<Renderer>().material.color = Color.red;
#endif
            //Explode
        }

    }
}
