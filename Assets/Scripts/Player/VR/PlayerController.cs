using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerController : MonoBehaviourPun, IDamageable
{
    [Header("Info")]
    public int id;
    private int curAttackerId;

    [Header("Stats")]
    public float curHP;
    public float maxHP;
    public int kills = 0;
    public bool dead;

    public Weapon grabbedWeapon;

    private bool flashingDamage;

    [Header("Components")]
    //public Rigidbody rig;
    public Player photonPlayer;
    //public PlayerWeapon weapon;
    public Image healthImage;
    public MeshRenderer mr;
    public Canvas deathCanvas;
    public LocomotionSystem locomotionSystem;

    [PunRPC]
    public void Initialize(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;

        // Add to players list
        GameManager.instance.players[id - 1] = this;

        // not local player
        if (!photonView.IsMine)
        {
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
            //rig.isKinematic = true;
        }
        else
        {
            GUI.instance.Initialize(this);
        }
    }

    [PunRPC]
    public void TakeDamage(int attackerId, float damage)
    {
        Debug.Log("Delivering damage");
        if (dead) return;

        curHP -= damage;
        curAttackerId = attackerId;

        // Flash player red
        photonView.RPC("DamageFlash", RpcTarget.Others);

        // Update Health UI
        GUI.instance.UpdateHealthBar();

        // Die if no health
        if (curHP <= 0)
            photonView.RPC("Die", RpcTarget.All);
    }

    [PunRPC]
    private void DamageFlash()
    {
        if (flashingDamage) return;

        StartCoroutine(DamageFlashCoroutine());

        IEnumerator DamageFlashCoroutine()
        {
            flashingDamage = true;

            Color defaultColor = mr.material.color;
            mr.material.color = Color.red;

            yield return new WaitForSeconds(0.05f);

            mr.material.color = defaultColor;
            flashingDamage = false;
        }
    }

    [PunRPC]
    private void Die()
    {
        curHP = 0;
        dead = true;

        GameManager.instance.alivePlayers--;

        // Host checks win
        if (PhotonNetwork.IsMasterClient)
            GameManager.instance.CheckWinCondition();

        // is this us?
        if (photonView.IsMine)
        {
            if (curAttackerId != 0)
                GameManager.instance.GetPlayer(curAttackerId).photonView.RPC("AddKill", RpcTarget.All);

            deathCanvas.gameObject.SetActive(true);
            locomotionSystem.enabled = false;

            // Set cam to spectator
            //GetComponentInChildren<CameraController>().SetAsSpectator();

            // Disable physics, hide player
            //rig.isKinematic = true;
            //transform.position = new Vector3(0, -50, 0);

        }
    }

    [PunRPC]
    public void AddKill()
    {
        kills++;

        // Update UI
        GUI.instance.UpdateInfoText();
    }

    [PunRPC]
    public void Heal(int amount)
    {
        curHP = Mathf.Clamp(curHP + amount, 0, maxHP);

        // Update UI
        GUI.instance.UpdateHealthBar();
    }
}
