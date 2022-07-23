using MyWebServices.Core.DataAccess.Entities;
using System.Text.Json.Serialization;

namespace MyWebServices.Core.Models
{
    public class UpdatedSettings
    {
        [JsonPropertyName("settings")]
        public UserSettingsEntity UserSettingsEntity { get; set; } = null!;

        public IEnumerable<CustomUserElement>? AddedElements { get; set; }
        public IEnumerable<CustomUserElement>? RemovedElements { get; set; }
        public IEnumerable<CustomUserElement>? UpdatedElements { get; set; }
    }
}
