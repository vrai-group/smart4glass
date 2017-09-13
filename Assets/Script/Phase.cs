using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase {
    /*
     *This class contains the three phase objects 
     */
    
    private GameObject currentTarget; //current object
    private GameObject nextTarget; //the final object
    private GameObject singleObjectTarget; //The single object
    


    public Phase(GameObject curr,GameObject next,GameObject singleObj)
    {
        
        currentTarget = curr;
        nextTarget = next;
        singleObjectTarget = singleObj;        
    }


    public GameObject getCurrTarget()
    {
        return currentTarget;
    }

    public GameObject getNextTarget()
    {
        return nextTarget;
    }

    public GameObject getSingleObject()
    {
        return singleObjectTarget;
    }
}
