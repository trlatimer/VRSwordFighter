using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    [Header("Buttons")]
    public Button CloseMenuButton;
    public Button ExitGameButton;
    public Button LeaveGameButton;

    public void OpenGameMenu()
    {
        this.gameObject.SetActive(true);
    }

    public void CloseGameMenu()
    {
        this.gameObject.SetActive(false);
    }

    #region Button Events
    public void CloseMenuButton_Click()
    {
        CloseGameMenu();
    }

    public void ExitGameButton_Click()
    {
        //PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
        NetworkManager.instance.LeaveRoom();
        Application.Quit();
    }

    public void LeaveGameButton_Click()
    {
        //PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
        NetworkManager.instance.LeaveRoom();
        NetworkManager.instance.ChangeScene("Main Scene");
    }
    #endregion
}
