using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public int questNumber = 0;
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ParkingSpace")
        {
            Debug.Log("You have parked in the correct space!");
            questNumber++;
        }
    }
    public void Update()
    {
        if (questNumber == 2)
        {
            Debug.Log("You have completed the quest!");
            questNumber = 0;
        }
    }
}
