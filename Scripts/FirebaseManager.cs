using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    /// <summary>
    /// Script Get e Set Firebased
    /// </summary>
    /// 
    int WhatsToday = 0;

    // Método para salvar informacao no Firebase
    // Método para ver o dia da premiacao atual - DAILY REWARD - Digamos que tenha uma premiacao diaria
    public void GetDailyReward(string userId)
    {
        FireGoogle.Instance.reference.Child("users").Child(userId).Child("dailyreward").GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("Erro em recuperar o dia!");
                SaveDailyReward(userId, 1);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log($"Dia do premio: {snapshot.Value}");
                WhatsToday = Convert.ToInt32(snapshot.Value);//Dia da premio
                if (WhatsToday < 7 && WhatsToday > 0)
                    WhatsToday++;
                else
                    WhatsToday = 1;

                DiaAtual = WhatsToday;

                SaveDailyReward(userId, WhatsToday);

            }
        });
    }
    // Método para salvar novo dia
    public void SaveDailyReward(string userId, int today)
    {
        FireGoogle.Instance.reference.Child("users").Child(userId).Child("dailyreward").SetValueAsync(today).ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("Erro ao salvar o dia atual");
            }
            else if (task.IsCompleted)
            {
                //Próximo passo depois de concluir, tipo, chamar o painel de premiacoes
            }
        });
    }
}
