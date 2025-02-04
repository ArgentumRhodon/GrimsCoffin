using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.Rendering;
using FMOD.Studio;
//using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using System;
using Unity.VisualScripting;
public class FMODGlobalParameterTester : MonoBehaviour
{
    public enum groundNamesEnum
    {
        Stone,
        Snow,
        Soil,
        Equi,
    }

    public enum levelNamesEnum
    {
        Tutorial,
        Denial,
        Menu
    }



    [Tooltip("FMOD events for the map music")]
    [SerializeField] public EventReference mapMXTutorial;
    [SerializeField] public EventReference mapMXDenial;
    [SerializeField] public EventReference mapMXMenu;
    protected EventInstance MXInstance;


    [Tooltip("FMOD events for the map amb")]
    [SerializeField] public EventReference mapAmbStoneSFX;
    [SerializeField] public EventReference mapAmbSnowSFX;
    [SerializeField] public EventReference mapAmbSoilSFX;
    protected EventInstance mapAmbInstance;

    [Tooltip("Select the parameter you want to ground switch controller")]
    [FMODUnity.ParamRef]
    [SerializeField] public string StandingGroundPram;
    [Tooltip("Selection of available ground names")]
    [SerializeField] public groundNamesEnum groundName;
    private groundNamesEnum initialGround;
    [Tooltip("Selection of available ground names")]
    [SerializeField] public levelNamesEnum levelName;
    private levelNamesEnum initialLevel;

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

    void Awake()
    {
        //Debug.Log("Current Ground Name is: " + (float)groundName);
        
        //Debug.Log("initialGround is: " + initialGround);
       //Debug.Log("groundName is: " + groundName);

        switch ((float)groundName)
        {
            case 0:
                mapAmbInstance = RuntimeManager.CreateInstance(mapAmbStoneSFX);
                break;
            case 1:
                mapAmbInstance = RuntimeManager.CreateInstance(mapAmbSnowSFX);
                break;
            case 2:
                mapAmbInstance = RuntimeManager.CreateInstance(mapAmbSoilSFX);
                break;
            default:
                mapAmbInstance = RuntimeManager.CreateInstance(mapAmbStoneSFX);
                break;
        }
        switch ((float)levelName)
        {
            case 0:
                MXInstance = RuntimeManager.CreateInstance(mapMXTutorial);
                break;
            case 1:
                MXInstance = RuntimeManager.CreateInstance(mapMXDenial);
                break;
            case 2:
                MXInstance = RuntimeManager.CreateInstance(mapMXMenu);
                break;
            default:
                MXInstance = RuntimeManager.CreateInstance(mapMXMenu);
                break;
        }
        initialGround = groundName;
        initialLevel = levelName;

    }
    void Start()
    {
        //initialGround = groundName;
        MXInstance.start();
        mapAmbInstance.start();
    }

    // Update is called once per frame
    void Update()
    {
        RuntimeManager.StudioSystem.setParameterByName(valueA, sliderValueA);
        RuntimeManager.StudioSystem.setParameterByName(valueB, sliderValueB);
        RuntimeManager.StudioSystem.setParameterByName(valueC, sliderValueC);
        RuntimeManager.StudioSystem.setParameterByName(StandingGroundPram, (float)groundName);
        //RuntimeManager.StudioSystem.setParameterByName()
        if ((float)initialGround != (float)groundName)
        {
            mapAmbInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            mapAmbInstance = new EventInstance();
            switch ((float)groundName)
            {
                case 0:
                    mapAmbInstance = RuntimeManager.CreateInstance(mapAmbStoneSFX);
                    break;
                case 1:
                    mapAmbInstance = RuntimeManager.CreateInstance(mapAmbSnowSFX);
                    break;
                case 2:
                    mapAmbInstance = RuntimeManager.CreateInstance(mapAmbSoilSFX);
                    break;
                default:
                    mapAmbInstance = RuntimeManager.CreateInstance(mapAmbStoneSFX);
                    break;
            }
            mapAmbInstance.start();
            initialGround = groundName;
        }


        if ((float)initialLevel != (float)levelName)
        {
            MXInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            MXInstance = new EventInstance();
            switch ((float)levelName)
            {
                case 0:
                    MXInstance = RuntimeManager.CreateInstance(mapMXTutorial);
                    break;
                case 1:
                    MXInstance = RuntimeManager.CreateInstance(mapMXDenial);
                    break;
                case 2:
                    MXInstance = RuntimeManager.CreateInstance(mapMXMenu);
                    break;
                default:
                    MXInstance = RuntimeManager.CreateInstance(mapMXMenu);
                    break;
            }
            MXInstance.start();
            initialLevel = levelName;
        }
    }

    private void OnDestroy()
    {
        mapAmbInstance.release();
        mapAmbInstance.clearHandle();
        MXInstance.release();
        MXInstance.clearHandle();
    }
}
