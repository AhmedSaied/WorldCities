namespace WorldCities.Data.Models
{
    public class City
    {
        #region Constructor
        public City()
        {

        }
        #endregion

        #region Properties
        public int Id { get; set; }

        public string Name { get; set; }

        public string Name_ASCII { get; set; }

        public decimal Lat { get; set; }
        public decimal Lon { get; set; }
        #endregion

        #region Relations And Navigation Properties
        public int CountryId { get; set; }

        public virtual Country Country { get; set; }
        #endregion
    }
}
