using UnityEngine;

namespace DialogueExamples
{
    public class NpcDialogueInteractor : MonoBehaviour
    {
        public RecruitableNpc npc;

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
                return;

            if (Input.GetKeyDown(KeyCode.E))
            {
                npc.Interact(other.GetComponent<PlayerController>());
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                npc.IncreaseLike(1);
            }
        }
    }
}
