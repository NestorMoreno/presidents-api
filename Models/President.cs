
namespace PresidentsList.API.Models
{
    public class President
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Birthday { get; set; }
        public string BirthPlace { get; set; }
        public string DeathDay { get; set; }
        public string DeathPlace { get; set; }
        public string Country { get; set; }
        public string CreatedBy { get; set; }        
        public int Age { get; set; }
    }
}
