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
        private readonly string url;
        private readonly IStringVisitor visitor;
        ChromiumWebBrowser browser;

        /// <summary>
        /// ctor of the form
        /// </summary>
        /// <param name="url">url of the EP-Online token endpoint.</param>
        /// <param name="visitor">Visitor that can be awaited for the bearer token.</param>
        public RetrieveTokenForm(string url, IStringVisitor visitor)
        {
            InitializeComponent();
            InitBrowser(url);
            this.url = url;
            this.visitor = visitor;
        }

        private void InitBrowser(string url)
        {
            browser = new ChromiumWebBrowser(url);
            browser.LoadingStateChanged += browser_LoadingStateChanged;
            browser.AddressChanged += BrowserAddressChanged;
            Controls.Add(browser);
            browser.Dock = DockStyle.Fill;
        }

        /// <summary>
        /// Event handler that watches the redirects and kicks in when the original url returns.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void BrowserAddressChanged(object sender, AddressChangedEventArgs eventArgs)
        {
            if (eventArgs.Address == url)
            {
                browser.LoadingStateChanged +=  (s, e) =>
                {
                    if (!e.IsLoading)
                    {
                        browser.GetMainFrame().GetText(visitor);
                    }
                };
            }
        }

        /// <summary>
        /// Quick fix needed to correct the return url as long as TVS doesn't have to correct return url configured.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (e.IsLoading == false)
            {
                if (e.Browser.MainFrame.Url.StartsWith("http://localhost/"))
                {
                    var codepos = e.Browser.MainFrame.Url.IndexOf("?code");
                    var url = $"http://localhost:56000/signin-tvs{e.Browser.MainFrame.Url.Substring(codepos)}";
                    browser.Load(url);
                }
            }
        }
    }
}
