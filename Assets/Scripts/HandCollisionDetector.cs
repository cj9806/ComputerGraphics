using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCollisionDetector : MonoBehaviour
{
    [HideInInspector]
    public Collision col;
    [SerializeField] SamplePlayerCharacter playerCharacter;
    private EnemyStats enemyStats;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.name);
        if (playerCharacter.attacking)
        {
            enemyStats = collision.gameObject.GetComponentInParent<EnemyStats>();
            if (playerCharacter.sword.activeSelf)
            {
                enemyStats.health -= 20;
            }
            else
            {
                enemyStats.health -= 2;
            }
            playerCharacter.attacking = false;
        }
    }
    private void OnCollisionExit(Collision collision)
    {

    }
}
