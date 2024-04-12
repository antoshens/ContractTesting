using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Consumer.Data
{
    public class CountryFilter
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int Limit { get; set; }

        [Required]
        [DefaultValue(SortOrder.asc)]
        public SortOrder SortOrder { get; set; }

        [Required]
        public int Population { get; set; }
    }

    public enum SortOrder : byte
    {
        asc,
        desc,
    }
}
