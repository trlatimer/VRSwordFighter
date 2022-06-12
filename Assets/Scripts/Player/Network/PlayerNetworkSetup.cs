using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class PlayerNetworkSetup : MonoBehaviourPunCallbacks
{
    public GameObject localXRRigGameObject;
    public GameObject AvatarHeadGameObject;
    public GameObject AvatarBodyGameObject;
    public GameObject[] AvatarModelPrefabs;
    public GameObject MainAvatarGameObject;

    public TextMeshProUGUI playerNameText;

    void Start()
    {
        if (photonView.IsMine) // Is this me?
        {
            // The player is me
            localXRRigGameObject.SetActive(true);

            //// Get avatar selection data
            //object avatarSelectionNumber;
            //if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AVATAR_SELECTION_NUMBER, out avatarSelectionNumber))
            //{
            //    Debug.Log("Avatar selcetion: " + (int) avatarSelectionNumber);
            //    photonView.RPC("InitializeSelectedAvatarModel", RpcTarget.AllBuffered, (int) avatarSelectionNumber);
            //}

            SetLayerRecursively(AvatarBodyGameObject, 6);
            SetLayerRecursively(AvatarHeadGameObject, 6);

            //TeleportationArea[] teleportationAreas = GameObject.FindObjectsOfType<TeleportationArea>();
            //if (teleportationAreas.Length > 0)
            //{
            //    foreach (var item in teleportationAreas)
            //    {
            //        item.teleportationProvider = localXRRigGameObject.GetComponent<TeleportationProvider>();
            //    }
            //}

            MainAvatarGameObject.AddComponent<AudioListener>();
        }
        else
        {
            // Player is remote
            localXRRigGameObject.SetActive(false);

            SetLayerRecursively(AvatarBodyGameObject, 0);
            SetLayerRecursively(AvatarHeadGameObject, 0);
        }

        if (playerNameText != null)
        {
            playerNameText.text = photonView.Owner.NickName;
        }
    }

    void Update()
    {
        
    }

    void SetLayerRecursively(GameObject go, int layerNumber)
    {
        if (go == null) return;
        foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = layerNumber;
        }
    }

    //[PunRPC]
    //public void InitializeSelectedAvatarModel(int avatarSelectionNumber)
    //{
    //    GameObject selectedAvatarGameobject = Instantiate(AvatarModelPrefabs[avatarSelectionNumber], localXRRigGameObject.transform);

    //    AvatarInputConverter avatarInputConverter = localXRRigGameObject.GetComponent<AvatarInputConverter>();
    //    AvatarHolder avatarHolder = selectedAvatarGameobject.GetComponent<AvatarHolder>();
    //    SetUpAvatarGameobject(avatarHolder.HeadTransform, avatarInputConverter.AvatarHead);
    //    SetUpAvatarGameobject(avatarHolder.BodyTransform, avatarInputConverter.AvatarBody);
    //    SetUpAvatarGameobject(avatarHolder.HandLeftTransform, avatarInputConverter.AvatarHand_Left);
    //    SetUpAvatarGameobject(avatarHolder.HandRightTransform, avatarInputConverter.AvatarHand_Right);
    //}

    void SetUpAvatarGameobject(Transform avatarModelTransform, Transform mainAvatarTransform)
    {
        avatarModelTransform.SetParent(mainAvatarTransform);
        avatarModelTransform.localPosition = Vector3.zero;
        avatarModelTransform.localRotation = Quaternion.identity;
    }
}
