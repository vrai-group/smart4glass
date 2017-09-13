using UnityEngine;
using System.Timers;
using UnityEngine.UI;
using Vuforia;
using System.Collections;
using System.Collections.Generic;

public class State_Machine : MonoBehaviour {

    private static Phase[] phaseList;
    private static Timer atimer,stepTimer,uiTimer;
    private static bool currentFound, nextFound, singleFound, //flag
        subStateTerminated;
    private static string[] textMessages = {"1. Take the cone and insert the metal ring","2. Insert the grinding tool","3. Insert the flange and the screw"};
    private const int timerInterval = 20000,stepTimerInterval = 4000,uiTimerInterval = 4000; //timer interval in ms
    public GameObject UIPanel,responsePanel; //Ui panel where the state machine display a message.
    private int touch;
    public Text objectText,responseText;
    private static Text objText;
    private static string[] uiText = {""};
    private const string OBJECT_FOUND = "OBJECT FOUND", OBJECT_NOT_FOUND = "OBJECT NOT FOUND";
    public GameObject[] multipleObjectArray = new GameObject[10];  //global target -- single object
    private static int phaseNum, //current phase number
         subStateNum; //the number of the substate. A phase is a collection of substates.

    private Outline outline;
    private Color colorGreen;
    private Color colorRed;
    private StateManager sm;

    // Use this for initialization
    void Start () {
        Debug.Log("start state machine");

        phaseList = new Phase[3];//initialize the phase list
        for(int i = 0; i < 3; i++)
        {
            phaseList[i] = new Phase(multipleObjectArray[2* i],       //current target
                                    multipleObjectArray[2 * i + 2],  //next target
                                    multipleObjectArray[2 * i + 1]  //single object
                                    );                             
        }

        //Initialize parameters
        phaseNum = 0;
        subStateNum = 0;
        currentFound = false;
        nextFound = false;
        singleFound = false;
        subStateTerminated = true;
        responsePanel.SetActive(false);
        objText = objectText;

        outline = responsePanel.GetComponent<Outline>();

        colorGreen = new Color(25 / 255f, 150 / 255f, 57 / 255f, 128 / 255f);

        colorRed = new Color(201 / 255f, 35 / 255f, 39 / 255f, 128 / 255f);

        sm = TrackerManager.Instance.GetStateManager();
    }
	
	// Update is called once per frame
	void Update () {

        //If the number of the phase is minor of 2, the state machine advance throw the states
        if (phaseNum < 2)
        {
            //If substateTerminated is false the state machine wait the end of the substate
            if (subStateTerminated && subStateNum == 1) //
            {
                responsePanel.SetActive(true);
                responseText.text = "Object found";
                outline.effectColor = Color.green;
                changeTracking();
                uiTimer = new Timer();
                uiTimer.Elapsed += new ElapsedEventHandler(OnUITimedEvent);
                uiTimer.Interval = stepTimerInterval; //ms
                uiTimer.Enabled = true;
                uiTimer.Start();
                execute(subStateNum);
            }//
            else if (Input.GetKeyDown(KeyCode.RightArrow) && subStateNum == 0 && subStateTerminated)
            {
                Vuforia.DefaultTrackableEventHandler.setObjectFound(false);
                execute(subStateNum);
            }
        }        
        else if (phaseNum==2)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Debug.Log("Vado alla fase 2");
                objectText.text = textMessages[phaseNum];
                phaseNum++;
                Debug.Log("mostro l'animaizone della fase " + phaseNum);
                AnimatorController.playAnim = true;
            }
        }
        else if (phaseNum == 3)
        {
            if(Input.GetKeyDown(KeyCode.RightArrow))
            {
                Debug.Log("Vado alla fase 3");
                objectText.text = "Task completed";
            }
            
        }
        
        
        
	}
    
    // this method handles the different substate
    private void execute(int state) {
        subStateTerminated = false;
        Debug.Log("Siamo nella fase " + phaseNum);
        switch(subStateNum)
        {
            case 0: //Stato in cui si deve inquadrare il target iniziale
                Debug.Log("stato " + subStateNum);
                Debug.Log("Inquadra la scena per iniziare");
                AnimatorController.stopAnimations();
                objectText.text = "Look the current object";
                atimer = new Timer();
                atimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                atimer.Interval = timerInterval; //ms
                atimer.Enabled = true;
                atimer.Start();
                break;
                
            case 1: //stato in cui deve inquadrare il target finale
            
                Debug.Log("stato " + subStateNum);
                Debug.Log("Inquadra l'oggeto finale");
                objectText.text = textMessages[phaseNum];
                AnimatorController.playAnim = true;                
                atimer = new Timer();
                atimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                atimer.Interval = timerInterval; //ms
                atimer.Enabled = true;
                atimer.Start();
                break;

            default:
                break;

            
           
        }
        Vuforia.DefaultTrackableEventHandler.setObjectFound(false);


    }

    public static void nextState() //metodo che reinizializza tutti i parametri per il substate successivo
    {
        currentFound = false;
        nextFound = false;
        singleFound = false;
        subStateTerminated = true;
        atimer.Stop();
        subStateNum++;
        if (subStateNum == 2) {            
            phaseNum++;
            if (phaseNum < 2)
            {
                subStateNum = 0;
                objText.text = "Press NEXT to continue";
            }
            else {
                Debug.Log("sono qui");
                objText.text = "Press NEXT to continue";
               // objText.text = "Press NEXT to continue";
            }
        }        
        Debug.Log("Fase " + phaseNum +" Cambiamento di stato a " + subStateNum);

    }

    public static Phase[] getPhaseList()
    {
        return phaseList;
    }

    public static int getSubStateNumber()
    {
        return subStateNum;
    }
   
    public static void setCurrentTargetFlag(bool a)
    {
        currentFound = a;
    }

    public static void setNextTargetFlag(bool a)
    {
        nextFound = a;
    }
    public static void setSingleObjectFlag(bool a)
    {
        singleFound = a;
    }

    public static int getPhaseNumber()
    {
        return phaseNum;
    }

    // Specify what you want to happen when the Elapsed event is raised.
    private void OnTimedEvent(object source, ElapsedEventArgs e)
    {
        Debug.Log("Timer scaduto!");
        atimer.Stop();
        changeTracking();
        switch (subStateNum)
        {            
            case 0:
                Debug.Log("Non riesco ad inquadrare il target di partenza!");
                responsePanel.SetActive(true);
                responseText.text = "Retry, object not found";
                outline.effectColor = Color.red;
                uiTimer = new Timer();
                uiTimer.Elapsed += new ElapsedEventHandler(OnUITimedEvent);
                uiTimer.Interval = uiTimerInterval; //ms
                uiTimer.Enabled = true;
                uiTimer.Start();                
                break;
            case 1:
                Debug.Log("Non riesco ad inquadrare il target finale!");
                if (!singleFound) //non è stato trovato nulla
                {
                    responsePanel.SetActive(true);
                    responseText.text = "Retry, object not found";
                    outline.effectColor = Color.red;
                }
                else //è stato trovato solo l'oggetto singolo
                {
                    responsePanel.SetActive(true);
                    responseText.text = "Object wrong assembled";
                    outline.effectColor = Color.red;
                }
                uiTimer = new Timer();
                uiTimer.Elapsed += new ElapsedEventHandler(OnUITimedEvent);
                uiTimer.Interval = uiTimerInterval; //ms
                uiTimer.Enabled = true;
                uiTimer.Start();

                break;
            default:
                break;
        }
        atimer.Start();
    }

    
        //perdere tracking nel cambio di stato
    private void changeTracking()
    {
        IEnumerable<TrackableBehaviour> activeTrackables = sm.GetActiveTrackableBehaviours();
        foreach (TrackableBehaviour tb in activeTrackables)
        {
            if (tb.isActiveAndEnabled)
            {
                tb.enabled = false;

                tb.enabled = true;
            }
        }
    }

    private void OnUITimedEvent(object source, ElapsedEventArgs e)
    {
        responsePanel.SetActive(false);
        uiTimer.Stop();
        
    }
}
