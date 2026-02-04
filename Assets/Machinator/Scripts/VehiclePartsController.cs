using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Text.RegularExpressions;

/// <summary>
/// 2026 01 27
///     Can add fucntion to just create and plus fill properties
///     Export settings for blender: 
///         Scale: 1.0
///         Apply settings: All local
///         Forward: -Z Forward
///         Up: Y up
///         Apply Unit
///         Use Space Transform
///         Apply Transform
/// </summary>



// SPAWN CAB IRL


public class VehiclePartsController : MonoBehaviour
{
    [System.Serializable] class CabCargoLP
    {
        public string lpName;
        public string modelName;
        public Transform lpTransform;

        // Constructor to make adding items easier
        public CabCargoLP(string lp, string model, Transform transform)
        {
            lpName = lp;
            modelName = model;
            lpTransform = transform;
        }
    }

    [SerializeField] List<CabCargoLP> CabCargoLPs = new List<CabCargoLP>()
    {
        new CabCargoLP("LP_CAB", "cab", null),
        new CabCargoLP("LP_BSK", "cargo", null)
    };

    Dictionary<string, string> lpSuspensionWheels = new Dictionary<string, string>()
    {
        { "LP_SSP", "suspension" },
        { "LP_WHL", "wheel" }
    };



    /// <summary>
    /// To do: sort by cab index name
    /// </summary>
    public void SetCabAndCargo()
    {
        // Reset list back (don't forget)
        Undo.RecordObject(this, "Set Cab and Cargo");

        Transform[] allTransforms = transform.GetComponentsInChildren<Transform>(true);
        foreach (CabCargoLP cabCargoLP in CabCargoLPs)
        {
            // Find "LP_"
            foreach (Transform transformA in allTransforms)
            {
                if (transformA.name.Contains(cabCargoLP.lpName))
                {
                    // Set spawn point for cab/cargo prefab here
                    cabCargoLP.lpTransform = transformA;

                    // Find "cab" "cargo" models
                    foreach (Transform transformB in allTransforms)
                    {
                        // Filter cab inside a cab LOL
                        if (transformB.name.Contains(cabCargoLP.modelName) &&
                            transformB.parent.GetComponent<VehiclePartsController>())
                        {
                            Undo.RegisterFullObjectHierarchyUndo(transformB, "Set Cab and Cargo");
                            
                            // Put where it belongs
                            transformB.SetParent(transformA);
                            transformB.localPosition = Vector3.zero;
                            //transformB.localRotation = Quaternion.identity;

                            // Add script for scripts
                            if (!transformB.gameObject.GetComponent<CabCargoController>())
                            {
                                CabCargoController cabCargoController = transformB.gameObject.AddComponent<CabCargoController>();
                                cabCargoController.SetBreakables();
                            }
                        }
                    }
                }
            }
        }
    }

    public void SetSuspensionAndWheels()
    {
        Transform[] transforms = transform.GetComponentsInChildren<Transform>(true);
        List<Transform> usedTransforms = new List<Transform>();
        foreach (var pair in lpSuspensionWheels)
        {
            // Find "LP_"
            foreach (Transform parent in transforms)
            {
                if (parent.name.Contains(pair.Key))
                {
                    // Filter "LP_01" and "LP_02"
                    string index = Regex.Match(parent.name, @"\d+").Value;
                    foreach (Transform child in transforms)
                    {
                        if (child.name.Contains(pair.Value) &&
                            child.name.Contains(index) &&
                            !usedTransforms.Contains(child))
                        {
                            Undo.SetTransformParent(child, parent, "Move Part");
                            child.localPosition = Vector3.zero;
                            child.localRotation = Quaternion.identity;

                            usedTransforms.Add(child);

                            // LP has one element
                            break;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// To do: // Index out of bounds
    /// </summary>
    /// <param name="cabIndex"></param>
    /// <param name="cargoIndex"></param>
    public void ShowCabCargoByIndex(int cabIndex, int cargoIndex)
    {
        if (CabCargoLPs[0].lpTransform == null)
        {
            Debug.Log("No cab detected");

            return;
        }

        foreach (CabCargoLP cabCargoLP in CabCargoLPs)
        {
            if (cabCargoLP.lpTransform != null)
            {
                foreach (Transform transform in cabCargoLP.lpTransform)
                {
                    Undo.RegisterCompleteObjectUndo(transform, "Set Cab and Cargo");
                    transform.gameObject.SetActive(false);
                }
            }
        }
        CabCargoLPs[0].lpTransform.GetChild(cabIndex).gameObject.SetActive(true);

        if (CabCargoLPs[1].lpTransform == null)
        {
            Debug.Log("No cargo detected");

            return;
        }

        CabCargoLPs[1].lpTransform.GetChild(cargoIndex).gameObject.SetActive(true);
    }

    public void ShowCabCargoBreakablesByIndex(int breakableIndex)
    {
        foreach (CabCargoLP cabCargoLP in CabCargoLPs)
        {
            if (cabCargoLP.lpTransform != null)
            {
                foreach (Transform transform in cabCargoLP.lpTransform)
                {
                    transform.gameObject.GetComponent<CabCargoController>().ShowBreakablesByIndex(breakableIndex);
                }
            }
        }
    }







/*


    // LP and part name
    Dictionary<string, string> lpCabCargo = new Dictionary<string, string>()
    {
        { "LP_CAB", "cab" },
        { "LP_BSK", "cargo" }
    };
   





    //public EntityHealth entityHealth;
    // public int armor = 75;




    //List<GameObject> cabs = new List<GameObject>();










    // show cab and cargo






















    /// <summary>
    /// To see how it looks in general after import
    /// </summary>
    public void SetCabAndCargoPosition()
    {
        Transform[] transforms = transform.GetComponentsInChildren<Transform>(true);
        foreach (string pair in lpCabCargo.Keys)
        {
            // Find "LP_"
            foreach (Transform parent in transforms)
            {
                if (parent.name.Contains(pair))
                {
                    // Find "cab"
                    foreach (Transform child in transforms)
                    {
                        if (child.name.Contains(lpCabCargo[pair]))
                        {
                            Undo.SetTransformParent(child, parent, "Set Cab and Cargo");
                            child.localPosition = Vector3.zero;
                            //child.localRotation = Quaternion.identity;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Enable desired cab/breakable
    /// </summary>
    /// <param name="cabIndex"></param>
    /// <param name="cargoIndex"></param>
    /// <param name="breakableIndex"></param>
    public void SetCabAndCargoIndex(int cabIndex, int cargoIndex, int breakableIndex)
    {



        // access each cab script

        CabCargoController[] transforms = transform.GetComponentsInChildren<CabCargoController>(true);


        foreach (CabCargoController controller in transforms)
        {
        //controller.gameObject.SetActive(false);
        }


        if (transforms[0].breakables.Count == 0 )
        {

            Debug.Log("cab cargo npot set");
            return;
        }


        // LP CAB OR NOT CAB
        for (int i = 0; i < transforms.Length; i++)
        {
            // 6
            transforms[i].gameObject.SetActive(false);
        }
        // relative to how it wass added not how it named
        //cab
        transforms[cabIndex].gameObject.SetActive(true);
        //transforms[cabIndex].SetCabAndCargoIndex(breakableIndex);

        //cargo
        int offsetIndex = transforms.Length / 2 + cargoIndex;
        transforms[offsetIndex].gameObject.SetActive(true);
        //transforms[offsetIndex].SetCabAndCargoIndex(breakableIndex);




    }

















    













    // SETCCAR BY PART

    // SET COL AND SCRIPTS

    // SET HEALTH TO CAB INSTEAD OF MAIN?









    /// <summary>
    /// Set vehicle model to any damaged state
    /// </summary>
    /// <param name="partParentName"></param>
    /// <param name="partIndex"></param>
    public void SetupVehicleModel(string partParentName, int partIndex)
    {
        int partCount = 0;

        // Faster way to find vehicle parts
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>(true);
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            // Filter vehicle parts.
            // Depends on how you named it in blender LOL.
            // I recommend adding "Empty" parent to all poly_surfaces
            Transform transform = meshRenderer.transform;
            if (transform.parent.name.Contains(partParentName))
            {
                GameObject gameObject = transform.gameObject;
                Undo.RecordObject(gameObject, "Setup Vehicle Model");

                // Enable for reset case
                if (!gameObject.activeSelf)
                {
                    gameObject.SetActive(true);
                }
                // Leave only chosen part visible
                if (transform.GetSiblingIndex() != partIndex)
                {
                    gameObject.SetActive(false);

                    partCount++;
                }
            }
        }
        Debug.Log($"<color=green>Setup vehicle parts:</color> searched {partCount} parts");
    }

    
   

   



















*/


    /*
        // damage class
        public void DamagePart(int damage)
        {
            //Debug.Log("DAMAGA");



            return;


            *//*if (index < 0 || index >= breakables.Count) return;

            ArmorParts part = breakables[index];

            // Вычисляем финальный урон с учетом брони
            float finalDamage = damage;

            // Допустим, стадии 0 и 1 — это наличие брони
            // Если текущая стадия меньше 2, снижаем урон на 30%
            if (part.currentStageIndex < 2)
            {
                finalDamage *= 0.7f; // Умножаем на 0.7 (проходит только 70% урона)
            }

            // Применяем урон
            part.health -= finalDamage;

            if (entityHealth != null)
            {
                entityHealth.AddGlobalDamage(finalDamage);
            }

            // Проверяем, не пора ли сменить стадию визуально
            if (part.currentStageIndex < part.stages.Length - 1)
            {
                int targetStage = 2 - Mathf.CeilToInt(part.health / 33.4f);
                targetStage = Mathf.Clamp(targetStage, 0, part.stages.Length - 1);

                if (targetStage > part.currentStageIndex)
                {
                    part.stages[part.currentStageIndex].SetActive(false);
                    part.currentStageIndex = targetStage;
                    part.stages[targetStage].SetActive(true);
                }
            }*//*
        }

    */

    //public List<GameObject> cabPrefabs = new List<GameObject>();
    //public Transform cabPosition;
    //public List<GameObject> cargoPrefabs = new List<GameObject>();
    //public Transform cargoPosition;
    // spawn at poin then take all data here


    // what are those damage data is known?
    // each cab stores

    /*
        public class Vehicle
        {
            //cab 

            //cargo

            // address gunbs too

        }


        // cab cargo info to fill with what is spawned in slot (usually index 0)

        // spaw

        public void SpawnCab(int index)
        {
            // 
        }

        public void SpawnCargo(int index)
        {
            //
        }
        public void SpawnPart(int index, int id)
        {

        }



        public void SetAll()
        {
            // 1 find spawn points


            // put to coordnates is separate





            // deep seel cuz we dont know the structure just keys



            // Get data

            //Debug.Log();
            // 

            // prefab name




            // find position to spawn cab to



                *//*// FIND LP from lpCabCargo




                foreach (string pair in lpCabCargo.Keys)
                {
                    // Find "LP_"
                    foreach (Transform parent in transforms)
                    {
                        if (parent.name.Contains(pair))
                        {
                            // Find "cab"
                            foreach (Transform child in transforms)
                            {
                                if (child.name.Contains(lpCabCargo[pair]))
                                {
                                    Undo.SetTransformParent(child, parent, "Set Cab and Cargo");
                                    child.localPosition = Vector3.zero;
                                    //child.localRotation = Quaternion.identity;
                                }
                            }
                        }
                    }
                }*//*





            }

    */


}
