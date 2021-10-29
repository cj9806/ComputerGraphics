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
        if (enemyStats != null && playerCharacter.attack)
        {
            if (playerCharacter.sword.activeSelf == false)
                enemyStats.health -= 2;
            else enemyStats.health -= 10;
            if (enemyStats.health <= 0) col = null;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (playerCharacter.attack)
        {
            col = collision;
            enemyStats = col.gameObject.GetComponentInParent<EnemyStats>();

            playerCharacter.attack = false;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        col = null;
    }
}
