using System.ComponentModel.DataAnnotations;

namespace TeamGlobal.Infrastructure.Model
{
    public class PortList
    {
        [Key]
        public int Id { get; set; }

        [StringLength(25)]
        public string Preference { get; set; }

        [StringLength(25)]
        public string OriginCode { get; set; }

        [StringLength(25)]
        public string DestinationCode { get; set; }

        [StringLength(25)]
        public string RoutingCode { get; set; }

        public int PreferenceOrder
        {
            get
            {
                switch (Preference)
                {
                    case "A":
                        return 1;

                    case "B":
                        return 1;

                    case "C":
                        return 1;

                    case "D":
                        return 1;

                    default:
                        return 10;
                }
            }
        }
    }
}