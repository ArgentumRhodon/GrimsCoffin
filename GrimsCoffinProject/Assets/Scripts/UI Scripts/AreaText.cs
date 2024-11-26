using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AreaText : MonoBehaviour
{
    [SerializeField] string areaName;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<TextMeshProUGUI>().text = areaName;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !this.GetComponent<Animator>().IsInTransition(0))
        {
            Destroy(this.gameObject);
        }
    }
}
