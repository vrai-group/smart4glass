/*==============================================================================
Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using UnityEngine;

namespace Vuforia
{
    /// <summary>
    /// A custom handler that implements the ITrackableEventHandler interface.
    /// </summary>
    public class DefaultTrackableEventHandler : MonoBehaviour,
                                                ITrackableEventHandler
    {
        #region PRIVATE_MEMBER_VARIABLES
 
        private TrackableBehaviour mTrackableBehaviour;
        private static bool objectFound;
      


        #endregion // PRIVATE_MEMBER_VARIABLES



        #region UNTIY_MONOBEHAVIOUR_METHODS

        void Start()
        {
            mTrackableBehaviour = GetComponent<TrackableBehaviour>();
            if (mTrackableBehaviour)
            {
                mTrackableBehaviour.RegisterTrackableEventHandler(this);
            }
            objectFound = true;
            

        }
        

        #endregion // UNTIY_MONOBEHAVIOUR_METHODS



        #region PUBLIC_METHODS




        /// <summary>
        /// Implementation of the ITrackableEventHandler function called when the
        /// tracking state changes.
        /// </summary>
        public void OnTrackableStateChanged(
                                        TrackableBehaviour.Status previousStatus,
                                        TrackableBehaviour.Status newStatus)
        {
            if (newStatus == TrackableBehaviour.Status.DETECTED ||
                newStatus == TrackableBehaviour.Status.TRACKED ||
                newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
            {
                OnTrackingFound();
            }
            else
            {
                OnTrackingLost();
            }
        }

        #endregion // PUBLIC_METHODS



        #region PRIVATE_METHODS


        private void OnTrackingFound()
        {
            Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
            Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);
            
            // Enable rendering:
            foreach (Renderer component in rendererComponents)
            {
                component.enabled = true;
            }

            // Enable colliders:
            foreach (Collider component in colliderComponents)
            {
                component.enabled = true;
            }
           

            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
           
            
            if (!objectFound)// se ha trovato l'oggetto deve aspettare la transizione nello stato successivo
            {
                Debug.Log("Sto ricercando il pezzo dello stato " + State_Machine.getSubStateNumber());
                switch (State_Machine.getSubStateNumber())
                {
                    case 0: //Ricerca dell'obiettivo corrente
                        if (mTrackableBehaviour.name.Equals(State_Machine.getPhaseList()[State_Machine.getPhaseNumber()].getCurrTarget().name))
                        { //controllo se ho trovato il target corrente della fase
                            State_Machine.setCurrentTargetFlag(true);
                            objectFound = true;
                            State_Machine.nextState();
                        }

                        break;
                 
                    case 1: //Ricerca dell'obiettivo finale
                        if (mTrackableBehaviour.name.Equals(State_Machine.getPhaseList()[State_Machine.getPhaseNumber()].getNextTarget().name)) //controllo se lo ho trovato il target finale della fase
                        {
                            State_Machine.setNextTargetFlag(true);
                            objectFound = true;
                            State_Machine.nextState();
                        }
                        if (mTrackableBehaviour.name.Equals(State_Machine.getPhaseList()[State_Machine.getPhaseNumber()].getSingleObject().name)) //controllo se lo ho trovato l'oggetto singolo della fase
                        {
                            State_Machine.setSingleObjectFlag(true);
                        }
                        break;
                    default: //default
                        break;

                }
            }
            

           
        }


        private void OnTrackingLost()
        {
            Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
            Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

            // Disable rendering:
            foreach (Renderer component in rendererComponents)
            {
                component.enabled = false;
            }

            // Disable colliders:
            foreach (Collider component in colliderComponents)
            {
                component.enabled = false;
            }

            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
           
        }

        #endregion // PRIVATE_METHODS
        public static void setObjectFound(bool a)
        {
            objectFound = a;
        }
    }
}
