
using System.ComponentModel.DataAnnotations;

namespace SmartTaskPlatform.Api.DTOs;

public class CreateTaskRequest
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(Low|Medium|High)$")]
    public string Priority { get; set; } = "Medium";
}
