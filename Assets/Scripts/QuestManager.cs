using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager
{
    public Trigger trigger;
    public int questNumber = 0;

    public void Update()
    {
        if (trigger.questNumber == 2)
        {
            Debug.Log("You have completed the quest!");
        }
    }

}
