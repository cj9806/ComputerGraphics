using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    public EnemyStats[] enemies;
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
            foreach(EnemyStats enemy in enemies)
            {
                enemy.onTheHunt = true;
            }
        }
    }
}
