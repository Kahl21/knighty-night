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
        _otherCheckpoints.Remove(this);
    }

    public void CheckPointHit()
    {
        _myCollider.enabled = false;
        for (int i = 0; i < _myListOfEnemies.Count; i++)
        {
            _myListOfEnemies[i].GetComponent<GraveyardGlhost>().ColorChange(_GhlostColorChange);
        }

        for (int i = 0; i < _otherCheckpoints.Count; i++)
        {
            _otherCheckpoints[i].MyReset();
        }
    }

    public void MyReset()
    {
        _myCollider.enabled = true;
    }
}
