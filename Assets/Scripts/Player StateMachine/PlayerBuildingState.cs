using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using DefaultNamespace.Items;
using UnityEngine;

namespace Player_StateMachine
{
    [System.Serializable]
    public class BuildingBlueprint
    {
        public BuildingType type;
        public GameObject prefab;
        public ResourceCost[] cost;
        public float buildTime;
    }

    [System.Serializable]
    public struct ResourceCost
    {
        public Item type;
        public int amount;
    }
    
    public enum BuildingType
    {
        Shelter,    // Basic protection from elements
        House,      // Residential structure
        Workshop,   // Crafting station
        Farm,       // Food production
        Storage     // Resource storage
    }

    public class PlayerBuildingState : PlayerState
    {
        private BuildingBlueprint currentBuilding;
        private GameObject previewInstance;
        private bool isValidPlacement;

        private PlayerController player;

        public PlayerBuildingState(BuildingType buildingType)
        {
            currentBuilding = GetBuildingBlueprint(buildingType);
        }

        public override void EnterState(PlayerController player)
        {
            this.player = player;
            var mousePosition = Input.mousePosition;
            
            // Create building preview at mouse position
            previewInstance = Object.Instantiate(
                currentBuilding.prefab, 
                player.playerCamera.ScreenToWorldPoint(mousePosition), 
                Quaternion.identity
            );

            // Disable colliders on preview
            var colliders = previewInstance.GetComponents<BoxCollider2D>().ToList();
            colliders.ForEach(x => x.enabled = false);

            //player.SetPreviewMaterials(previewInstance);
        }

        public override void UpdateState(PlayerController player)
        {
            UpdatePreviewPosition(player);
            CheckPlacementValidity(player);
            HandlePlacementInput(player);
        }
        
        public override void ExitState(PlayerController player)
        {
            Object.Destroy(previewInstance);
        }

        private void UpdatePreviewPosition(PlayerController player)
        {
            var mousePosition = Input.mousePosition;
            mousePosition.z = player.playerCamera.transform.position.y;
            var worldPosition = player.playerCamera.ScreenToWorldPoint(mousePosition);
            previewInstance.transform.position = new Vector3(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.y));
        }

        private void CheckPlacementValidity(PlayerController player)
        {
            /*isValidPlacement = player.CanPlaceBuilding(
                previewInstance.transform.position,
                currentBuilding
            );
        
            player.UpdatePreviewColor(previewInstance, isValidPlacement);*/
        }

        private void HandlePlacementInput(PlayerController player)
        {
            isValidPlacement = true;
            if (Input.GetMouseButtonDown(0) && isValidPlacement)
            {
                if (TryPlaceBuilding(currentBuilding.type, previewInstance.transform.position))
                {
                    player.stateMachine.SwitchState(new PlayerIdleState());
                }
                
                // TODO: What to do if building failed?
            }
        }
        
        
        public bool TryPlaceBuilding(BuildingType type, Vector3 position)
        {
            // Check resources
            var blueprint = GetBuildingBlueprint(type);
            var cost = GetBuildingCost(type);
            if (!GameManager.Instance.Inventory.HasResources(cost))
            {
                Debug.Log("Cannot place building here. Not enough resources.");
                return false;
            }

            // Check valid position
            Vector2 mousePos = player.playerCamera.ScreenToWorldPoint(Input.mousePosition);
            var colliders = Physics2D.OverlapCircleAll(mousePos, 1f, LayerMask.GetMask("Terrain"));
            if (colliders.Length > 0)
            {
                Debug.Log("Cannot place building here. Hit a collider.");
                return false;
            }

            // Place building
            GameManager.Instance.Inventory.RemoveResources(cost);
            player.InstantiateObject(blueprint.prefab, position);
            
            Object.Instantiate(
                currentBuilding.prefab, 
                position, 
                Quaternion.identity
            );
            
            Debug.Log("Placed building: " + type);
            
            return true;
        }

        private Dictionary<Item, int> GetBuildingCost(BuildingType type)
        {
            return GetBuildingBlueprint(type).cost.ToDictionary(cost => cost.type, cost => cost.amount);
        }
        
        public BuildingBlueprint GetBuildingBlueprint(BuildingType type)
        {
            return type switch
            {
                BuildingType.Shelter => new BuildingBlueprint
                {
                    type = BuildingType.Shelter,
                    prefab = null,
                    cost = new ResourceCost[] { new ResourceCost { type = ItemDatabase.Instance.GetItemById(0), amount = 20 } }, // TODO: Don't use hardcoded ID here
                    buildTime = 5f
                },
                BuildingType.House => new BuildingBlueprint
                {
                    type = BuildingType.House,
                    prefab = null,
                    cost = new ResourceCost[] { new ResourceCost { type = ItemDatabase.Instance.GetItemById(0), amount = 50 }, new ResourceCost { type = ItemDatabase.Instance.GetItemById(1), amount = 20 } },
                    buildTime = 10f
                },
                BuildingType.Workshop => new BuildingBlueprint
                {
                    type = BuildingType.Workshop,
                    prefab = null,
                    cost = new ResourceCost[] { new ResourceCost { type = ItemDatabase.Instance.GetItemById(0), amount = 30 }, new ResourceCost { type = ItemDatabase.Instance.GetItemById(1), amount = 30 } },
                    buildTime = 15f
                },
                BuildingType.Farm => new BuildingBlueprint
                {
                    type = BuildingType.Farm,
                    prefab = Resources.Load("FarmPrefab") as GameObject,
                    cost = new ResourceCost[] { new ResourceCost { type = ItemDatabase.Instance.GetItemById(0), amount = 40 }, new ResourceCost { type = ItemDatabase.Instance.GetItemById(1), amount = 40 } },
                    buildTime = 20f
                },
                BuildingType.Storage => new BuildingBlueprint
                {
                    type = BuildingType.Storage,
                    prefab = null,
                    cost = new ResourceCost[] { new ResourceCost { type = ItemDatabase.Instance.GetItemById(0), amount = 60 }, new ResourceCost { type = ItemDatabase.Instance.GetItemById(1), amount = 60 } },
                    buildTime = 25f
                },
                _ => null
            };
        }
        
        public void OnGUI()
        {
            GUI.Label(new Rect(10, 50, 200, 20), "Current Building: " + currentBuilding.type);
            if (GUI.Button(new Rect(10, 70, 200, 20), "Give Resources"))
            {
                currentBuilding.cost.ToList().ForEach(cost =>
                {
                    GameManager.Instance.Inventory.AddResource(cost.type, cost.amount);
                });
            }
        }

    }
    
    
}