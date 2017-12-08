using UnityEngine;

namespace RUSH
{
    

    public class Teleporter : ActionObject
    {

        public Teleporter otherTeleporter;

        protected override void Start()
        {
            base.Start();
            currentState = ACTION_STATE.TELEPORT;
        }

        // c'est la même chose que les autres ActionObjects sauf qu'au lieu de s'envoyer lui même dans l'event il envoie l'atre teleporteur
        protected override void GiveBehavior(RaycastHit hit)
        {
            ActionObjectsManager.instance.SendActionEvent(hit, otherTeleporter);
        }

    }
}