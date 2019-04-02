using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthTracker : MonoBehaviour {

    [SerializeField]
    Image _halfHeart;                               //half heart image
    [SerializeField]
    Image _fullHeart;                               //full heart image
    [SerializeField]
    Image _rim;                                     //rim image
    [SerializeField]
    Image _helmet;                                  //helmet image

    public void DepleteHealth(float _newHealth)
    {

        if (_newHealth % 1 >= 0 && _newHealth % 1 < .25f)        //if heart is quarter full
        {
            _helmet.enabled = false;
            _rim.enabled = false;
            _fullHeart.enabled = false;
            _halfHeart.enabled = false;
        }
        else if (_newHealth % 1 >= .25f && _newHealth % 1 < .5f) //if heart is half full
        {
            _helmet.enabled = false;
            _rim.enabled = false;
            _fullHeart.enabled = false;
        }
        else if (_newHealth % 1 >= .5f && _newHealth % 1 < .75f) //if heart is 3/4 full
        {
            _helmet.enabled = false;
            _rim.enabled = false;
        }
        else if (_newHealth % 1 >= .75f)                         //if heart is full
        {
            _helmet.enabled = false;
        }

    }

    public void RegainHealth(float _newHealth)
    {
        if (_newHealth % 1 <= .25f)                              //if heart is empty
        {
            _halfHeart.enabled = true;
        }
        else if (_newHealth % 1 > .25f && _newHealth % 1 < .5f) //if heart is quarter full
        {
            _fullHeart.enabled = true;
        }
        else if (_newHealth % 1 > .5f && _newHealth % 1 <= .75f) //if heart is half full
        {
            _fullHeart.enabled = true;
            _rim.enabled = true;
        }
        else if (_newHealth % 1 > .75f)                          //if heart is 3/4 full
        {
            _fullHeart.enabled = true;
            _rim.enabled = true;
            _helmet.enabled = true;
        }
        else if (_newHealth % 1 == 0)                           //if heart is full
        {
                _halfHeart.enabled = true;
                _fullHeart.enabled = true;
                _rim.enabled = true;
                _helmet.enabled = true;
        }
    }

    //full reset of heart
    public void ResetHealth()
    {
        _halfHeart.enabled = true;
        _fullHeart.enabled = true;
        _rim.enabled = true;
        _helmet.enabled = true;
    }

    //empty reset of heart
    public void EmptyHealth()
    {
        _halfHeart.enabled = false;
        _fullHeart.enabled = false;
        _rim.enabled = false;
        _helmet.enabled = false;
    }

}
