using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace DemoWindowsFormsApplication
{
    /// <summary>
    /// Sample form with an embedded browser, to retrieve a bearer token from the EP-Online token endpoint.
    /// </summary>
    public partial class RetrieveTokenForm : Form
    {
        private readonly string tokenDomainUrl;
        private readonly IStringVisitor visitor;
        ChromiumWebBrowser browser;

        /// <summary>
        /// ctor of the form
        /// </summary>
        /// <param name="tokenDomainUrl">tokenEndpoint of the EP-Online token endpoint.</param>
        /// <param name="tokenEndpointPath"></param>
        /// <param name="visitor">Visitor that can be awaited for the bearer token.</param>
        public RetrieveTokenForm(string tokenDomainUrl, string tokenEndpointPath, IStringVisitor visitor)
        {
            InitializeComponent();
            InitBrowser($"{tokenDomainUrl}{tokenEndpointPath}");
            this.tokenDomainUrl = tokenDomainUrl;
            this.visitor = visitor;
        }

        private void InitBrowser(string url)
        {
            browser = new ChromiumWebBrowser(url);
            browser.AddressChanged += BrowserAddressChanged;

            // init quick fixes as long as TVS has not configured the right return urls
            browser.LoadingStateChanged += browser_LoadingStateChanged;
            browser.LoadError += Browser_LoadError;
            //

            Controls.Add(browser);
            browser.Dock = DockStyle.Fill;
        }


        /// <summary>
        /// Event handler that watches the redirects and kicks in when the original tokenEndpoint returns.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void BrowserAddressChanged(object sender, AddressChangedEventArgs eventArgs)
        {
            if (eventArgs.Address.StartsWith(tokenDomainUrl))
            {
               browser.LoadingStateChanged += (s, e) =>
               {
                   if (!e.IsLoading)
                    browser.GetMainFrame().GetText(visitor);
               };
            }
        }

        /// <summary>
        /// Quick fix needed to correct the return tokenEndpoint as long as TVS doesn't have to correct return tokenEndpoint configured.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (e.IsLoading == false && e.Browser.MainFrame.Url.StartsWith("http://localhost/"))
            {
                RedirectToSigninTvsHack(e.Browser.MainFrame.Url);
            }
        }

        /// <summary>
        /// Quick fix needed to correct the return tokenEndpoint as long as TVS doesn't have to correct return tokenEndpoint configured.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Browser_LoadError(object sender, LoadErrorEventArgs e)
        {
            if (e.FailedUrl.StartsWith("http://localhost/"))
                RedirectToSigninTvsHack(e.FailedUrl);
        }

        private void RedirectToSigninTvsHack(string url)
        {
            var indexOfCodeQueryString = url.IndexOf("?code");
            var correctedUrl = $"{tokenDomainUrl}/signin-tvs{url.Substring(indexOfCodeQueryString)}";
            browser.Load(correctedUrl);
        }
    }
}

