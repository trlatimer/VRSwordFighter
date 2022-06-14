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

    private PlayerController playerController;

    private void Start()
    {
        instance = this;
    }

    public void Initialize(PlayerController controller)
    {
        playerController = controller;
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
