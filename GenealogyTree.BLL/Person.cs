namespace BLL
{
    public class Person
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; }
        public Dictionary<string, List<Guid>> Relations { get; set; } = new();

        public Person(string fullName, DateTime birthDate, string gender)
        {
            Id = Guid.NewGuid();
            FullName = fullName;
            BirthDate = birthDate;
            Gender = gender;
        }

        public override string ToString() => $"{Id}:\t {FullName}\t\t ({Gender}, {BirthDate:yyyy-MM-dd})";
    }
}
