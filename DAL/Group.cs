namespace DAL
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> Students { get; set; }
        public List<string> Subjects { get; set; }
        public List<List<List<int>>> Marks { get; set; }
        // [[[0 - mark] - subject] - student]
        public Group()
        {
        }
        public Group(int id, string name, List<int> students, List<string> subjects, List<List<List<int>>> marks)
        {
            Id = id;
            Name = name;
            Students = students;
            Subjects = subjects;
            Marks = marks;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
