using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType
{
    Pot1 = 0,
    Pot2 = 1,
    Snow = 2,
    Bones1 = 3,
    Bones2 = 4
}

public class BreakableObject : MonoBehaviour
{
    [SerializeField] private List<Sprite> idleSprites;
    [SerializeField] private ObjectType objectType;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isBroken;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
        animator.SetInteger("BreakableObject", (int)objectType);
    }

    // Update is called once per frame
    void Update()
    {
        if (isBroken)
            timer -= Time.deltaTime;
        if (timer <= 0 && isBroken)
            Destroy(gameObject);
    }

    public void TakeDamage()
    {
        //Set animator conditional to play animation
        isBroken = true;
        animator.SetBool("IsBroken", isBroken);
        timer = 1;

        if (objectType == ObjectType.Bones1 || objectType == ObjectType.Bones2)
            PersistentDataManager.Instance.UpdateEnemyCurrency(Random.Range(5, 20), true);
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
