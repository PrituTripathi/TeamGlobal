using System.ComponentModel.DataAnnotations;

namespace TeamGlobal.Infrastructure.Model
{
    public class PortInfo
    {
        [Key]
        public int Id { get; set; }

        public string PORT_A { get; set; }
        public string PORT_B { get; set; }
        public string PORT_C { get; set; }
    }
}