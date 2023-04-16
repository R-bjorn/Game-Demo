using System.Collections;
using System.Collections.Generic;
using Game_Manager.Mind.Game_Scripts;
using UnityEngine;
using Unity.Netcode;

public class NetworkHealthPack : NetworkBehaviour
{
    public static HealthPack Instance { get; private set; }
    public int healthPackValue = 50;

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
        if (other.CompareTag("Player"))
        {
            Debug.Log("Contact");
            NetworkPlayerStats player = other.GetComponent<NetworkPlayerStats>();
            if (player != null)
            {
                Debug.Log("Heal");
                player.Heal(healthPackValue);
                Destroy(gameObject);
            }
            else
            {

            }
        }
        else
        {

        }
    }
}
