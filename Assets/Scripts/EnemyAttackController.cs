using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackController : MonoBehaviour
{
    public EnemyStats enemy;
    SamplePlayerCharacter player;
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
        if (enemy.attacking)
        {
            player = collision.gameObject.GetComponentInParent<SamplePlayerCharacter>();
            Debug.Log(player.gameObject.name);
            player.health -= 10;
            enemy.attacking = false;
        }
    }
}
