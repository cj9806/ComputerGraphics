using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    public GameObject[] enemies;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "Player")
        {
            foreach(GameObject enemy in enemies)
            {
                EnemyStats enemyScript = enemy.GetComponent<EnemyStats>();
                enemyScript.onTheHunt = true;
            }
        }
    }
}
