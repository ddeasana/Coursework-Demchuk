namespace DAL
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public Student()
        {
        }
        public Student(int id, string name, string surname)
        {
            Id = id;
            Name = name;
            Surname = surname;
        }
        public override string ToString()
        {
            return Name + " " + Surname;
        }
    }
}
