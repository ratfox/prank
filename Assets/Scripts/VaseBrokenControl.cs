using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaseBrokenControl : MonoBehaviour
{
    public GameObject player = null;
    float timeSinceExists;
    int TIME_UNTIL_DISAPPEAR = 4;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        timeSinceExists = Time.time;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.time - timeSinceExists > TIME_UNTIL_DISAPPEAR) {
            player.GetComponent<PlayerControl>().item_exists = false;
            Destroy(this.gameObject);
        }
    }
}
