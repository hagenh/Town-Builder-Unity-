using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    private readonly Queue<DialogueLine> _lines = new();
    private bool _showing;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartDialogue(Dialogue dialogue)
    {
        _lines.Clear();
        foreach (var line in dialogue.Lines)
        {
            _lines.Enqueue(line);
        }

        _showing = true;
    }

    private void Update()
    {
        if (!_showing)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _lines.Dequeue();
            if (_lines.Count == 0)
            {
                _showing = false;
            }
        }
    }

    private void OnGUI()
    {
        if (!_showing || _lines.Count == 0)
            return;

        var line = _lines.Peek();
        GUI.Box(new Rect(10, Screen.height - 110, Screen.width - 20, 100), $"{line.Speaker}: {line.Text}");
        GUI.Label(new Rect(Screen.width - 80, Screen.height - 30, 70, 20), "[Space]");
    }
}
