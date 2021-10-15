using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; 

public class CombatControls : MonoBehaviour
{
    [SerializeField] GameObject fist;
    [SerializeField] SamplePlayerCharacter playerCharacter;
    private GameObject activeWeapon;
    private GameObject collide;
    public HandCollisionDetector hCD;
    // Start is called before the first frame update
    void Start()
    {
        //set default active weapon = fists
        activeWeapon = fist;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerCharacter.attack == true && hCD.col != null)
        {
            //look for collision
            Debug.Log(hCD.col.gameObject.name);
            //hCD.col.gameObject.GetComponent<EnemyStats>().health -= 2;
        }
    }
    
}
