using System.ComponentModel;
using UnityEngine;
using Mirror;

/// <summary>
/// An extension for the NetworkManager that displays a default HUD for controlling the network state of the game.
/// <para>This component also shows useful internal state for the networking system in the inspector window of the editor. It allows users to view connections, networked objects, message handlers, and packet statistics. This information can be helpful when debugging networked games.</para>
/// </summary>
[DisallowMultipleComponent]
[AddComponentMenu("Network/NetworkManagerHUD CCG")]
[RequireComponent(typeof(NetworkManager))]
[EditorBrowsable(EditorBrowsableState.Never)]
[HelpURL("https://mirror-networking.com/docs/Components/NetworkManagerHUD.html")]
public class NetworkManagerHUDCCG : MonoBehaviour
{
    NetworkManager manager;

    string username = "";

    /// <summary>
    /// Whether to show the default control HUD at runtime.
    /// </summary>
    public bool showGUI = true;

    /// <summary>
    /// The horizontal offset in pixels to draw the HUD runtime GUI at.
    /// </summary>
    public int offsetX;
    
    /// <summary>
    /// The vertical offset in pixels to draw the HUD runtime GUI at.
    /// </summary>
    public int offsetY;

    void Awake()
    {
        manager = GetComponent<NetworkManager>();

        // Set last username used in the username's input field
        if (PlayerPrefs.GetString("Name") != null) 
            username = PlayerPrefs.GetString("Name");
    }

    void OnGUI()
    {
        if (!showGUI)
            return;

       
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();
        }

        // client ready
        if (NetworkClient.isConnected && !ClientScene.ready)
        {
            if (GUILayout.Button("Client Ready"))
            {
                ClientScene.Ready(NetworkClient.connection);

                if (ClientScene.localPlayer == null)
                {
                    ClientScene.AddPlayer(NetworkClient.connection);
                }
            }
        }

        StopButtons();

       
    }

    void StartButtons()
    {
        GUIStyle yazi = new GUIStyle(GUI.skin.label);
        yazi.fontSize = 40;
        GUIStyle mystyle = new GUIStyle(GUI.skin.button);
        mystyle.fontSize = 40;
        GUIStyle usernameandport = new GUIStyle(GUI.skin.textField);
        usernameandport.fontSize = 40;
    
      
        if (!NetworkClient.active)
        {
            // Server + Client
            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                if (GUI.Button(new Rect(800, 450, 350, 80), "HOST GAME", mystyle))
                {
                    manager.StartHost(); //Starts a server and client in the application.

                    // Save the player's username
                    PlayerPrefs.SetString("Name", username);

                    // Hide GUI
                    showGUI = false;
                }
            }

            // Client
            
            if (GUI.Button(new Rect(800,550, 350, 80), "JOIN TO GAME",mystyle))
            {
                manager.StartClient();

                // Save the player's username
                PlayerPrefs.SetString("Name", username);

                // Hide GUI
                showGUI = false;
            }
            GUI.Label(new Rect(600, 375, 350, 50), "Port:", yazi);
            manager.networkAddress = GUI.TextField(new Rect(800,375, 350, 50), manager.networkAddress,usernameandport);


            // Username field
            GUI.Label(new Rect(600, 300, 350, 50), "Username:",yazi);
            username = GUI.TextField(new Rect(800,300,350, 50), username,usernameandport);


            // Server Only
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                // cant be a server in webgl build
                GUILayout.Box("(  WebGL cannot be server  )");
            }
            else
            {
                if (GUI.Button(new Rect(800,650, 350, 80), "RUN SERVER ",mystyle))
                {
                    manager.StartServer();
                }
                   
            }
        }
        else
        {
            // Connecting
            GUILayout.Label("Connecting to " + manager.networkAddress + "..");
            if (GUILayout.Button("Cancel Connection Attempt"))
            {
                manager.StopClient();
            }
        }
    }

    void StatusLabels()
    {
        // server / client status message
        if (NetworkServer.active)
        {
            GUILayout.Label("Server: active. Transport: " + Transport.activeTransport);
        }
        if (NetworkClient.isConnected)
        {
            GUILayout.Label("Client: address=" + manager.networkAddress);
        }
    }

    void StopButtons()
    {
        // stop host if host mode
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            if (GUILayout.Button("Stop Host"))
            {
                manager.StopHost();
            }
        }
        // stop client if client-only
        else if (NetworkClient.isConnected)
        {
            if (GUILayout.Button("Stop Client"))
            {
                manager.StopClient();
            }
        }
        // stop server if server-only
        else if (NetworkServer.active)
        {
            if (GUILayout.Button("Stop Server"))
            {
                manager.StopServer();
            }
        }
    }
}