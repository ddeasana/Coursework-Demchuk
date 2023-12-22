using BLL;
using DAL;

namespace PL
{
    public class Menu
    {
        GroupService groups = new();
        StudentService students = new();
        public void MainMenu()
        {
            int page = 1;
            int itemsPerPage = 20;
            while (true)
            {
                Console.Clear();
                int maxPage = groups.Groups.Count < itemsPerPage ? 1 : (groups.Groups.Count / itemsPerPage);
                Console.WriteLine($"Groups: {groups.Groups.Count}, page {page} of {maxPage}");
                int groupNumber = 0;
                foreach (Group group in groups.Groups)
                {
                    if (groupNumber >= (page - 1) * itemsPerPage && groupNumber < page * itemsPerPage)
                    {
                        Console.WriteLine($"{group.Id}. {group.Name}");
                    }
                    groupNumber++;
                }
                Console.WriteLine("0. Exit ; q. Add group ; del. Delete group ; pX. Set page ; s. Students list");
                while (true)
                {
                    string? groupId = Console.ReadLine();
                    if (string.IsNullOrEmpty(groupId)) { continue; }
                    if (groupId == "0") { break; }
                    if (groupId == "s")
                    {
                        StudentsMenu();
                        break;
                    }
                    if (groupId == "q")
                    {
                        Console.WriteLine("Enter group name:");
                        string groupName = Console.ReadLine();
                        groups.Insert(groupName, new(), new(), new());
                        break;
                    }
                    if (groupId.ToLower().StartsWith("p"))
                    {
                        if (int.TryParse(groupId[1..], out int newPage))
                        {
                            if (newPage > 0 && newPage <= maxPage)
                            {
                                page = newPage;
                                break;
                            }
                        }
                    }
                    if (groupId.ToLower().StartsWith("del"))
                    {
                        Console.WriteLine("Enter group id:");
                        string? groupIdToDelete = Console.ReadLine();
                        if (string.IsNullOrEmpty(groupIdToDelete)) { continue; }
                        if (groupIdToDelete == "0") { break; }
                        if (int.TryParse(groupIdToDelete, out int deleteId))
                        {
                            Group? group = groups.DeleteById(deleteId);
                            if (group == null) { Console.WriteLine("Not found"); }
                        }
                        break;
                    }
                    if (int.TryParse(groupId, out int id))
                    {
                        Group? group = groups.FindById(id);
                        if (group == null) { continue; }
                        GroupMenu(group);
                        break;
                    }
                }
            }
        }
        void GroupMenu(Group group)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Group {group.Name}");
                Console.WriteLine("Students:");
                int studentNumber = 1;
                foreach (int studentId in group.Students)
                {
                    Student? student = students.FindById(studentId);
                    if (student == null) { continue; }

                    int avgMark = 0;
                    for (int i = 0; i < group.Subjects.Count; i++)
                    {
                        if (group.Marks[studentNumber - 1][i].Count > 0)
                        {
                            avgMark += group.Marks[studentNumber - 1][i].Aggregate(0, (acc, x) => acc + x) / group.Marks[studentNumber - 1][i].Count;
                        }
                    }
                    if (group.Subjects.Count > 0)
                    {
                        avgMark /= group.Subjects.Count;
                    }

                    Console.WriteLine($"{studentNumber}. {student.Name} {student.Surname} - {avgMark} points");
                    studentNumber++;
                }
                Console.WriteLine("Subjects:");
                int subjectNumber = 1;
                foreach (string subject in group.Subjects)
                {
                    int avgMark = 0;
                    for (int i = 0; i < group.Students.Count; i++)
                    {
                        if (group.Marks[i][subjectNumber - 1].Count > 0)
                        {
                            avgMark += group.Marks[i][subjectNumber - 1].Aggregate(0, (acc, x) => acc + x) / group.Marks[i][subjectNumber - 1].Count;
                        }
                    }
                    if (group.Students.Count > 0)
                    {
                        avgMark /= group.Students.Count;
                    }

                    Console.WriteLine($"{subjectNumber}. {subject} - {avgMark} points");
                    subjectNumber++;
                }
                Console.WriteLine("\n0. Back\nNumber. View student ; sNumber. View subject\nq. Add student ; s. Add subject\nc. Show successful / unsuccessful\nr. Rename");

                string? groupInput = Console.ReadLine();
                if (string.IsNullOrEmpty(groupInput)) { continue; }
                if (groupInput == "0") { break; }
                if (groupInput == "q")
                {
                    Console.WriteLine("Enter student name:");
                    string? studentName = Console.ReadLine();
                    while (string.IsNullOrEmpty(studentName))
                    {
                        Console.WriteLine("Enter student name:");
                        studentName = Console.ReadLine();
                    }
                    Console.WriteLine("Enter student surname:");
                    string? studentSurname = Console.ReadLine();
                    while (string.IsNullOrEmpty(studentSurname))
                    {
                        Console.WriteLine("Enter student surname:");
                        studentSurname = Console.ReadLine();
                    }
                    Student newStudent = students.Insert(studentName, studentSurname);
                    groups.UpdateById(group.Id, GroupService.AddStudent(group, newStudent.Id));
                    continue;
                }
                if (groupInput == "r")
                {
                    Console.WriteLine("Enter new group name:");
                    string? groupName = Console.ReadLine();
                    while (string.IsNullOrEmpty(groupName))
                    {
                        Console.WriteLine("Enter new group name:");
                        groupName = Console.ReadLine();
                    }
                    group.Name = groupName;
                    groups.UpdateById(group.Id, group);
                    continue;
                }
                if (groupInput == "c")
                {
                    Console.Clear();
                    Console.WriteLine($"{group.Name}");
                    List<Tuple<int, int>> tuples = GroupService.StudentsMarks(group);
                    Console.WriteLine("Successful:");
                    foreach (Tuple<int, int> tuple in tuples.FindAll(el => el.Item2 >= 60))
                    {
                        Student? student = students.FindById(tuple.Item1);
                        if (student == null) { continue; }
                        Console.WriteLine($"{student.Name} {student.Surname} - {tuple.Item2} points");
                    }
                    Console.WriteLine("Unsuccessful:");
                    foreach (Tuple<int, int> tuple in tuples.FindAll(el => el.Item2 < 60))
                    {
                        Student? student = students.FindById(tuple.Item1);
                        if (student == null) { continue; }
                        Console.WriteLine($"{student.Name} {student.Surname} - {tuple.Item2} points");
                    }
                    Console.WriteLine("0. Back");
                    while (true)
                    {
                        string? input = Console.ReadLine();
                        if (string.IsNullOrEmpty(input)) { continue; }
                        if (input == "0") { break; }
                    }
                    continue;
                }
                if (groupInput == "s")
                {
                    Console.WriteLine("Enter subject name:");
                    string? subjectName = Console.ReadLine();
                    while (string.IsNullOrEmpty(subjectName))
                    {
                        Console.WriteLine("Enter subject name:");
                        subjectName = Console.ReadLine();
                    }
                    groups.UpdateById(group.Id, GroupService.AddSubject(group, subjectName));
                    continue;
                }
                if (groupInput.ToLower().StartsWith("s"))
                {
                    if (int.TryParse(groupInput[1..], out int groupSubjectNumber))
                    {
                        if (groupSubjectNumber <= 0 || groupSubjectNumber > group.Subjects.Count) { continue; }
                        string subject = group.Subjects[groupSubjectNumber - 1];

                        while (true)
                        {
                            Console.Clear();
                            Console.WriteLine($"Subject {subject}:");

                            int _subjectStudentNumber = 1;
                            for (int i = 0; i < group.Students.Count; i++)
                            {
                                Student? student = students.FindById(group.Students[i]);
                                if (student == null) { continue; }
                                Console.WriteLine($"{_subjectStudentNumber}. {student.Name} {student.Surname}");
                                int subjectMark = group.Marks[i][groupSubjectNumber - 1].Count > 0 ? group.Marks[i][groupSubjectNumber - 1].Aggregate(0, (acc, x) => acc + x) / group.Marks[i][groupSubjectNumber - 1].Count : 0;
                                Console.WriteLine($" Marks: {String.Join(", ", group.Marks[i][groupSubjectNumber - 1].ToArray())} - {subjectMark} points");
                                _subjectStudentNumber++;
                            }
                            Console.WriteLine("0. Back ; Number. Edit marks ; s. Show successful / unsuccessful ; del. Delete subject");

                            string? subjectInput = Console.ReadLine();
                            if (string.IsNullOrEmpty(subjectInput)) { continue; }
                            if (subjectInput == "0") { break; }
                            if (subjectInput == "del")
                            {
                                groups.UpdateById(group.Id, GroupService.RemoveSubject(group, subject));
                                break;
                            }
                            if (subjectInput == "s")
                            {
                                Console.Clear();
                                Console.WriteLine($"{subject} in {group.Name}");
                                List<Tuple<int, int>> tuples = GroupService.StudentsMarksOnSubject(group, groupSubjectNumber - 1);
                                Console.WriteLine("Successful:");
                                foreach (Tuple<int, int> tuple in tuples.FindAll(el => el.Item2 >= 60))
                                {
                                    Student? student = students.FindById(tuple.Item1);
                                    if (student == null) { continue; }
                                    Console.WriteLine($"{student.Name} {student.Surname} - {tuple.Item2} points");
                                }
                                Console.WriteLine("Unsuccessful:");
                                foreach (Tuple<int, int> tuple in tuples.FindAll(el => el.Item2 < 60))
                                {
                                    Student? student = students.FindById(tuple.Item1);
                                    if (student == null) { continue; }
                                    Console.WriteLine($"{student.Name} {student.Surname} - {tuple.Item2} points");
                                }
                                Console.WriteLine("0. Back");
                                while (true)
                                {
                                    string? input = Console.ReadLine();
                                    if (string.IsNullOrEmpty(input)) { continue; }
                                    if (input == "0") { break; }
                                }
                                continue;
                            }
                            if (int.TryParse(subjectInput, out int subjectStudentNumber))
                            {
                                Console.WriteLine("0. Back ; Enter marks like '60 76 100':");
                                string? marksInput = Console.ReadLine();
                                if (marksInput == "0") { break; }
                                if (string.IsNullOrEmpty(marksInput)) { Console.WriteLine("Try again:"); continue; }
                                List<int> marks = new();
                                foreach (string mark in marksInput.Split(' '))
                                {
                                    if (int.TryParse(mark, out int markInt))
                                    {
                                        if (markInt > 0 && markInt <= 100)
                                        {
                                            marks.Add(markInt);
                                        }
                                    }
                                }
                                groups.UpdateById(group.Id, GroupService.SetMarks(group, group.Students[subjectStudentNumber - 1], subject, marks));
                                break;
                            }
                        }
                    }
                }
                if (int.TryParse(groupInput, out int groupStudentNumber))
                {
                    if (groupStudentNumber <= 0 || groupStudentNumber > group.Students.Count) { continue; }
                    Student? student = students.FindById(group.Students[groupStudentNumber - 1]);
                    if (student == null) { continue; }

                    Console.Clear();
                    Console.WriteLine($"{student.Name} {student.Surname} in {group.Name}");

                    for (int i = 0; i < group.Subjects.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {group.Subjects[i]}");

                        int subjectMark = group.Marks[groupStudentNumber - 1][i].Count > 0 ? group.Marks[groupStudentNumber - 1][i].Aggregate(0, (acc, x) => acc + x) / group.Marks[groupStudentNumber - 1][i].Count : 0;

                        Console.WriteLine($" Marks: {String.Join(", ", group.Marks[groupStudentNumber - 1][i].ToArray())} - {subjectMark} points");
                    }

                    Console.WriteLine("0. Back ; Number. Edit marks ; d. Delete student");

                    while (true)
                    {
                        string? studentInput = Console.ReadLine();
                        if (string.IsNullOrEmpty(studentInput)) { continue; }
                        if (studentInput == "0") { break; }
                        if (studentInput == "d")
                        {
                            groups.UpdateById(group.Id, GroupService.RemoveStudent(group, student.Id));
                            break;
                        }
                        if (int.TryParse(studentInput, out int studentSubjectNumber))
                        {
                            if (studentSubjectNumber <= 0 || studentSubjectNumber > group.Subjects.Count) { continue; }
                            Console.WriteLine("0. Back ; Enter marks like '60 76 100':");
                            string? marksInput = Console.ReadLine();
                            if (marksInput == "0") { break; }
                            if (string.IsNullOrEmpty(marksInput)) { Console.WriteLine("Try again:"); continue; }
                            List<int> marks = new();
                            foreach (string mark in marksInput.Split(' '))
                            {
                                if (int.TryParse(mark, out int markInt))
                                {
                                    if (markInt > 0 && markInt <= 100)
                                    {
                                        marks.Add(markInt);
                                    }
                                }
                            }
                            groups.UpdateById(group.Id, GroupService.SetMarks(group, student.Id, group.Subjects[studentSubjectNumber - 1], marks));
                            break;
                        }
                    }
                }
            }
        }
        void StudentsMenu()
        {
            int page = 1;
            int itemsPerPage = 20;
            while (true)
            {
                int maxPage = students.Students.Count < itemsPerPage ? 1 : (students.Students.Count / itemsPerPage);

                Console.Clear();
                Console.WriteLine("Students:");

                int studentNumber = 1;
                foreach (Student student in students.Students)
                {
                    if (studentNumber >= (page - 1) * itemsPerPage && studentNumber < page * itemsPerPage)
                    {
                        Console.WriteLine($"{student.Id}. {student.Name} {student.Surname}");
                    }
                    studentNumber++;
                }
                Console.WriteLine($"0. Back ; q. Add student ; del. Delete student ; pX. Set page ; s. Search ; s1. Search by avg mark");

                string? studentId = Console.ReadLine();
                if (string.IsNullOrEmpty(studentId)) { continue; }
                if (studentId == "0") { break; }
                if (studentId == "q")
                {
                    Console.WriteLine("Enter student name:");
                    string? studentName = Console.ReadLine();
                    while (string.IsNullOrEmpty(studentName))
                    {
                        Console.WriteLine("Enter student name:");
                        studentName = Console.ReadLine();
                    }
                    Console.WriteLine("Enter student surname:");
                    string? studentSurname = Console.ReadLine();
                    while (string.IsNullOrEmpty(studentSurname))
                    {
                        Console.WriteLine("Enter student surname:");
                        studentSurname = Console.ReadLine();
                    }
                    students.Insert(studentName, studentSurname);
                    break;
                }
                if (studentId.ToLower().StartsWith("p"))
                {
                    if (int.TryParse(studentId[1..], out int newPage))
                    {
                        if (newPage > 0 && newPage <= maxPage)
                        {
                            page = newPage;
                            break;
                        }
                    }
                }
                if (studentId.ToLower().StartsWith("del"))
                {
                    Console.WriteLine("Enter student id:");
                    string? studentIdToDelete = Console.ReadLine();
                    if (string.IsNullOrEmpty(studentIdToDelete)) { continue; }
                    if (studentIdToDelete == "0") { break; }
                    if (int.TryParse(studentIdToDelete, out int deleteId))
                    {
                        Student? student = students.DeleteById(deleteId);
                        if (student == null) { Console.WriteLine("Not found"); }
                    }
                    continue;
                }
                if (studentId == "s")
                {
                    Console.Clear();
                    Console.WriteLine("0. Back ; Enter student name or surname:");
                    string? studentNameOrSurname = Console.ReadLine();
                    if (string.IsNullOrEmpty(studentNameOrSurname)) { break; }
                    if (studentNameOrSurname == "0") { break; }
                    foreach (Student student in students.Search(studentNameOrSurname))
                    {
                        Console.WriteLine($"{student.Id}. {student.Name} {student.Surname}");
                    }
                    Console.WriteLine("0. Back ; Number. Go to student");
                    while (true)
                    {
                        string? studentInput = Console.ReadLine();
                        if (string.IsNullOrEmpty(studentInput)) { continue; }
                        if (studentInput == "0") { break; }
                        if (int.TryParse(studentInput, out int studentGroupId))
                        {
                            Student? student = students.FindById(studentGroupId);
                            if (student == null) { continue; }
                            StudentMenu(student);
                            break;
                        }
                    }
                }
                if (studentId == "s1")
                {
                    Console.Clear();
                    Console.WriteLine("0. Back ; Enter avg points of student:");
                    string? studentNameOrSurname = Console.ReadLine();
                    if (string.IsNullOrEmpty(studentNameOrSurname)) { continue; }
                    if (studentNameOrSurname == "0") { break; }
                    if (int.TryParse(studentNameOrSurname, out int avg))
                    {
                        foreach (Tuple<Group, List<Student>> result in groups.SearchByAvg(avg, students))
                        {
                            Console.WriteLine($"{result.Item1.Id}. {result.Item1.Name}");
                            foreach (Student student in result.Item2)
                            {
                                Console.WriteLine($" {student.Id}. {student.Name} {student.Surname}");
                            }
                        }
                        Console.WriteLine("0. Back ; Number. Go to group");
                        while (true)
                        {
                            string? studentInput = Console.ReadLine();
                            if (string.IsNullOrEmpty(studentInput)) { continue; }
                            if (studentInput == "0") { break; }
                            if (int.TryParse(studentInput, out int studentGroupId))
                            {
                                Group? group = groups.FindById(studentGroupId);
                                if (group == null) { continue; }
                                GroupMenu(group);
                                break;
                            }
                        }
                    }

                }
                if (int.TryParse(studentId, out int id))
                {
                    Student? student = students.FindById(id);
                    if (student == null) { continue; }
                    StudentMenu(student);
                    break;
                }
            }
        }
        void StudentMenu(Student student)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Student {student.Name} {student.Surname}");

                Console.WriteLine("Groups:");
                foreach (Group group in groups.SearchByStudent(student.Id))
                {
                    Console.WriteLine($"{group.Id}. {group.Name}");
                }
                Console.WriteLine("0. Back ; Number. Go to group ; q. Change Name ; w. Change Surname ; d. Delete");

                string? studentInput = Console.ReadLine();
                if (string.IsNullOrEmpty(studentInput)) { continue; }
                if (studentInput == "0") { break; }
                if (studentInput == "q")
                {
                    Console.WriteLine("0. Back ; Enter new student name:");
                    string? studentName = Console.ReadLine();
                    while (string.IsNullOrEmpty(studentName))
                    {
                        Console.WriteLine("Enter new student name:");
                        studentName = Console.ReadLine();
                    }
                    if (studentName == "0") { break; }
                    student.Name = studentName;
                    students.UpdateById(student.Id, student);
                    continue;
                }
                if (studentInput == "w")
                {
                    Console.WriteLine("0. Back ; Enter new student surname:");
                    string? studentSurname = Console.ReadLine();
                    while (string.IsNullOrEmpty(studentSurname))
                    {
                        Console.WriteLine("Enter new student surname:");
                        studentSurname = Console.ReadLine();
                    }
                    if (studentSurname == "0") { break; }
                    student.Surname = studentSurname;
                    students.UpdateById(student.Id, student);
                    continue;
                }
                if (studentInput == "d")
                {
                    groups.DeleteStudent(student.Id);
                    students.DeleteById(student.Id);
                    break;
                }
                if (int.TryParse(studentInput, out int studentGroupId))
                {
                    Group? group = groups.FindById(studentGroupId);
                    if (group == null) { continue; }
                    GroupMenu(group);
                    continue;
                }
            }
        }
    }
}
