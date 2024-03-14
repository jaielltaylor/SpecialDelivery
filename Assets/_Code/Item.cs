using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
    [Header("Package Sprites")]
    [Space]
    public List<Sprite> sPackageSprites;
    public List<Sprite> mPackageSprites;
    public List<Sprite> lPackageSprites;

    [Header("Food Sprites")]
    [Space]
    public List<Sprite> sFoodSprites;
    public List<Sprite> mFoodSprites;
    public List<Sprite> lFoodSprites;

    [Header("Liquid Sprites")]
    [Space]
    public List<Sprite> sLiquidSprites;
    public List<Sprite> mLiquidSprites;
    public List<Sprite> lLiquidSprites;
    public enum ItemType
    {
        //Mail and other stuff
        smallPackage, //0
        mediumPackage, //1
        largePackage, //2
        
        //Grocery and Takeout
        smallFood, //3
        mediumFood, //4
        largeFood, //5

        //Liquids
        smallLiquid, //6
        mediumLiquid, //7
        largeLiquid, //8
    }

    public ItemType itemType;
    public int id;
    public int requiredTemp; //0 is null, 1 is cold, 2 is hot
    public int size; //1 is small, 2 is medium, 4 is large
    public Sprite sprite;

    public static ItemType GetItemType(int identifier)
    {
        switch (identifier)
        {
            default:
            case 0: return ItemType.smallPackage;
            case 1: return ItemType.mediumPackage;
            case 2: return ItemType.largePackage;
            case 3: return ItemType.smallFood;
            case 4: return ItemType.mediumFood;
            case 5: return ItemType.largeFood;
            case 6: return ItemType.smallLiquid;
            case 7: return ItemType.mediumLiquid;
            case 8: return ItemType.largeLiquid;
        }
    }

    public Sprite setSprite(int identifier)
    {
        switch(identifier)
        {
            default:
            case 0: return spriteRandomizer(sPackageSprites);
            case 1: return spriteRandomizer(mPackageSprites);
            case 2: return spriteRandomizer(lPackageSprites);
            case 3: return spriteRandomizer(sFoodSprites);
            case 4: return spriteRandomizer(mFoodSprites);
            case 5: return spriteRandomizer(lFoodSprites);
            case 6: return spriteRandomizer(sLiquidSprites);
            case 7: return spriteRandomizer(mLiquidSprites);
            case 8: return spriteRandomizer(lLiquidSprites);
        }
    }

    public int idBaseSize(int identifier)
    {
        switch (identifier)
        {
            default:
            case 0: 
            case 3:
            case 6:
                return 1;
            case 1:
            case 4:
            case 7:
                return 2;
            case 2:
            case 5:
            case 8:
                return 4;
        }
    }

    public int idBaseTemp(int identifier)
    {
        int choice = UnityEngine.Random.Range(0, 1);
        switch (identifier)
        {
            default:
            case 0:
            case 1: 
            case 2:
                if (choice == 0) { return 0; }
                else { return 1; }
            case 3: 
            case 4: 
            case 5:
                if (choice == 0) { return 0; }
                else { return 2; }
            case 6: 
            case 7: 
            case 8:
                if (choice == 0) { return 0; }
                else { return 1; }
        }
    }

    private Sprite spriteRandomizer(List<Sprite> sprites)
    {
        int itemSprite = UnityEngine.Random.Range(0, sprites.Count);
        return sprites[itemSprite];
    }
}
