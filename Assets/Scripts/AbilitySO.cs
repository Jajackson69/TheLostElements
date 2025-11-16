using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Ability")]
public class AbilitySO : ScriptableObject
{
    [Header("Info")]
    public string abilityId = "SteamBurst";
    public Sprite uiIcon;
    public int manaCost = 0;

    [Header("Projectile/VFX")]
    public GameObject prefab;   // projectile or vfx
    public float speed = 12f;   // 0 = no movement
    public float lifetime = 3f; // seconds before auto-destroy

    [Header("Damage")]
    public int damage = 1;      // 0 = purely visual

    [Header("Aiming / Spawn")]
    public bool faceMouse = true;
    public bool spawnAtFirePoint = true;
}
