using DAL;

namespace BLL
{
    public class GroupService
    {
        List<Group> data = new();
        readonly JSONProvider<Group> db = new("groups.json");
        public GroupService()
        {
            data = db.Load();
        }
        public Group Insert(string name, List<int> students, List<string> subjects, List<List<List<int>>> marks)
        {
            int NewId = 1;
            try
            {
                NewId = data[^1].Id + 1;
            }
            catch (Exception) { }
            Group newCategory = new(NewId, name, students, subjects, marks);
            data.Add(newCategory);
            db.Save(data);
            return newCategory;
        }
        public Group? FindById(int Id)
        {
            return data.Find(el => el.Id == Id);
        }
        public Group? UpdateById(int Id, Group group)
        {
            int groupIndex = data.FindIndex(el => el.Id == Id);
            if (groupIndex != -1)
            {
                Group oldGroup = data[groupIndex];
                data[groupIndex] = group;
                db.Save(data);
                return oldGroup;
            }
            return null;
        }
        public static Group AddStudent(Group group, int studentId)
        {
            group.Students.Add(studentId);
            List<List<int>> subjectMarks = new();
            group.Subjects.ForEach(el => subjectMarks.Add(new()));
            group.Marks.Add(subjectMarks);
            return group;
        }
        public static Group RemoveStudent(Group group, int studentId)
        {
            int studentIndex = group.Students.FindIndex(el => el == studentId);
            if (studentIndex != -1)
            {
                group.Students.RemoveAt(studentIndex);
                group.Marks.RemoveAt(studentIndex);
            }
            return group;
        }
        public static Group AddSubject(Group group, string subject)
        {
            group.Subjects.Add(subject);
            group.Marks.ForEach(el => el.Add(new()));
            return group;
        }
        public static Group RemoveSubject(Group group, string subject)
        {
            int subjectIndex = group.Subjects.FindIndex(el => el == subject);
            if (subjectIndex != -1)
            {
                group.Subjects.RemoveAt(subjectIndex);
                group.Marks.ForEach(el => el.RemoveAt(subjectIndex));
            }
            return group;
        }
        public static Group AddMark(Group group, int studentId, string subject, int mark)
        {
            int studentIndex = group.Students.FindIndex(el => el == studentId);
            if (studentIndex != -1)
            {
                int subjectIndex = group.Subjects.FindIndex(el => el == subject);
                if (subjectIndex != -1)
                {
                    group.Marks[studentIndex][subjectIndex].Add(mark);
                }
            }
            return group;
        }
        public static Group SetMarks(Group group, int studentId, string subject, List<int> marks)
        {
            int studentIndex = group.Students.FindIndex(el => el == studentId);
            if (studentIndex != -1)
            {
                int subjectIndex = group.Subjects.FindIndex(el => el == subject);
                if (subjectIndex != -1)
                {
                    group.Marks[studentIndex][subjectIndex] = marks;
                }
            }
            return group;
        }
        public static Group RemoveMark(Group group, int studentId, string subject, int markIndex)
        {
            int studentIndex = group.Students.FindIndex(el => el == studentId);
            if (studentIndex != -1)
            {
                int subjectIndex = group.Subjects.FindIndex(el => el == subject);
                if (subjectIndex != -1)
                {
                    group.Marks[studentIndex][subjectIndex].RemoveAt(markIndex);
                }
            }
            return group;
        }
        public void DeleteStudent(int studentId)
        {
            data.ForEach(group =>
            {
                int studentIndex = group.Students.FindIndex(el => el == studentId);
                if (studentIndex != -1)
                {
                    group.Students.RemoveAt(studentIndex);
                    group.Marks.RemoveAt(studentIndex);
                }
            });
            db.Save(data);
        }
        public List<Group> SearchByStudent(int studentId)
        {
            List<Group> groups = new();
            GroupService groupService = new();
            data.ForEach(group =>
            {
                int studentIndex = group.Students.FindIndex(el => el == studentId);
                if (studentIndex != -1)
                {
                    groups.Add(group);
                }
            });
            return groups;
        }
        public List<Tuple<Group, List<Student>>> SearchByAvg(int avg, StudentService studentService)
        {
            List<Tuple<Group, List<Student>>> results = new();

            data.ForEach(group =>
            {
                List<Student> students = new();
                group.Students.ForEach(studentId =>
                {
                    Student? student = studentService.FindById(studentId);
                    if (student != null)
                    {
                        int studentIndex = group.Students.FindIndex(el => el == studentId);

                        int avgMark = 0;
                        for (int i = 0; i < group.Subjects.Count; i++)
                        {
                            if (group.Marks[studentIndex][i].Count > 0)
                            {
                                avgMark += group.Marks[studentIndex][i].Aggregate(0, (acc, x) => acc + x) / group.Marks[studentIndex][i].Count;
                            }
                        }
                        if (group.Subjects.Count > 0)
                        {
                            avgMark /= group.Subjects.Count;
                        }

                        if (avgMark == avg)
                        {
                            students.Add(student);
                        }
                    }
                });
                if (students.Count != 0)
                {
                    results.Add(new Tuple<Group, List<Student>>(group, students));
                }
            });

            return results;
        }
        // [ { studentId, subjectMark } ]
        public static List<Tuple<int, int>> StudentsMarksOnSubject (Group group, int subjectIndex)
        {
            List<Tuple<int, int>> results = new();

            for(int i = 0; i < group.Students.Count; i++)
            {
                int studentId = group.Students[i];
                int avgMark = 0;
                if (group.Marks[i][subjectIndex].Count > 0)
                {
                    avgMark = group.Marks[i][subjectIndex].Aggregate(0, (acc, x) => acc + x) / group.Marks[i][subjectIndex].Count;
                }
                results.Add(new Tuple<int, int>(studentId, avgMark));
            }

            return results;
        }
        // [ { studentId, mark } ]
        public static List<Tuple<int, int>> StudentsMarks(Group group)
        {
            List<Tuple<int, int>> results = new();

            for (int studentIndex = 0; studentIndex < group.Students.Count; studentIndex++)
            {
                int studentId = group.Students[studentIndex];
                int avgMark = 0;

                for(int subjectIndex = 0; subjectIndex < group.Subjects.Count; subjectIndex++)
                {
                    if (group.Marks[studentIndex][subjectIndex].Count > 0)
                    {
                        avgMark += group.Marks[studentIndex][subjectIndex].Aggregate(0, (acc, x) => acc + x) / group.Marks[studentIndex][subjectIndex].Count;
                    }
                }
                if (group.Subjects.Count > 0)
                {
                    avgMark /= group.Subjects.Count;
                }

                results.Add(new Tuple<int, int>(studentId, avgMark));
            }

            return results;
        }
        public Group? DeleteById(int Id)
        {
            Group? group = data.Find(el => el.Id == Id);
            if (group != null)
            {
                data.Remove(group);
                db.Save(data);
            }
            return group;
        }
        public List<Group> Groups => data;
    }
}
