using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCheckpoint : MonoBehaviour {

    BoxCollider _myCollider;
    DungeonMechanic _myRoom;
    List<BaseEnemy> _myListOfEnemies;
    [SerializeField]
    Color _GhlostColorChange;
    List<MazeCheckpoint> _otherCheckpoints;

    public void Init(DungeonMechanic myroom, List<BaseEnemy> enemyListRef, List<MazeCheckpoint> checkpoints)
    {
        _myCollider = GetComponent<BoxCollider>();
        _myRoom = myroom;
        _myListOfEnemies = enemyListRef;
        _otherCheckpoints = checkpoints;
    }

    public void CheckPointHit()
    {
        _myCollider = GetComponent<BoxCollider>();
        _myCollider.enabled = false;
        for (int i = 0; i < _myRoom.GetCurrEnemyList.Count; i++)
        {
            _myRoom.GetCurrEnemyList[i].GetComponent<GraveyardGlhost>().ColorChange(_GhlostColorChange);
        }

        for (int i = 0; i < _otherCheckpoints.Count; i++)
        {
            if(_otherCheckpoints[i] != this)
            {
                _otherCheckpoints[i].MyReset();
            }
        }
    }

    public void MyReset()
    {
        _myCollider = GetComponent<BoxCollider>();
        _myCollider.enabled = true;
    }
}
