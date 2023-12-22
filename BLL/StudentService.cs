using DAL;

namespace BLL
{
    public class StudentService
    {
        List<Student> data = new();
        readonly JSONProvider<Student> db = new("students.json");
        public StudentService()
        {
            data = db.Load();
        }
        public Student Insert(string name, string surname)
        {
            int NewId = 1;
            try
            {
                NewId = data[^1].Id + 1;
            }
            catch (Exception) { }
            Student newStudent = new(NewId, name, surname);
            data.Add(newStudent);
            db.Save(data);
            return newStudent;
        }
        public Student? FindById(int Id)
        {
            return data.Find(el => el.Id == Id);
        }
        public Student? UpdateById(int Id, Student student)
        {
            int studentIndex = data.FindIndex(el => el.Id == Id);
            if (studentIndex != -1)
            {
                Student oldStudent = data[studentIndex];
                data[studentIndex] = student;
                db.Save(data);
                return oldStudent;
            }
            return null;
        }
        public Student? DeleteById(int Id)
        {
            Student? student = data.Find(el => el.Id == Id);
            if (student != null)
            {
                data.Remove(student);
                db.Save(data);
            }
            return student;
        }
        public List<Student> Search(string query)
        {
            return data.FindAll(el => el.Name.ToLower().Contains(query.ToLower()) || el.Surname.ToLower().Contains(query.ToLower()));
        }
        public List<Student> Students => data;
    }
}
