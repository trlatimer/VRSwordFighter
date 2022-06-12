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

    private PhotonView photonView;
    private PlayerController currPlayer;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    // When grabbed, we need to tell the PlayerController that we have a weapon so we can shoot
    public void SetWeaponOwner(SelectEnterEventArgs selectEnterEventArgs)
    {
        PlayerController interactorController = selectEnterEventArgs.interactorObject.transform.gameObject.GetComponentInParent<PlayerController>();
        if (interactorController != null)
        {
            currPlayer = interactorController;
            interactorController.grabbedWeapon = this;
        }
    }

    // When dropped, we need to clear the weapon from the PlayerController so we don't continue shooting
    public void ReleaseWeaponOwner(SelectExitEventArgs selectExitEventArgs)
    {
        PlayerController interactorController = selectExitEventArgs.interactorObject.transform.gameObject.GetComponentInParent<PlayerController>();
        if (interactorController != null)
        {
            photonView = null;
            interactorController.grabbedWeapon = null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision with: " + collision.gameObject.name);
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            photonView.RPC("DeliverDamage", collision.gameObject.GetComponent<PhotonView>().Owner, currPlayer.photonPlayer.ActorNumber, collision.gameObject, damage);
                
        }
    }

    [PunRPC]
    private void DeliverDamage(int attackerId, GameObject receiver, float damage)
    {
        // deliver damage to receiver
        PlayerController controller = receiver.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.TakeDamage(attackerId, damage);
        }
    }
}
