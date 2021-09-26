using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WorldCities.Data.Models
{
    public class Country
    {
        #region Contructor
        public Country()
        {

        }
        #endregion

        #region Properties
        public int Id { get; set; }

        public string Name { get; set; }

        [JsonPropertyName("iso2")]
        public string ISO2 { get; set; }

        [JsonPropertyName("iso3")]
        public string ISO3 { get; set; }

        #endregion

        #region Client-side properties
        //the number of cities related to this country
        [NotMapped]
        public int TotCities
        {
            get
            {
                return (Cities != null) ? Cities.Count : _TotCities;
            }
            set
            {
                _TotCities = value;
            }
        }

        private int _TotCities = 0;
        #endregion

        #region Relations And Navigation Properties
        [JsonIgnore]
        public List<City> Cities { get; set; }
        #endregion
    }
}
