using DefaultNamespace;
using DefaultNamespace.Items;
using Player_StateMachine;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private static readonly int Speed = Animator.StringToHash("Speed");

    [Header("References")]
    public Rigidbody2D rb;
    public Animator animator;
    public GameObject weaponObject;
    public float moveSpeed = 5f;
    
    [Header("Building")]
    public GameObject[] buildingPrefabs;
    public Material previewMaterial;
    
    [Header("Combat")]
    public float attackCooldown = 0.5f;
    public float lastAttackTime;
    
    [HideInInspector] public PlayerStateMachine stateMachine;

    public Vector2 movement;
    public Interactable currentTarget;
    public Camera playerCamera;
    public InventoryUi inventoryUi;
    
    public float inventoryMessageTime = 2f;

    private void Start()
    {
        playerCamera = Camera.main;
        stateMachine = gameObject.AddComponent<PlayerStateMachine>();
        rb = GetComponent<Rigidbody2D>();
        inventoryUi = FindFirstObjectByType<InventoryUi>();
    }

    private void Update()
    {
        // Handle input for state transitions
        if (Input.GetMouseButtonDown(0) && stateMachine.CurrentState is not PlayerBuildingState && !GameManager.Instance.CurrentItem.IsUnityNull() && GameManager.Instance.CurrentItem!.itemType == ItemType.Weapon)
        {
            stateMachine.SwitchState(new PlayerCombatState());
        }
        
        if (Input.GetKeyDown(KeyCode.E) && currentTarget)
        {
            stateMachine.SwitchState(new PlayerInteractionState(currentTarget));
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            stateMachine.SwitchState(new PlayerBuildingState(BuildingType.Farm));
        }
        
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        if (movement.sqrMagnitude > 0)
        {
            stateMachine.SwitchState(new PlayerMovementState());
        }

        HandleToolbarInput();
    }

    // Trigger detection for interactables
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Interactable>(out var interactable))
        {
            Debug.Log("Collided with interactable: " + interactable);
            currentTarget = interactable;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Interactable>() == currentTarget)
        {
            currentTarget = null;
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 20), "Current State: " + stateMachine.CurrentState.GetType().Name);
        GUI.Label(new Rect(10, 30, 200, 20), "Current Interaction: " + (currentTarget != null ? currentTarget.name : "null"));
        GUI.Label(new Rect(10, 50, 200, 20), "Position: " + transform.position);
        GUI.Label(new Rect(10, 70, 200, 20), "FPS: " + (int)(1f / Time.unscaledDeltaTime));
        
        
        if(stateMachine.CurrentState is PlayerBuildingState buildingState)
        {
            buildingState.OnGUI();
        }
        
        if(stateMachine.CurrentState is PlayerInteractionState interactionState)
        {
            interactionState.OnGUI();
        }
    }
    
    public void InstantiateObject(GameObject prefab, Vector3 position)
    {
        Instantiate(prefab, position, Quaternion.identity);
    }
    
    public void DestroyObject(GameObject obj)
    {
        Destroy(obj);
    }

    void HandleToolbarInput()
    {
        for (int i = 0; i < inventoryUi.slots.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                inventoryUi.selectedIndex = i;
                inventoryUi.UpdateSelection(i);

                var elementAtIndex = GameManager.Instance.Inventory.Resources.SafeGetElementOrNull(i);
                if (!elementAtIndex.IsUnityNull() && elementAtIndex.HasValue)
                {
                    GameManager.Instance.CurrentItem = elementAtIndex.Value.Key;
                }
                else
                {
                    GameManager.Instance.CurrentItem = null;
                }
                    
                return;
            }
        }
        
        
    }
}