using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;

namespace RagBlueprintAccelerator.Client.Models
{
    public class WebAssemblyTicket
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        //[Required]
        //public string Description { get; set; } = String.Empty;

        public IReadOnlyList<IBrowserFile> Attachments { get; set; } = new List<IBrowserFile>();
    }
}
