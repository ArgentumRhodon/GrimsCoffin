using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    public enum EnemyDropType
    {
        Health,
        SpiritPower
    }


    [SerializeField] public EnemyDropType dropType;
    [SerializeField] public float value = 10;

    [SerializeField] private float speed = 15;
    [SerializeField] private float lifetime = 3.5f;
    private float lifetimeTimer;

    [SerializeField] private Color healthColor;
    [SerializeField] private Color spiritPowerColor;

    private bool collected;

    private void Start()
    {
        if (Random.Range(1, 100) <= 50 && PlayerControllerForces.Instance.Data.canScytheThrow && PlayerControllerForces.Instance.currentSP < PlayerControllerForces.Instance.Data.maxSP)
        {
            dropType = EnemyDropType.SpiritPower;
        }

        else if (PlayerControllerForces.Instance.currentHP < PlayerControllerForces.Instance.Data.maxHP)
        {
            dropType = EnemyDropType.Health;
        }

        else
            Destroy(gameObject);

        switch (dropType)
        {
            case EnemyDropType.Health:
                GetComponent<SpriteRenderer>().color = healthColor;
                break;

            case EnemyDropType.SpiritPower:
                GetComponent<SpriteRenderer>().color = spiritPowerColor;
                value = 5;
                break;
        }

        lifetimeTimer = 0;
    }

    private void Update()
    {
        if (collected)
        {
            Debug.Log("Pickup Moving");
            this.transform.position = Vector3.MoveTowards(transform.position, PlayerControllerForces.Instance.transform.position, speed * Time.deltaTime);
        }

        else
        {
            lifetimeTimer += Time.deltaTime;
            if (lifetimeTimer >= lifetime)
                Destroy(gameObject);
        }

        if (transform.position == PlayerControllerForces.Instance.transform.position)
        {
            CollectDrop();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerControllerForces>() != null)
        {
            Debug.Log("Pickup Collected");
            collected = true;
        }
    }

    private void CollectDrop()
    {
        switch (dropType)
        {
            case EnemyDropType.Health:
                PlayerControllerForces.Instance.currentHP += value;
                Mathf.Clamp(PlayerControllerForces.Instance.currentHP, 0, PlayerControllerForces.Instance.Data.maxHP);
                break;

            case EnemyDropType.SpiritPower:
                PlayerControllerForces.Instance.currentSP += value;
                Mathf.Clamp(PlayerControllerForces.Instance.currentSP, 0, PlayerControllerForces.Instance.Data.maxSP);
                break;
        }

        Destroy(gameObject);
    }
}
