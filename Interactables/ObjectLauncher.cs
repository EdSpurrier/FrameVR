using Foundry.Common;
using FrameCoreU.Events;
using FrameCoreU.Unity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FrameVR.Interactables
{
    public class ObjectLauncher: MonoBehaviour
    {
        [FoldoutGroup("Settings")]
        [SerializeField] private bool active = true;
        [FoldoutGroup("Settings")]
        [HideLabel]
        [SerializeField] private Cooldown launchCooldown;
        
        [FoldoutGroup("Settings/Launch")]
        [HorizontalGroup("Settings/Launch/Split", 0.5f)]
        [HideLabel]
        [SuffixLabel("force", true)]
        [SerializeField] private float launchForce = 20f;
        [HorizontalGroup("Settings/Launch/Split", 0.5f)]
        [HideLabel]
        [SerializeField] private ForceMode forceMode = ForceMode.Impulse;
        
        [FoldoutGroup("Spawn & Prefab")]
        [SerializeField] private Transform projectilePrefab;
        [FoldoutGroup("Spawn & Prefab")]
        [SerializeField] private Transform spawnPoint;
        
        [BoxGroup("Events")]
        [FoldoutGroup("Events/Launch")]
        [SerializeField, HideLabel] private FrameCoreEvent onLaunch = new()
        {
            eventName = "On - Launch"
        };
        
        
        public void Launch()
        {
            if (!active)
                return;
            
            if (!launchCooldown.TryUse())
                return;
            
            if (projectilePrefab == null || spawnPoint == null)
            {
                Debug.LogWarning("ObjectLauncher >> Missing projectilePrefab or spawnPoint.");
                return;
            }
            
            GameObject spawn = projectilePrefab.SpawnObject(spawnPoint.position, spawnPoint.rotation);

            if (!spawn.TryGetComponent(out Rigidbody rb))
            {
                Debug.LogWarning("ObjectLauncher >> Spawned projectile has no Rigidbody.");
                return;
            }
            
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddForce(spawnPoint.forward.normalized * launchForce, forceMode);
            
            onLaunch.Activate();
        }
    }
}