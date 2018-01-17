/* Author: Gabriel B. Gallagher Spetember 6th, 2017
 * 
 * Inherits the DestructableRockControl class and adds the logic to report the locations of the destroyed
 * blocks to the destructable block parent game object.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable_Block_Tutorial_Boss : DestructableRockControl
{
    public override void DealDamage(int damage)
    {
        base.health -= damage;

        if (base.health <= 0)
        {
            GameObject parent = transform.parent.gameObject;
            parent.GetComponent<Tutorial_Boss_Room_Destructable_Blocks>().ReportDestroyedChild(
                transform.name, transform.position);
            Destroy(gameObject);
        }
    }
}
