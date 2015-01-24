using UnityEngine;
using System.Collections;

public class PlayerProfile : MonoBehaviour
{
    private string playerName = string.Empty;
    public string PlayerName { set { playerName = value; } get { return playerName; } }

    private Color playerColour = Color.white;
    public Color PlayerColour { set { playerColour = value; } get { return playerColour; } }
}
