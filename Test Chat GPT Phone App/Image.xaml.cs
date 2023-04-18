using Microsoft.Maui.Controls.Shapes;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Test_Chat_GPT_Phone_App.Models;

namespace Test_Chat_GPT_Phone_App;

public partial class Image : ContentPage
{
	public Image()
    {
		InitializeComponent();        
    }

    private async void NavigateToTextCompletionPage(object sender, EventArgs e)
    {
        ImageSlot.Source = "";
        ImageSlot.IsVisible = false;
        await Shell.Current.GoToAsync("//MainPage");
    }

    private async void SendImageRequestToChatGPTAsync(object sender, EventArgs e)
    {      
        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;
        string prompt = UserInputEntry.Text;
        UserInputEntry.Text = "";
        ResultLabel.Text = "";
        ImageSlot.IsVisible = false;
        ImageSlot.Source = "";

        string apiKey = "Bearer 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";
        string url = "https://api.openai.com/v1/images/generations";

        using (var httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", apiKey);

            var imagePrompt = new ImagePrompt { n = 1, prompt = prompt, size = "512x512" };

            string json = JsonConvert.SerializeObject(imagePrompt);

            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(url, content);

            if (response.StatusCode == HttpStatusCode.OK)
            {                
                var initialContent = await response.Content.ReadAsStringAsync();

                dynamic imageResponse = JsonConvert.DeserializeObject(initialContent);

                var imageUrl = Convert.ToString(imageResponse.data[0].url);

                ImageSlot.Source = imageUrl;
                ImageSlot.IsVisible = true;
                ImageSlot.Focus();
            }
            else
            {
                ResultLabel.Text = ($"Request failed with status code {response.StatusCode}.");                
            }
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }
    }
}