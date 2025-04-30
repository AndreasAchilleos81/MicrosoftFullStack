namespace FeedbackApp.Services;
using System.Text.Json;
using FeedbackApp.Models;
using Microsoft.JSInterop;
public class FeedbackService
{
    private List<Feedback> _feedbacks;
    private readonly IJSRuntime _jsRuntime;
    private const string FeedbackKey = "feedbacks";

    public FeedbackService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        _feedbacks = new List<Feedback>();
    }
    public FeedbackService()
    {
        _feedbacks = new List<Feedback>();
    }

    public async Task AddFeedback(Feedback feedback)
    {
        if (feedback == null)
        {
            throw new ArgumentNullException(nameof(feedback));
        }
        
        List<Feedback> feedbacks = await GetAllFeedbacks();        
        feedbacks.Add(feedback);
        var feedbacksJson = JsonSerializer.Serialize<List<Feedback>>(feedbacks);
        await _jsRuntime.InvokeAsync<object>("localStorage.setItem", FeedbackKey, feedbacksJson);
    }

    public async Task<List<Feedback>> GetAllFeedbacks()
    {
        string feedbacksJson = null;
        try
        {
          feedbacksJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", FeedbackKey);
        }
        catch
        {
            Console.WriteLine($"No Feedbacks found in local storage."); 
            return new List<Feedback>();
        }
        
         if (string.IsNullOrEmpty(feedbacksJson)){
            return new List<Feedback>();
         }

        _feedbacks = JsonSerializer.Deserialize<List<Feedback>>(feedbacksJson);
        return _feedbacks;
    }
}