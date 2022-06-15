using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI : MonoBehaviour
{
    public static GUI instance;

    public TMPro.TextMeshProUGUI killText;
    public TMPro.TextMeshProUGUI playerText;
    public Image healthBar;

    public PlayerController playerController;

    private void Awake()
    {
        instance = this;
    }

    public void Initialize(PlayerController controller)
    {
        Debug.Log("Initializing GUI with: " + controller.photonPlayer.NickName);
        playerController = controller;
        UpdateHealthBar();
        UpdateInfoText();
    }

    public void UpdateHealthBar()
    {
        healthBar.fillAmount = playerController.curHP / playerController.maxHP;
    }

    public void UpdateInfoText()
    {
        killText.text = $"{playerController.kills} kills";
        playerText.text = $"{GameManager.instance.players.Length} players";
    }
}
