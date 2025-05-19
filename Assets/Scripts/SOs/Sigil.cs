using UnityEngine;

[CreateAssetMenu(fileName = "New Sigil", menuName = "Sigils/Sigil")]
public class Sigil : ScriptableObject
{
    public string sigilName;
    public Sprite sigilSprite;
    public string IDNumber;
    public Color trailColor = Color.white;
    public Family family;
    public Family advantage;
    public BallEffect special;

    [Range(1, 5)]
    public int difficulty;

    public Shard ingredient1;
    public Shard ingredient2;
    public Shard ingredient3;

    public string description;
    

    public int BallSpeed
    {
        get
        {
            return difficulty switch
            {
                1 => 10,
                2 => 15,
                3 => 20,
                4 => 25,
                5 => 30,
                _ => 10
            };
        }
    }
}
