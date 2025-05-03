using System.ComponentModel.DataAnnotations;

namespace FeedbackApp.Models;
public class Feedback{
    [Required(ErrorMessage = "Need your name")]
    public string Name { get; set; }

    [EmailAddress(ErrorMessage = "No email address no feedback")]
    public string Email { get; set; }

    [StringLength(maximumLength: 500, MinimumLength = 10, ErrorMessage = "Enter between 10 and 500 chars")]
    [Required]
    public string Comment { get; set; }
}