using Firebase;
using Firebase.AppCheck;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseInit : MonoBehaviour
{
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result != DependencyStatus.Available)
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
                return;
            }

            // Initialize Firebase
            FirebaseApp app = FirebaseApp.DefaultInstance;
        });
    }
}
