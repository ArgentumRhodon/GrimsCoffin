using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class DistanceTester : MonoBehaviour
{

    private Transform object1;
    [SerializeField] private Transform object2;
    [SerializeField] private EventReference object1SFX;
    private float distance;
    private EventInstance object1Instance;
    private float instanceDistance;

    private void Start()
    {
        object1 = this.transform;
        object1Instance = RuntimeManager.CreateInstance(object1SFX);
        
    }

    private void OnEnable()
    {
        object1Instance.start();
    }

    private void OnDisable()
    {
        object1Instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
    void Update()
    {
        //Debug.Log("Distance: " + 0);
        if (object1 != null && object2 != null)
        {
            distance = Vector3.Distance(object1.position, object2.position);
            //Debug.Log("Distance: " + distance);
        }
        object1Instance.setParameterByName("LocalDistance", distance);
        float position = object1.position.x - object2.position.x;

        if(position < 0) {

            object1Instance.setParameterByName("LocalDirection", -distance/2);
        }
        else
        {
            object1Instance.setParameterByName("LocalDirection", distance/2);
        }

        

    }
}