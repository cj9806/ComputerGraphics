using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissleScript : MonoBehaviour
{
    public float lifeTime;
    public float velocity;
    public float damage;
    public LayerMask collisonMask;

    private float timer = 0;
    private bool isUsed;

    // Update is called once per frame
    void Update()
    {
        this.transform.position += transform.up * velocity * Time.deltaTime;
        timer += Time.deltaTime;
        if (timer >= lifeTime) Destroy(this.gameObject);

    }
    private void OnCollisionEnter(Collision collision)
    {
        if(isUsed) { return; }

        Debug.Log("hit: " + collision.collider.name);
        // early exit - we are already destroyed

        EnemyStats eStats = collision.gameObject.GetComponentInParent<EnemyStats>();
        if (eStats)
            eStats.health -= damage;
        isUsed = true;
        Destroy(this.gameObject);
    }
}
