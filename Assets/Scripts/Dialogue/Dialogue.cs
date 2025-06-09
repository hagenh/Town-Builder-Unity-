using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue")]
public class Dialogue : ScriptableObject
{
    public DialogueLine[] Lines;
}
