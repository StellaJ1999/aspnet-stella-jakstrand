using System.ComponentModel.DataAnnotations;

namespace Presentation.WebApp.Areas.Admin.Models;

public class TrainingSessionForm : IValidatableObject
{
    [Required(ErrorMessage = "Name is required")]
    [DataType(DataType.Text)]
    [StringLength(200, ErrorMessage = "Name cannot be longer than 200 characters")]
    [Display(Name = "Name", Prompt = "Enter session name")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Start time is required")]
    [DataType(DataType.DateTime)]
    [Display(Name = "Start time", Prompt = "Select start time")]
    public DateTime StartTime { get; set; }

    [Required(ErrorMessage = "End time is required")]
    [DataType(DataType.DateTime)]
    [Display(Name = "End time", Prompt = "Select end time")]
    public DateTime EndTime { get; set; }

    [Required(ErrorMessage = "Max participants is required")]
    [Range(1, 500, ErrorMessage = "Max participants must be between 1 and 500")]
    [Display(Name = "Max participants", Prompt = "20")]
    public int MaxParticipants { get; set; } = 20;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartTime >= EndTime)
        {
            yield return new ValidationResult(
                errorMessage: "Start time must be before end time.",
                memberNames: new[] { nameof(StartTime), nameof(EndTime) }
            );
        }
    }
}
