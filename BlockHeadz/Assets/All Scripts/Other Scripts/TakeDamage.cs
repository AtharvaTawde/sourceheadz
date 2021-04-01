using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamage : MonoBehaviour {

    [SerializeField] string self;

    private void Start() {
        self = gameObject.name;
    }

    public void ReceiveDamage(int damage) {
        #region Switch Statement that Doesn't Work
        //switch (self) {
        //    case "Nyert":
        //        GetComponent<NyertCombat>().TakeDamage(damage);
        //        break;
        //    case "Goblin":
        //        GetComponent<GoblinEnemy>().TakeDamage(damage);
        //        break;
        //    case "Fallen":
        //        GetComponent<FallenCombat>().TakeDamage(damage);
        //        break;
        //    case "Archer":
        //        GetComponent<ArcherCombat>().TakeDamage(damage);
        //        break;
        //    case "Revenant":
        //        GetComponent<RevenantPatrol>().TakeDamage(damage);
        //        break;
        //    case "Gunflower":
        //        GetComponent<Gunflower>().TakeDamage(damage);
        //        break;
        //    case "Trunk":
        //        GetComponent<TreeMaterials>().TakeDamage(damage);
        //        break;
        //    case "Leaves":
        //        GetComponent<TreeMaterials>().TakeDamage(damage);
        //        break;
        //    case "Apples":
        //        GetComponent<TreeMaterials>().TakeDamage(damage);
        //        break;
        //    case "Log":
        //        GetComponent<TreeMaterials>().TakeDamage(damage);
        //        break;
        //    case "Stone Block":
        //        GetComponent<TreeMaterials>().TakeDamage(damage);
        //        break;
        //    case "Oven":
        //        GetComponent<TreeMaterials>().TakeDamage(damage);
        //        break;
        //    case "Firetrap":
        //        GetComponent<Firetrap>().TakeDamage(damage);
        //        break;
        //    case "Zombie":
        //        GetComponent<Zombie>().TakeDamage(damage);
        //        break;
        //    case "Buff":
        //        GetComponent<Buff>().TakeDamage(damage);
        //        break;
        //    case "Chest":
        //        GetComponent<Chest>().TakeDamage(damage);
        //        break;
        //    case "Chicken":
        //        GetComponent<Chicken>().TakeDamage(damage);
        //        break;
        //    case "Stratum":
        //        GetComponent<Stratum>().TakeDamage(damage);
        //        break;
        //}
        #endregion

        if (self.Contains("Nyert")) {
            GetComponent<NyertCombat>().TakeDamage(damage);
        } else if (self.Contains("Goblin")) {
            GetComponent<GoblinEnemy>().TakeDamage(damage);
        } else if (self.Contains("Fallen")) {
            GetComponent<FallenCombat>().TakeDamage(damage);
        } else if (self.Contains("Archer")) {
            GetComponent<ArcherCombat>().TakeDamage(damage);
        } else if (self.Contains("Revenant")) {
            GetComponent<RevenantPatrol>().TakeDamage(damage);
        } else if (self.Contains("Gunflower")) {
            GetComponent<Gunflower>().TakeDamage(damage);
        } else if (self.Contains("Trunk") || self.Contains("Leaves") || self.Contains("Apples") || self.Contains("Log") || self.Contains("Stone Block") || self.Contains("Oven")) {
            GetComponent<TreeMaterials>().TakeDamage(damage);
        } else if (self.Contains("Firetrap")) {
            GetComponent<Firetrap>().TakeDamage(damage);
        } else if (self.Contains("Zombie")) {
            GetComponent<Zombie>().TakeDamage(damage);
        } else if (self.Contains("Buff")) {
            GetComponent<Buff>().TakeDamage(damage);
        } else if (self.Contains("Chest")) {
            GetComponent<Chest>().TakeDamage(damage);
        } else if (self.Contains("Chicken")) {
            GetComponent<Chicken>().TakeDamage(damage);
        } else if (self.Contains("Stratum")) {
            GetComponent<Stratum>().TakeDamage(damage);
        } else if (self.Contains("Traveler")) {
            GetComponent<Traveler>().TakeDamage(damage);
        }
    }

    public void SetBurnTime(float burnTime) {
        if (self.Contains("Nyert")) {
            GetComponent<NyertCombat>().burnTime = burnTime;
        } else if (self.Contains("Goblin")) {
            GetComponent<GoblinEnemy>().burnTime = burnTime;
        } else if (self.Contains("Archer")) {
            GetComponent<ArcherCombat>().burnTime = burnTime;
        } else if (self.Contains("Revenant")) {
            GetComponent<RevenantPatrol>().burnTime = burnTime;
        } else if (self.Contains("Buff")) {
            GetComponent<Buff>().burnTime = burnTime;
        } else if (self.Contains("Chicken")) {
            GetComponent<Chicken>().burnTime = burnTime;
        } else if (self.Contains("Traveler")) {
            GetComponent<Traveler>().burnTime = burnTime;
        } 
    }

    public void AddBurnTime(float addBurnTime) {
        if (self.Contains("Nyert")) {
            GetComponent<NyertCombat>().burnTime += addBurnTime;
        } else if (self.Contains("Goblin")) {
            GetComponent<GoblinEnemy>().burnTime += addBurnTime;
        } else if (self.Contains("Archer")) {
            GetComponent<ArcherCombat>().burnTime += addBurnTime;
        } else if (self.Contains("Revenant")) {
            GetComponent<RevenantPatrol>().burnTime += addBurnTime;
        } else if (self.Contains("Buff")) {
            GetComponent<Buff>().burnTime += addBurnTime;
        } else if (self.Contains("Chicken")) {
            GetComponent<Chicken>().burnTime += addBurnTime;
        } else if (self.Contains("Traveler")) {
            GetComponent<Traveler>().burnTime += addBurnTime;
        } 
    }

    public float BurnTime() {
        if (self.Contains("Nyert")) {
            return GetComponent<NyertCombat>().burnTime;
        } else if (self.Contains("Goblin")) {
            return GetComponent<GoblinEnemy>().burnTime;
        } else if (self.Contains("Archer")) {
            return GetComponent<ArcherCombat>().burnTime;
        } else if (self.Contains("Revenant")) {
            return GetComponent<RevenantPatrol>().burnTime;
        } else if (self.Contains("Buff")) {
            return GetComponent<Buff>().burnTime;
        } else if (self.Contains("Chicken")) {
            return GetComponent<Chicken>().burnTime;
        } else if (self.Contains("Traveler")) {
            return GetComponent<Traveler>().burnTime;
        } else {
            return 0;
        }
    }

    public void BlockDrop() {
        if (self.Contains("Trunk") || self.Contains("Leaves") || self.Contains("Apples") || self.Contains("Log") || self.Contains("Stone Block") || self.Contains("Oven")) {
            GetComponent<TreeMaterials>().Die();
        } else if (self.Contains("Chest")) {
            GetComponent<Chest>().Die();
        }
    }

}
    