using System.ComponentModel.DataAnnotations;

namespace Obilet.JourneySearch.Web.Models.ViewModels;

public sealed class SearchJourneyViewModel
{
    [Required]
    public int? OriginId { get; set; }
    [Required]
    public string? OriginName { get; set; }

    [Required]
    public int? DestinationId { get; set; }
    [Required]
    public string? DestinationName { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime? DepartureDate { get; set; }
}
