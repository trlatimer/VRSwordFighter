using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkGrabbing : MonoBehaviourPunCallbacks, IPunOwnershipCallbacks
{
    PhotonView m_photonView;
    Rigidbody rig;
    bool isBeingHeld = false;

    private void Awake()
    {
        m_photonView = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isBeingHeld)
        {
            // Object is being grapped
            rig.isKinematic = true;
        }
        else
        {
            rig.isKinematic = false;
        }
    }

    private void TransferOwnership()
    {
        m_photonView.RequestOwnership();  
    }

    public void OnSelectEntered()
    {
        m_photonView.RPC("StartNetworkGrabbing", RpcTarget.AllBuffered);

        if (m_photonView.Owner != PhotonNetwork.LocalPlayer)
            TransferOwnership();
    }

    public void OnSelectExited()
    {
        m_photonView.RPC("StopNetworkGrabbing", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void StartNetworkGrabbing()
    {
        isBeingHeld = true;
    }

    [PunRPC]
    public void StopNetworkGrabbing()
    {
        isBeingHeld = false;
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        if (targetView != m_photonView) return;

        Debug.Log("Ownership requested for: " + targetView.name + " from " + requestingPlayer.NickName);
        m_photonView.TransferOwnership(requestingPlayer);
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        Debug.Log("Onwership transferred to " + targetView.name + " from " + previousOwner.NickName);
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
        
    }
}
