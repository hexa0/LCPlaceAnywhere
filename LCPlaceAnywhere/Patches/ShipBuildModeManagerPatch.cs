using HarmonyLib;
using UnityEngine;

namespace LCPlaceAnywhere.Patches
{
    [HarmonyPatch(typeof(ShipBuildModeManager))]
    internal class ShipBuildModeManagerPatch
    {
        [HarmonyPatch("EnterBuildMode")]
        [HarmonyPrefix]
        static void forceAllowBuildPatch(ref float ___timeSincePlacingObject, ref bool ___CanConfirmPosition)
        {
            ___timeSincePlacingObject = 1.69420f; // remove the cooldown
            ___CanConfirmPosition = true; // force allow position confirmation
        }

        // always allow the player to build unless they're actively using the terminal
        // disabling building during ship movement could fix the rotation issue but i'd rather fix that bug instead
        [HarmonyPatch("PlayerMeetsConditionsToBuild")]
        [HarmonyPostfix]
        static void forceAllowBuildOutsideShip(ref bool __result)
        {
            __result = true;

            if (!(GameNetworkManager.Instance.localPlayerController == null))
            {
                if (GameNetworkManager.Instance.localPlayerController.isPlayerDead || GameNetworkManager.Instance.localPlayerController.inSpecialInteractAnimation || GameNetworkManager.Instance.localPlayerController.activatingItem)
                {
                    __result = false;
                }

            }
        }

        // there's probably a better way of doing this rather than replacing the whole method with the patched v45 one but ehh it works
        // this broke in v49 as v49 checks if the position is outside of the ship, and if it is then the position is reset to what it was before
        // however a weird side effect is that it still saves in that position so a reload of the save will fix it
        [HarmonyPatch("PlaceShipObject")]
        [HarmonyPrefix]
        static bool fixV49StuckGlitch(Vector3 placementPosition, Vector3 placementRotation, PlaceableShipObject placeableObject, bool placementSFX = true)
        {
            StartOfRound.Instance.suckingFurnitureOutOfShip = false;
            StartOfRound.Instance.unlockablesList.unlockables[placeableObject.unlockableID].placedPosition = placementPosition;
            StartOfRound.Instance.unlockablesList.unlockables[placeableObject.unlockableID].placedRotation = placementRotation;
            Debug.Log(string.Format("Saving placed position as: {0}", placementPosition));
            StartOfRound.Instance.unlockablesList.unlockables[placeableObject.unlockableID].hasBeenMoved = true;
            if (placeableObject.parentObjectSecondary != null)
            {
                Quaternion lhs = Quaternion.Euler(placementRotation) * Quaternion.Inverse(placeableObject.mainMesh.transform.rotation);
                placeableObject.parentObjectSecondary.transform.rotation = lhs * placeableObject.parentObjectSecondary.transform.rotation;
                placeableObject.parentObjectSecondary.position = placementPosition + (placeableObject.parentObjectSecondary.transform.position - placeableObject.mainMesh.transform.position) + (placeableObject.mainMesh.transform.position - placeableObject.placeObjectCollider.transform.position);
            }
            else if (placeableObject.parentObject != null)
            {
                Quaternion lhs2 = Quaternion.Euler(placementRotation) * Quaternion.Inverse(placeableObject.mainMesh.transform.rotation);
                placeableObject.parentObject.rotationOffset = (lhs2 * placeableObject.parentObject.transform.rotation).eulerAngles;
                placeableObject.parentObject.transform.rotation = lhs2 * placeableObject.parentObject.transform.rotation;
                placeableObject.parentObject.positionOffset = StartOfRound.Instance.elevatorTransform.InverseTransformPoint(placementPosition + (placeableObject.parentObject.transform.position - placeableObject.mainMesh.transform.position) + (placeableObject.mainMesh.transform.position - placeableObject.placeObjectCollider.transform.position));
            }
            if (placementSFX)
            {
                placeableObject.GetComponent<AudioSource>().PlayOneShot(placeableObject.placeObjectSFX);
            }

            return false; // stop the original method
        }

        // never outline in red as if the player cannot place the object
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void alwaysDisplayGreenGhost(ref ShipBuildModeManager __instance)
        {
            __instance.ghostObjectRenderer.sharedMaterial = __instance.ghostObjectGreen;
        }
    }
}