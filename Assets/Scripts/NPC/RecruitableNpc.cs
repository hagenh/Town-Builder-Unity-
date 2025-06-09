using UnityEngine;

namespace DefaultNamespace.NPC
{
    public class RecruitableNpc : MonoBehaviour
    {
        public Dialogue introductionDialogue;
        public Dialogue recruitedDialogue;
        public int like;
        public int recruitThreshold = 3;
        public bool recruited;

        public void Interact(PlayerController player)
        {
            if (recruited)
            {
                if (recruitedDialogue != null)
                {
                    DialogueManager.Instance.StartDialogue(recruitedDialogue);
                }
                return;
            }

            if (like >= recruitThreshold)
            {
                recruited = true;
                if (recruitedDialogue != null)
                {
                    DialogueManager.Instance.StartDialogue(recruitedDialogue);
                }
            }
            else if (introductionDialogue != null)
            {
                DialogueManager.Instance.StartDialogue(introductionDialogue);
            }
        }

        public void IncreaseLike(int amount)
        {
            like += amount;
            like = Mathf.Min(like, recruitThreshold);
        }
    }
}
