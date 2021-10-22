using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStats : MonoBehaviour
{
    public GameObject player;
    public int health;
    [SerializeField] Animator animator;

    public Transform goal;
    private float ranFloat = 0;
    private float counter= 0;
    NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame

    void Update()
    {
        agent.destination = goal.position;
        if(health <= 0)
        {
            Destroy(this.gameObject);
        }
        Vector3 lookAt = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.LookAt(lookAt);
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if(distanceToPlayer <= 2.1)
        {
            if(ranFloat == 0)
            {
                ranFloat = Random.Range(1.0f, 3.0f);
            }
            else
            {
                counter += Time.deltaTime;
                if(counter > ranFloat)
                {
                    animator.SetBool("Attacking", true);
                    ranFloat = 0;
                    counter = 0;
                }
            }
        }
        //find forward and right velocity
        Vector3 localVelocity = transform.InverseTransformVector(agent.velocity);

        //find if touching ground or nor

        
        //update animator
        animator.SetFloat("Speed", agent.velocity.magnitude);
        animator.SetFloat("Forward speed", localVelocity.z);
        animator.SetFloat("Horizontal speed", localVelocity.x);
    }
}
