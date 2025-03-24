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
    [SerializeField] public float value = 5;
    [SerializeField] private float dropScalar = 5;

    [SerializeField] private float speed = 15;
    [SerializeField] private float lifetime = 3.5f;
    private float lifetimeTimer;

    [SerializeField] private Sprite healthSprite;
    [SerializeField] private Sprite spiritPowerSprite;

    private bool collected;

    private void Start()
    {
        float maxHP = PlayerControllerForces.Instance.Data.maxHP;
        float currentHP = PlayerControllerForces.Instance.currentHP;
        float maxSP = PlayerControllerForces.Instance.Data.maxSP;
        float currentSP = PlayerControllerForces.Instance.currentSP;

        //Check for HP drop chance
        //Player is more likely to get a HP drop the lower the ratio between their max and current, multiplied by some scalar

        //Example: 50 + (50/10) * 5 = 75, 75% chance for health pickup at 10 HP when your max is 50 (higher if max is higher or current is lower) 
        if (Random.Range(1, 100) <= (50 + (maxHP / currentHP) * dropScalar) && currentHP < maxHP)
        {
            dropType = EnemyDropType.Health;
        }

        //Check for SP drop chance
        //Player is more likely to get a SP drop the lower the ratio between their max and current, multiplied by some scalar
        else if (Random.Range(1, 100) <= (50 + maxSP / currentSP) * dropScalar && PlayerControllerForces.Instance.Data.canScytheThrow && currentSP < maxSP)
        {
            dropType = EnemyDropType.SpiritPower;
        }

        else
            Destroy(gameObject);

        switch (dropType)
        {
            case EnemyDropType.Health:
                GetComponent<SpriteRenderer>().sprite = healthSprite;
                break;

            case EnemyDropType.SpiritPower:
                GetComponent<SpriteRenderer>().sprite = spiritPowerSprite;
                value = 5;
                break;
        }

        lifetimeTimer = 0;
    }

    private void Update()
    {
        if (collected)
        {
            this.transform.position = Vector3.MoveTowards(transform.position, PlayerControllerForces.Instance.transform.position, speed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(transform.forward, transform.position - PlayerControllerForces.Instance.transform.position);
            Debug.Log("Pickup Moving");
        }

        else
        {
            lifetimeTimer += Time.deltaTime;
            if (lifetimeTimer >= lifetime)
                Destroy(gameObject);
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

    public void CollectDrop()
    {
        switch (dropType)
        {
            case EnemyDropType.Health:
                PlayerControllerForces.Instance.currentHP += value;
                PlayerControllerForces.Instance.currentHP = Mathf.Clamp(PlayerControllerForces.Instance.currentHP, 0, PlayerControllerForces.Instance.Data.maxHP);
                break;

            case EnemyDropType.SpiritPower:
                PlayerControllerForces.Instance.currentSP += value;
                PlayerControllerForces.Instance.currentSP = Mathf.Clamp(PlayerControllerForces.Instance.currentSP, 0, PlayerControllerForces.Instance.Data.maxSP);
                break;
        }

        Destroy(gameObject);
    }
}
