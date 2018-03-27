/******************************************************
 * Intuit sample app for Oauth2 using Intuit .Net SDK
 * ****************************************************/

//https://stackoverflow.com/questions/23562044/window-opener-is-undefined-on-internet-explorer/26359243#26359243
//IE issue- https://stackoverflow.com/questions/7648231/javascript-issue-in-ie-with-window-opener

using System;
using System.Configuration;
using System.Net;
using System.Web.UI;
using Intuit.Ipp.OAuth2PlatformClient;

namespace HelloWorldApp
{
    public partial class Default : System.Web.UI.Page
    {
        /// <summary>
        /// Page Load event
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.AsyncMode = true;
        }

        protected async void ImgC2QB_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                await getAuthorizationEndpoint();
                //Connect to QBO and redirect to your app's welcome page
                ConnectToQBOAndRedirect();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// To Initiate the OAuth 2 process redirect your App’s customer to Intuit's OAuth 2.0 server. 
        /// In order to retrieve Intuit's OAuth 2.0 server endpointor authorization_endpoint retrieve 
        /// the authorizationEndpoint URI from the discovery document.
        /// </summary>
        /// <returns>The async task</returns>
        private async System.Threading.Tasks.Task getAuthorizationEndpoint()
        {
            //Intialize DiscoverPolicy
            DiscoveryPolicy dpolicy = new DiscoveryPolicy();
            dpolicy.RequireHttps = true;
            dpolicy.ValidateIssuerName = true;

            DiscoveryClient discoveryClient = new DiscoveryClient(ConfigurationManager.AppSettings["discoveryUrl"]);
            DiscoveryResponse doc = await discoveryClient.GetAsync();

            if (doc.StatusCode == HttpStatusCode.OK)
            {
                Application["authorizationEndpoint"] = doc.AuthorizeEndpoint;
                Application["tokenEndpoint"] = doc.TokenEndpoint;
            }
            else
            {
                throw new ArgumentException("Invalid discoveryUrl", "discoveryUrl");
            }
        }

        /// <summary>
        /// Send Authorization using Oauth2 and then redirect the request
        /// </summary>
        /// <returns>Response reditect Task</returns>
        private void ConnectToQBOAndRedirect()
        {
            string stateVal = CryptoRandom.CreateUniqueId();
            string scopeVal = OidcScopes.Accounting.GetStringValue();

            string authorizationRequest = string.Format("{0}?client_id={1}&response_type=code&scope={2}&redirect_uri={3}&state={4}",
                    Application["authorizationEndpoint"],
                    ConfigurationManager.AppSettings["clientID"],
                    scopeVal,
                    System.Uri.EscapeDataString(ConfigurationManager.AppSettings["redirectURI"]),
                    stateVal);
            Response.Redirect(authorizationRequest);

        }
    }
}