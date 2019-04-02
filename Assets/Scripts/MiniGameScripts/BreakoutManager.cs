using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakoutManager : MonoBehaviour {

    [Header("Breakout Blocks Variables")]
    [SerializeField]
    GameObject _blockPF;
    [SerializeField]
    List<Color> _rowsOfColors;
    [SerializeField]
    int _howManyPerRow;
    [SerializeField]
    float _horizontalSpawnOffset;
    [SerializeField]
    float _verticalSpawnOffset;
    Vector3 _spawnPos;

    public void Init()
    {
        _spawnPos = transform.position;
        for (int row = 0; row < _rowsOfColors.Count; row++)
        {
            _spawnPos.z = transform.position.z - (_verticalSpawnOffset * row);
            SpawnRowOfBlocks(_rowsOfColors[row]);
        }
    }

    private void SpawnRowOfBlocks(Color spawnColor)
    {
        for (int blockNum = 0; blockNum < _howManyPerRow; blockNum++)
        {
            _spawnPos.x = transform.position.x + (_horizontalSpawnOffset * blockNum);
            GameObject newBlock = Instantiate<GameObject>(_blockPF, _spawnPos, transform.rotation, null);
            newBlock.GetComponent<BreakoutBlock>().Init(this, spawnColor);
        }
    }
}
