using UnityEngine;

[CreateAssetMenu(fileName = "BlockletData", menuName = "ScriptableObjects/BlockletData")]
public class BlockletData : ScriptableObject
{
    public Sprite sprite;
    public Color color;

    public Blocklet blockletPrefab;
}
