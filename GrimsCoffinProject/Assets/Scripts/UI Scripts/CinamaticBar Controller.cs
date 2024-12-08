using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinamaticBarController : MonoBehaviour
{
    public static CinamaticBarController Instance { get; private set; }
    
    [SerializeField] private GameObject BarcontainerGO;
    [SerializeField] private Animator BarAnimator;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }
    public void ShowBars()
    {
        BarcontainerGO.SetActive(true);
    }

    public void HideBars() 
    {
        if (BarcontainerGO.activeSelf)
        {
            StartCoroutine(HideBarsAndDisableGO());
        }

    }
    public IEnumerator HideBarsAndDisableGO()
    {
        BarAnimator.SetTrigger("HideBar");
        yield return new WaitForSeconds(0.5f);
        BarcontainerGO.SetActive(false);
    }
}
