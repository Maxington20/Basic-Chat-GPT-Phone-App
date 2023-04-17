using System.Net;
using System.Text;
using System;
using Newtonsoft.Json;
using Test_Chat_GPT_Phone_App.Models;

namespace Test_Chat_GPT_Phone_App;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
        BindingContext = this;
	}

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            if (_isLoading != value)
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }
    }

    private double _loadingProgress;
    public double LoadingProgress
    {
        get => _loadingProgress;
        set
        {
            if (_loadingProgress != value)
            {
                _loadingProgress = value;
                OnPropertyChanged(nameof(LoadingProgress));
            }
        }
    }


    private void OnCounterClicked(object sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}

    private async void SendTextToChatGPTAsync(object sender, EventArgs e)
    {
        string text = UserInputEntry.Text;

        string apiKey = "Bearer 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";
        string url = "https://api.openai.com/v1/chat/completions";

        int retries = 5;

        while (retries > 0)
        {
            try
            {
                UserInputEntry.Text = "";
                IsLoading = true;
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;
                LoadingProgress = 0;
                ResultLabel.Text = "";

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", apiKey);

                    var body = new DataBody { model = "gpt-3.5-turbo", messages = new List<Message>() };

                    body.messages.Add(new Message { role = "user", content = text });

                    string json = JsonConvert.SerializeObject(body);

                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");                 

                    var response = await httpClient.PostAsync(url, content);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var firstResult = await response.Content.ReadAsStringAsync();
                        dynamic result = JsonConvert.DeserializeObject(firstResult);
                        var finalResul = result?.choices[0]?.message?.content ?? null;

                        ResultLabel.Text = finalResul;
                        retries = 0;

                        LoadingProgress = 1;
                        LoadingIndicator.IsVisible = false;

                        return;
                    }
                    else if (retries > 1)
                    {
                        ResultLabel.Text = ($"Request failed with status code {response.StatusCode}. Retrying {retries - 1} more times.");
                        retries--;
                        return;
                    }
                    else
                    {
                        ResultLabel.Text = ($"Request failed with status code {response.StatusCode} after retrying 5 times.");
                        return;
                    }
                }
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                if (retries > 1)
                {
                    ResultLabel.Text = ($"Request timed out. Retrying {retries - 1} more times.");
                    retries--;
                    return;
                }
                else
                {
                    ResultLabel.Text = ("Request timed out after retrying 5 times.");
                }
            }
            catch (Exception ex)
            {
                ResultLabel.Text = ($"An error occurred: {ex.Message}");
            }
            finally
            {
                // Hide the loading indicator
                LoadingProgress = 1;
                LoadingIndicator.IsVisible = false;
                IsLoading = false;
            }
        }

        ResultLabel.Text = ("Failed to get a response after retrying 5 times.");
    }
}

