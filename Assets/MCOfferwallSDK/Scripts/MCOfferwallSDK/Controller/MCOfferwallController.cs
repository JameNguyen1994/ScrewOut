using Assets.Scripts.MCOfferwallSDK.Service;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MCOfferwallController : MonoBehaviour
{
    UniWebView webView;
    UriBuilderService uriBuilderService = new UriBuilderService();
    UserService userService = new UserService();

    public delegate void OfferwallClosedEventHandler();
    public event OfferwallClosedEventHandler OnOfferwallClosed;

    private void Awake()
    {
        InitializeWebView();
    }

    private void InitializeWebView()
    {
        if (webView == null)
        {

            UniWebView.SetAllowAutoPlay(true);
            UniWebView.SetAllowInlinePlay(true);
            
            var webViewGameObject = new GameObject("UniWebView");
            webView = webViewGameObject.AddComponent<UniWebView>();
            webViewGameObject.transform.SetParent(transform, false); // Parent to this GameObject, keeping the local positioning.
            


            webView.SetBouncesEnabled(false);
            webView.SetCacheMode(UniWebViewCacheMode.NoCache);
            webView.SetUseWideViewPort(true);
            webView.EmbeddedToolbar.Show();
            webView.EmbeddedToolbar.HideNavigationButtons();
            webView.EmbeddedToolbar.SetDoneButtonText("Close");

            UpdateWebViewFrame();

 
            webView.AddUrlScheme("mychips");





            RegisterEvents();
        }
    }

    public void RegisterEvents()
    {

        if (webView != null)
        {
            // Register events
            webView.OnPageFinished += OnPageFinished;
            webView.OnShouldClose += OnShouldClose;
            webView.OnMessageReceived += OnMessageReceived;
            webView.OnLoadingErrorReceived += WebView_OnLoadingErrorReceived;
        }
    }


    public void UnregisterEvents()
    {
        if (webView != null)
        {
            webView.OnPageFinished -= OnPageFinished;
            webView.OnShouldClose -= OnShouldClose;
            webView.OnMessageReceived -= OnMessageReceived;
            webView.OnLoadingErrorReceived -= WebView_OnLoadingErrorReceived;
        }
    }


    private void WebView_OnLoadingErrorReceived(UniWebView webView, int errorCode, string errorMessage, UniWebViewNativeResultPayload payload)
    {
        string htmlData =
         "<html>" +
         "<head>" +
         "<meta name='viewport' content='width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no'>" +
         "<style>" +
         "body { font-size: 16pt; font-family: Arial, sans-serif; margin: 0; padding: 0; display: flex; justify-content: center; align-items: center; height: 100%; background-color: #f7f7f7; }" +
         "div { text-align: center; width: 100%; box-sizing: border-box; padding: 10px; }" +
         "h1 { color: #333; }" +
         "p { color: #666; }" +
         "@media (min-width: 768px) { body { font-size: 30pt; } }" + // Adjust font size for larger screens
         "</style></head>" +
         "<body>" +
         "<div>" +
         "<h1>Connection Error</h1>" +
         "<p>Sorry, we're unable to load the offers.<br>Please check your connection and try again.</p>" +
         "</div>" +
         "</body></html>";

        webView.LoadHTMLString(htmlData, "");
    }
    

    private void OnPageFinished(UniWebView webView, int statusCode, string url)
    {
        webView.SetOpenLinksInExternalBrowser(true);
        Debug.Log("Page Finished Loading: " + url + ", Status Code: " + statusCode);
    }

    private bool OnShouldClose(UniWebView webView)
    {

   

        // Optionally reset or perform clean up tasks here
        Debug.Log("WebView is closing");
        webView.Hide();  // Just hide the web view instead of destroying
                         // Clear the history and reset the state of the web view
       

        webView.Load("about:blank"); // Load a blank page to clear the current URL

        OnOfferwallClosed?.Invoke();
        return false;    // Prevent UniWebView from actually closing, since we're just hiding it
    }

    // Update method to check for screen size changes
    private void Update()
    {
        if (webView != null)
        {
            //handling Rotation
            if (Screen.width != webView.Frame.width || Screen.height != webView.Frame.height)
            {
                UpdateWebViewFrame();
            }

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (webView.Url.Contains("page=home") || !webView.Url.Contains("page="))
                {
                    Hide();
                }

                //ShowToast(webView.Url.Substring(140, webView.Url.Length - 140));
            }
        }

     
    }

    private void UpdateWebViewFrame()
    {
        if (webView != null)
        {
            webView.Frame = new Rect(0, 0, Screen.width, Screen.height);
            webView.UpdateFrame();
        }
    }

    private void OnMessageReceived(UniWebView webView, UniWebViewMessage message)
    {
        Debug.Log("Message received from WebView: " + message.RawMessage);
    }

    // Original method kept for backward compatibility.
    public void Show(string adUnitId)
    {
        // Call the new method with null for additional parameters.
        Show(adUnitId, null);
    }

    // New overloaded method.
    public void Show(string adUnitId, QueryParameterBuilder additionalParams)
    {
        RateLimitService.ResetSlidingWindow("getBalance");

        // Create a new QueryParameterBuilder for constructing the final query parameters.
        QueryParameterBuilder queryParameter = new QueryParameterBuilder();
    
        // Set the mandatory fields.
        queryParameter.UserId = userService.GetOrCreateId();
        queryParameter.IDFA = userService.GetIDFA();
        queryParameter.GAID = userService.GetGAID();
        queryParameter.Age = userService.GetAge();          
        queryParameter.Gender = userService.GetGender();
        queryParameter.AffSub1 = userService.GetAffSub1();
        queryParameter.AffSub2 = userService.GetAffSub2();
        queryParameter.AffSub3 = userService.GetAffSub3();
        queryParameter.AffSub4 = userService.GetAffSub4();
        queryParameter.AffSub5 = userService.GetAffSub5();
        queryParameter.AdUnitId = adUnitId;

        // Merge additional custom parameters if provided.
        if (additionalParams != null)
        {
            foreach (var kv in additionalParams.GetCustomParameters())
            {
                // In case of duplicate keys, the value from additionalParams overwrites the existing one.
                queryParameter.SetParam(kv.Key, kv.Value);
            }
        }

        // Build the final URL using the queryParameter and load it into the web view.
        string url = uriBuilderService.BuildOfferwallUrl(queryParameter);
        webView.SetOpenLinksInExternalBrowser(false);
        webView.Load(url);
        webView.Show();
        Debug.Log("Showing Offerwall: " + url);
    }


    public void Hide()
    {
        webView.Hide();    // Hide the web view
        Debug.Log("Hiding Offerwall");
    }

    private void OnDestroy()
    {
        // Clean up by unregistering event handlers
        UnregisterEvents();
    }


}
