using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PM_Teleporter : MonoBehaviour
{
    [SerializeField]
    bool _leftRight;
    [SerializeField]
    bool _upDown;
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    
    public void TriggerTeleport(Collider other)
    {
        if (other.GetComponent<PlayerController>().transform || other.GetComponent<PM_BasicGhlost>())
        {
            //if (_upDown)
            //{

            //}
            //else if (_leftRight)
            //{
                transform.parent.GetComponent<PM_Manager>().teleportObject(other.gameObject, this.gameObject);
            //}
            
        }
    }
    
}
