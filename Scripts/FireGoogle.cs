using System.Collections;
using UnityEngine;
using Firebase.Auth;
using Google;
using System.Threading.Tasks;
using Firebase.Extensions;
using TMPro;
using UnityEngine.UI;
using System;
using Firebase.Database;
using Firebase;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class FireGoogle : MonoBehaviour
{
    /// <summary>
    /// Script principal do Firebase, faz login e busca a data
    /// Utilizando o pacote da Asset Store da Unity, "Secured PlayerPrefs"
    /// </summary>
    public Button BtLogin, BtLogout;
    public string imageUrl, uidUserLogin, dateLoginUser;
    public GameObject OptionLogin;

    public string GoogleWebAPI = "1234COD-1cod.apps.googleusercontent.com";

    private GoogleSignInConfiguration configuration;

    FirebaseAuth auth;
    FirebaseUser user;
    public DatabaseReference reference;

    public FirebaseManager FireManagerScript;
    public FireInventory FireInventoryScript;

    public static FireGoogle Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;//Inicial do jogo - Autentication
    }
    void ShowLoginButton()
    {
        BtLogin.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "LOGIN";
        BtLogin.onClick.AddListener(() => OptionLogin.SetActive(true));
        Button btLogin = OptionLogin.transform.GetChild(0).GetComponent<Button>();
        btLogin.onClick.RemoveAllListeners();
        btLogin.onClick.AddListener(GoogleSignInClick);
    }

    void ShowPlayButton()
    {
        BtLogin.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "JOGAR";
        BtLogin.onClick.RemoveAllListeners();
        BtLogin.onClick.AddListener(GoogleSignInClick);
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)//
    {
        FireManagerScript = GetComponent<FirebaseManager>();
        FireInventoryScript = GetComponent<FireInventory>();

        BtLogin = GameObject.Find("BtLogin").GetComponent<Button>();
        OptionLogin = GameObject.Find("GetLogin").transform.GetChild(1).gameObject;


        configuration = new GoogleSignInConfiguration
        {
            WebClientId = GoogleWebAPI,
            RequestIdToken = true
        };
        StartCoroutine(InitFirebase());//Conecta com o banco de dados - Realtime Database

        if (!ZPlayerPrefs.HasKey("HasLoggedIn"))
        {
            // Não há registro de login, mostre o botão de Login
            ShowLoginButton();
        }
        else
        {
            // Existe um registro de login, mostre o botão de Jogar
            if (ZPlayerPrefs.GetInt("HasLoggedIn") == 1)
                ShowPlayButton();
            else
                ShowLoginButton();
        }
    }
    public void GoogleSignInClick()//Chamar com um botão, inicia o login
    {
        BtLogin.interactable = false;
        OptionLogin.SetActive(false);
        GameManager.Instance.WaitGame.SetActive(true);
        GoogleSignIn.Configuration=configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.RequestEmail = true;

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnGoogleAuthenticatedFinished);
    }
    public void OnLogoutButtonClicked()//Deslogar o usuario
    {
        FirebaseAuth.DefaultInstance.SignOut();
        ZPlayerPrefs.SetInt("HasLoggedIn", 0);
        // Atualize a UI após o logout
    }

    void OnGoogleAuthenticatedFinished(Task<GoogleSignInUser> task)
    {
        try
        {
            if (task.IsFaulted)
            {
                Debug.Log("Fault");
            }
            else if (task.IsCanceled)
            {
                Debug.LogError("Login Cancel");
            }
            else
            {
                Credential credential = GoogleAuthProvider.GetCredential(task.Result.IdToken, null);

                auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled)
                    {
                        Debug.LogError("SignInWithCredentialAsync was canceled");
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                        return;
                    }

                    user = auth.CurrentUser;

                    ZPlayerPrefs.SetInt("HasLoggedIn", 1);
                    ZPlayerPrefs.Save();

                    uidUserLogin = user.UserId;

                    StartCoroutine(GoMenuGame("Menu"));
                });
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }
    //Se preferir buscar a imagem de perfil do usuário pelo Google
    //private string CheckImageUrl(string url)
    //{
    //    if(!string.IsNullOrEmpty(url))
    //    {
    //        return url;
    //    }

    //    return imageUrl;
    //}
    //IEnumerator LoadImage(string imageUri)
    //{

    //    WWW www = new WWW(imageUri);
    //    yield return www;

    //    UserProfilePic.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
    //}
    IEnumerator InitFirebase()
    {
        var checkAndFixTask = FirebaseApp.CheckAndFixDependenciesAsync();
        yield return new WaitUntil(() => checkAndFixTask.IsCompleted);
        try
        {
            var dependencyStatus = checkAndFixTask.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;

                // Obtenha a instância do banco de dados com o URL fornecido
                reference = FirebaseDatabase.GetInstance("https://yourdatabase.firebaseio.com/").RootReference;

                auth = FirebaseAuth.DefaultInstance;
                BtLogin.interactable = true;
            }
            else
            {
                Debug.Log($"\nCould not resolve all Firebase dependencies: {dependencyStatus}");
                yield break; // Stop the coroutine here
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }
}
