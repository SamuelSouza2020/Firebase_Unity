using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireUserAward : MonoBehaviour
{
    public int HowManyChests = 0;
    public List<ChestData> MyChest;//ScriptableObject
    public void GetChosenChest(string userId, int whatChest)
    {
        string chestName = WhatIsTheChest(whatChest);
        FireGoogle.Instance.reference.Child("users").Child(userId).Child("chest").Child(chestName).GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("Erro Get Chest");
                SaveChestFB(userId, whatChest, 0);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                HowManyChests = Convert.ToInt32(snapshot.Value);
                MyChest[whatChest].HowManyChest = HowManyChests;
            }
        });
    }
    public void SaveChestFB(string userId, int whatChest, int howMany)
    {
        string chestName = WhatIsTheChest(whatChest);
        HowManyChests += howMany;
        FireGoogle.Instance.reference.Child("users").Child(userId).Child("chest").Child(chestName).SetValueAsync(HowManyChests).ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("Erro Save Chest");
                HowManyChests -= howMany;
            }
            else if (task.IsCompleted)
            {
                MyChest[whatChest].HowManyChest = HowManyChests;
            }
        });
    }
    string WhatIsTheChest(int numberCall)
    {
        string nameChest = "chestlendario";

        switch(numberCall)
        {
            case 0:
                nameChest = "chestlendario";
                break;
            case 1:
                nameChest = "chestepico";
                break;
            case 2:
                nameChest = "chestraro";
                break;
            case 3:
                nameChest = "chestcomum";
                break;
            case 4:
                nameChest = "chestdiamante";
                break;
        }
        return nameChest;
    }
}
