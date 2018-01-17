/* Author: Gabriel B. Gallagher Spetember 6th, 2017
 * 
 * Class is attached to a the parent game object of the destructable blocks in the Boss room. Contains
 * the logic for respawning destroyed blocks which is a mechanic added to punish the player for either
 * going a full attack cycle without dealing damage, or dropping a bomb on an invulnerable attack square
 */

using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Boss_Room_Destructable_Blocks : MonoBehaviour
{
    public GameObject destructableBlockPrefab, indestructableBlockPrefab;

    public List<string> destroyedChildNames;

    public List<Vector2> destroyedChildPositions;

    private void Start()
    {
        destroyedChildNames = new List<string>();
        destroyedChildPositions = new List<Vector2>();
    }

    /* When a child block gets hit by an explosion, before it gets destroyed, it will call this method 
     * to add its name and position to the destroyedChildNames list and the destroyedChildPositions
     * respectively
     */
    public void ReportDestroyedChild(string childName, Vector2 childPosition)
    {
        destroyedChildNames.Add(childName);
        destroyedChildPositions.Add(childPosition);
    }

    /* When the Boss goes a full attack cycle without taking damage, spawn 1, 2, or 3 indestructable
     * blocks in a random location location previously occupied by a destroyed destructable block based 
     * on whether the boss has 3, 2, or 1 health remaining.
    */
    public void SpawnIndestructableBlocks(int health)
    {
        Debug.Log("Spawning indestructable blocks");
        for (int i = 0; i <= (3 - health); i++)
        {
            int randomIndex = Random.Range(0, destroyedChildNames.Count);
            GameObject newBlock = Instantiate(indestructableBlockPrefab, destroyedChildPositions[i], 
                Quaternion.identity, transform.parent.Find("Indestructable_Blocks"));
            destroyedChildNames.Remove(destroyedChildNames[i]);
            destroyedChildPositions.Remove(destroyedChildPositions[i]);
        }
    }

    /* If the player drops a bomb on an invulnerable target, spawn 1, 2, or 3 destructable blocks 
     * in a random location previously occupied by a destroyed destructable block based on whether the 
     * boss has 3, 2, or 1 health remaining. 
     */
    public void SpawnDestructableBlocks(int health)
    {
        for (int i = 0; i <= (3 - health); i++)
        {
            int randomIndex = Random.Range(0, destroyedChildNames.Count);
            GameObject newBlock = Instantiate(destructableBlockPrefab, destroyedChildPositions[i],
                Quaternion.identity, transform);
            newBlock.name = destroyedChildNames[i];
        }
    }
}
