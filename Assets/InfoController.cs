using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoController : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] EnemyStats stats;
    [SerializeField] Slider healthbar;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target);
        transform.Rotate(0, 180, 0);
        healthbar.value = stats.health;
    }
}
