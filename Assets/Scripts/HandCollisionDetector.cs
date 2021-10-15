using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCollisionDetector : MonoBehaviour
{
    [HideInInspector]
    public Collision col;
    [SerializeField] SamplePlayerCharacter playerCharacter;
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
        if (playerCharacter.punch) 
        {
            col = collision;
            var enemyStats = col.gameObject.GetComponent<EnemyStats>();
            if(enemyStats!=null)
            enemyStats.health -= 2;
            if (enemyStats.health <= 0) col = null;
        }
       
    }
    private void OnCollisionExit(Collision collision)
    {
        col = null;
    }
}
