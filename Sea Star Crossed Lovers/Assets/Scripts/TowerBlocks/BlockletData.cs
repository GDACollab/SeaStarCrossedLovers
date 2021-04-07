using UnityEngine;

[CreateAssetMenu(fileName = "BlockletData", menuName = "ScriptableObjects/BlockletData")]
public class BlockletData : ScriptableObject
{
    public Texture texture;
    public Color color;

    public Blocklet blockletPrefab;
}
