using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string characterSelectSceneName = "CharacterSelection";
    [SerializeField] private string gameplaySceneName = "Introduction";

    public static ServerManager Instance { get; private set; }

    private bool gameHasStarted;
    public Dictionary<ulong, ClientData> ClientData { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.OnServerStarted += OnNetworkReady;

        ClientData = new Dictionary<ulong, ClientData>();

        Debug.Log("Starting host");
        NetworkManager.Singleton.StartHost();
        Debug.Log("Started host");
    }

    public void StartServer()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.OnServerStarted += OnNetworkReady;

        ClientData = new Dictionary<ulong, ClientData>();

        NetworkManager.Singleton.StartServer();
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if (ClientData.Count >= 3 || gameHasStarted)
        {
            response.Approved = false;
            return;
        }

        response.Approved = true;
        response.CreatePlayerObject = false;
        response.Pending = false;

        ClientData[request.ClientNetworkId] = new ClientData(request.ClientNetworkId);

        Debug.Log($"Added client {request.ClientNetworkId}");
    }

    private void OnNetworkReady()
    {
        Debug.Log("NETWORK READY");
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;

        NetworkManager.Singleton.SceneManager.LoadScene(characterSelectSceneName, LoadSceneMode.Single);
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (ClientData.ContainsKey(clientId))
        {
            if (ClientData.Remove(clientId))
            {
                Debug.Log($"Removed client {clientId}");
            }
        }
    }

    public void SetCharacter(ulong clientId, int characterId)
    {
        Debug.Log("CHARACTER SET2");
        if (ClientData.TryGetValue(clientId, out ClientData data))
        {
            data.characterId = characterId;
            Debug.Log("CHARACTER SET");
        }
    }

    public void StartGame()
    {
        Debug.Log("STARTING GAME");
        gameHasStarted = true;

        NetworkManager.Singleton.SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);
    }
}
