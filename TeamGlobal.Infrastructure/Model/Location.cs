using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamGlobal.Infrastructure.Model
//{
//    public class Location
//    {
//        public int Id { get; set; }

//        [StringLength(100)]
//        public string Code { get; set; }

//        [StringLength(100)]
//        public string Name { get; set; }

//        [StringLength(100)]
//        public string CountryCode { get; set; }

//        [StringLength(100)]
//        public string CountryName { get; set; }

//        [StringLength(100)]
//        public string EIDPortCode { get; set; }

//        [StringLength(100)]
//        public string TransportMode { get; set; }

//        [StringLength(100)]
//        public string LOVStatus { get; set; }

//        public bool IsActive { get; set; }

//        //CODE VARCHAR(100),
//        //NAME VARCHAR(100),
//        //COUNTRY_CODE VARCHAR(100),
//        //COUNTRY_NAME VARCHAR(100),
//        //EDI_PORT_CODE VARCHAR(100),
//        //TRANSPORT_MODE VARCHAR(100),
//        //LOV_STATUS VARCHAR(100)
//    }
{
    [Table("Location")]
    public partial class Location
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string CODE { get; set; }

        [StringLength(100)]
        public string NAME { get; set; }

        [StringLength(100)]
        public string CountryCode { get; set; }

        [StringLength(100)]
        public string CountryName { get; set; }

        [StringLength(100)]
        public string EIDPortCode { get; set; }

        [StringLength(100)]
        public string TransportMode { get; set; }

        [StringLength(100)]
        public string LOVStatus { get; set; }
    }
}