using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStats : MonoBehaviour
{
    public GameObject player;
    public int health;
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] KinematicPlayerMotor motor;
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
    void FixedUpdate()
    {
        agent.destination = goal.position;
    }
    void Update()
    {
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
        animator.SetFloat("Speed", rigidbody.velocity.magnitude);
        animator.SetBool("Grounded", motor.Grounded);

    }
}
