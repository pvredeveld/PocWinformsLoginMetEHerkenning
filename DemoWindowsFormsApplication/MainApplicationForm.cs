using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace DemoWindowsFormsApplication
{
    public partial class MainApplicationForm : Form
    {
        private string apiHostUrl;

        public MainApplicationForm()
        {
            InitializeComponent();
            Cef.Initialize(new CefSettings());
        }

        private async void GetTokenButtonClick(object sender, EventArgs e)
        {
            if (!IsApiHostUrlSet())
                return;

            var visitor = new TaskStringVisitor();
            using (var frm = new RetrieveTokenForm($"{apiHostUrl}","/token", visitor))
            {
                frm.Show(this);
                var bearer = await visitor.Task;
                bearerTextBox.Text = bearer;
            }
        }

        private bool IsApiHostUrlSet()
        {
            apiHostUrl = hostTextBox.Text;
            if (string.IsNullOrWhiteSpace(apiHostUrl))
            {
                MessageBox.Show("API host url is verreist.", "Fout", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            return true;
        }

        private async void ToevoegenButtonClick(object sender, EventArgs e)
        {
            if (!IsApiHostUrlSet())
                return;

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerTextBox.Text);

            var response = await httpClient.PostAsync($"{apiHostUrl}/api/v1/Energielabels/toevoegen", new StringContent("<EPC></EPC>", Encoding.UTF8, "application/xml"));
            MessageBox.Show(response.StatusCode.ToString(), "Toevoegen");
        }

        private async void UitbreidenButtonClick(object sender, EventArgs e)
        {
            if (!IsApiHostUrlSet())
                return;

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerTextBox.Text);

            var response = await httpClient.PostAsync($"{apiHostUrl}/api/v1/Energielabels/uitbreiden", new StringContent("<EPC></EPC>", Encoding.UTF8, "application/xml"));
            MessageBox.Show(response.StatusCode.ToString(), "Uitbreiden");
        }

        private async void VervangenButtonClick(object sender, EventArgs e)
        {
            if (!IsApiHostUrlSet())
                return;

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerTextBox.Text);

            var response = await httpClient.PostAsync($"{apiHostUrl}/api/v1/Energielabels/vervangen", new StringContent("<EPC></EPC>", Encoding.UTF8, "application/xml"));
            MessageBox.Show(response.StatusCode.ToString(), "Vervangen");
        }
    }
}
