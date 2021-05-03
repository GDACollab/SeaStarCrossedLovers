using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

[CreateAssetMenu(fileName = "BlockData", menuName = "ScriptableObjects/BlockData")]
public class BlockData : ScriptableObject
{
    public float GRID_SIZE = 4;

    public Block blockPrefab;

    public BlockletData cell_00;
    public BlockletData cell_10;
    public BlockletData cell_20;
    public BlockletData cell_30;

    public BlockletData cell_01;
    public BlockletData cell_11;
    public BlockletData cell_21;
    public BlockletData cell_31;

    public BlockletData cell_02;
    public BlockletData cell_12;
    public BlockletData cell_22;
    public BlockletData cell_32;

    public BlockletData cell_03;
    public BlockletData cell_13;
    public BlockletData cell_23;
    public BlockletData cell_33;

    public Dictionary<(int, int), BlockletData> GridCellDictionary =
        new Dictionary<(int, int), BlockletData>();

    private void OnValidate()
    {
        InitializeDict();
    }

    public void InitializeDict() {
        GridCellDictionary.Clear();
        GridCellDictionary.Add((0, 0), cell_00);
        GridCellDictionary.Add((1, 0), cell_10);
        GridCellDictionary.Add((2, 0), cell_20);
        GridCellDictionary.Add((3, 0), cell_30);

        GridCellDictionary.Add((0, 1), cell_01);
        GridCellDictionary.Add((1, 1), cell_11);
        GridCellDictionary.Add((2, 1), cell_21);
        GridCellDictionary.Add((3, 1), cell_31);

        GridCellDictionary.Add((0, 2), cell_02);
        GridCellDictionary.Add((1, 2), cell_12);
        GridCellDictionary.Add((2, 2), cell_22);
        GridCellDictionary.Add((3, 2), cell_32);

        GridCellDictionary.Add((0, 3), cell_03);
        GridCellDictionary.Add((1, 3), cell_13);
        GridCellDictionary.Add((2, 3), cell_23);
        GridCellDictionary.Add((3, 3), cell_33);
    }
}
