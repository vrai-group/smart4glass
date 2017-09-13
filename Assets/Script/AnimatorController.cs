using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimatorController : MonoBehaviour {

    public static Animator anim;
    public GameObject[] gameobjs = new GameObject[5];
    private static string element;
    public static bool playAnim;
   

    // Use this for initialization
    void Start() {
        disableObjects();
        anim = GetComponent<Animator>();
        playAnim = false;
    }

    // Update is called once per frame
    void Update() {
        

        if (playAnim) {


            //gli oggetti sono stati inseriti in ordine inverso
            switch (State_Machine.getPhaseNumber())
            {
                case 0://Cone and metal ring animations
                   
                    if (!gameobjs[4].activeSelf)
                    {
                        gameobjs[4].SetActive(true);
                        gameobjs[3].SetActive(true);
                    }
                    anim.Play("pezzo4-5", -1, 0f);
                  
                    break;
                case 1://Grinding tool animation
                    if (!gameobjs[2].activeSelf) {
                    gameobjs[2].SetActive(true);
                    }
                    anim.Play("pezzo3", -1, 0f);
                   
                    break;
                case 3://Flange and screw animations
                    if (!gameobjs[1].activeSelf)
                    {
                        gameobjs[1].SetActive(true);
                        gameobjs[0].SetActive(true);
                    }
                    anim.Play("pezzo1-2", -1, 0f);
                   // Debug.Log(gameobjs[1].name);
                    //.Play("pezzo1", -1, 0f);
                   // Debug.Log(gameobjs[0].name);
                    break;
            }
            playAnim = false;

        }
        else
        {
           //anim.Play("idle", -1, 0f);
        }
        


    }

    public static void setElement(string elem) {
        element = elem;
    }

    //Play the "idle" animation 
    public static void stopAnimations()
    {
        anim.Play("idle", -1, 0f);
    }

    //Hide all objects
    private void disableObjects()
    {
        for (int i = 0; i < 5; i++)
        {
            gameobjs[i].SetActive(false);
        }
    }
}
