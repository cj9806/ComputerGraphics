using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrophyScript : MonoBehaviour
{
    public GameObject[] enemies;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, .25f, 0));
    }
    private void OnTriggerEnter(Collider other)
    {
        foreach(GameObject enemey in enemies)
        {
            if(enemey)
                return;
        }

        Destroy(this.gameObject);
        other.gameObject.GetComponent<SamplePlayerCharacter>().hasWon = true;
    }
}
