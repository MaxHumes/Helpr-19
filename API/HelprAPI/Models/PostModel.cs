using System.Device.Location;
namespace HelprAPI.Models
{
    public class PostModel
    {
        public int post_id { get; set; }
        public int? thread_id { get; set; }
        public int? user_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        //distance is in meters
        public double? dist_from_user { get; set; }

        public PostModel() { }

        public GeoCoordinate GetGeoCoordinates()
        {
            if ((latitude.HasValue && longitude.HasValue) && (latitude < 500 && longitude < 500))
            {
                return new GeoCoordinate((double)latitude, (double)longitude);
            }
            return null;
        }

        public void SetDistFromUser(GeoCoordinate userLocation)
        {
            dist_from_user = this.GetGeoCoordinates().GetDistanceTo(userLocation);
        }

        public bool IsValidPost()
        {
            if (!thread_id.HasValue || !user_id.HasValue || name == null || description == null)
            {
                return false;
            }

            return true;
        }
    }
}
