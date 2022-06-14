using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.XR.Interaction.Toolkit;

public class Weapon : MonoBehaviour
{
    [Header("Stats")]
    public int damage;

    public PhotonView photonView;
    public PlayerController currPlayer;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    // When grabbed, we need to tell the PlayerController that we have a weapon so we can shoot
    public void SetWeaponOwner(SelectEnterEventArgs selectEnterEventArgs)
    {
        PlayerController interactorController = selectEnterEventArgs.interactorObject.transform.root.GetComponent<PlayerController>();
        if (interactorController != null)
        {
            currPlayer = interactorController;
            interactorController.grabbedWeapon = this;
        }
    }

    // When dropped, we need to clear the weapon from the PlayerController so we don't continue shooting
    public void ReleaseWeaponOwner(SelectExitEventArgs selectExitEventArgs)
    {
        PlayerController interactorController = selectExitEventArgs.interactorObject.transform.root.GetComponent<PlayerController>();
        if (interactorController != null)
        {
            currPlayer = null;
            interactorController.grabbedWeapon = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger with " + other.name);
        if (other.transform.root.GetComponent<PlayerController>() != null)
            photonView.RPC("DeliverDamage", other.transform.root.GetComponent<PhotonView>().Controller, photonView.ControllerActorNr, other.gameObject, damage);
    }

    [PunRPC]
    private void DeliverDamage(int attackerId, GameObject receiver, float damage)
    {
        // deliver damage to receiver
        PlayerController controller = receiver.transform.root.GetComponent<PlayerController>();
        if (controller != null)
        {
            Debug.Log("Found controller, sending damage");
            controller.TakeDamage(attackerId, damage);
        }
        else
        {
            Debug.Log("No controller found");
        }
    }
}
