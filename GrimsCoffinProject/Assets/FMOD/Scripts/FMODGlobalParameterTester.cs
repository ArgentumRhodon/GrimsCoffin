using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.Rendering;
using FMOD.Studio;
public class FMODGlobalParameterTester : MonoBehaviour
{
    #region valueA
    [Range(0f, 1f)]
    [Tooltip("This slide controls the value A in the FMOD")]
    public float sliderValueA = 0.5f;
    [Tooltip("Input the name and the id of value A you want")]
    [FMODUnity.ParamRef]
    public string valueA;
    [Space(10)]
    #endregion

    #region valueB
    [Range(0f, 1f)]
    [Tooltip("This slide controls the value B in the FMOD")]
    public float sliderValueB = 0.5f;
    [Tooltip("Input the name and the id of value B you want")]
    [FMODUnity.ParamRef]
    public string valueB;
    [Space(10)]
    #endregion

    #region valueC
    [Range(0f, 1f)]
    [Tooltip("This slide controls the value C in the FMOD")]
    public float sliderValueC = 0.5f;
    [Tooltip("Input the name and the id of value C you want")]
    [FMODUnity.ParamRef]
    public string valueC;
    #endregion


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RuntimeManager.StudioSystem.setParameterByName(valueA, sliderValueA);
        //RuntimeManager.StudioSystem.setParameterByName()
    }
}
