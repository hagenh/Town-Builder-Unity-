using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string Speaker;
    [TextArea(2,5)]
    public string Text;
}
