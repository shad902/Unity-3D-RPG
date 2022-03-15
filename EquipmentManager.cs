using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{

    public static EquipmentManager instance;


    void Awake()
    {
        instance = this;   
    }
    public Equipment[] defaultItems;//array that holds default items we start with
    public SkinnedMeshRenderer targetMesh;
    //this tracks what we currently have equipped in an array
    Equipment[] currentEquipment;
    SkinnedMeshRenderer[] currentMeshes;

    //callback when we change an item by removing/adding to inventory thru equip/unequip
    public delegate void OnEquipmentChanged(Equipment newItem, Equipment oldItem);
    public OnEquipmentChanged onEquipmentChanged;//other scripts can now call on this method

    Inventory inventory;
    void Start()
    {
        inventory = Inventory.instance;
        //string array of all equipment inside equipment array so we get length of it and store in numSlots
        int numSlots=System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        currentEquipment = new Equipment[numSlots];
        currentMeshes = new SkinnedMeshRenderer[numSlots];

        EquipDefaultItems();//call our method in the start 

    }
    //method to equip an item 
    public void Equip(Equipment newItem)
    {
        int slotIndex = (int)newItem.equipSlot;
        //lets us automatically add old item back to inventory. 
        Equipment oldItem = Unequip(slotIndex);


        //callback method whenever we change items
        if (onEquipmentChanged!=null)
        {
            onEquipmentChanged.Invoke(newItem, oldItem);
        }
        SetEquipmentBlendShapes(newItem, 100); //adds new item with 100 set to weight

        currentEquipment[slotIndex] = newItem;
        //instantiate the new equipment mesh 
        SkinnedMeshRenderer newMesh = Instantiate<SkinnedMeshRenderer>(newItem.mesh);
        //set parent object where target mesh is our player
        newMesh.transform.parent = targetMesh.transform;
        newMesh.bones= targetMesh.bones;
        newMesh.rootBone = targetMesh.rootBone;
        currentMeshes[slotIndex] = newMesh;
    }

    //to unequip items we are wearing 
    public Equipment Unequip(int slotIndex)
    {
        //check if we have any items in the slotIndex to see if we put it back in inventory
        if (currentEquipment[slotIndex]!=null)
        {
            if (currentMeshes[slotIndex]!=null)
            {
                Destroy(currentMeshes[slotIndex].gameObject);
            }
            Equipment oldItem = currentEquipment[slotIndex];
            SetEquipmentBlendShapes(oldItem, 0);
            inventory.Add(oldItem); //add back old items
            currentEquipment[slotIndex] = null;
            //callback method whenever we change items
            if (onEquipmentChanged != null)
            {
                onEquipmentChanged.Invoke(null, oldItem);
            }
            return oldItem; //when we return the old item from removing it from equip
        }
        return null; //if there was no item currently in equip slot we return nothing to inventory
    }
    public void UnequipAll()
    {
        for (int i=0; i<currentEquipment.Length;i++)//loop thru all items to unequip them all
        {
            Unequip(i);
        }
        EquipDefaultItems();//we call this again since we unequip everything but want to keep default items

    }

    void SetEquipmentBlendShapes(Equipment item, int weight)
    {
        foreach(EquipmentMeshRegion blendShape in item.coveredMeshRegions)
        {
            //int blendshape will get index of legs, arms or torso 
            targetMesh.SetBlendShapeWeight((int)blendShape, weight);
        }
    }

    void EquipDefaultItems() //method to equip default items at the start
    {
        foreach (Equipment item in defaultItems) //loop thru each default item and equip 
        {
            Equip(item);
        }
        
    }

    private void Update()
    {
        //when we press U button we unequip all our items we are wearing
        if (Input.GetKeyDown(KeyCode.U))
        {
            UnequipAll();
        }
    }

}
