using BLL;
using DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLL.Tests
{
    [TestClass()]
    public class GroupServiceTests
    {
        GroupService groups = new();
        StudentService students = new();

        [TestMethod()]
        public void InsertTest()
        {
            // Arrange
            string name = "Test Group";
            // Act
            Group group = groups.Insert(name, new(), new(), new());
            // Assert
            Assert.AreEqual(group.Name, name);
            // Cleanup
            groups.DeleteById(group.Id);
        }

        [TestMethod()]
        public void FindByIdTest()
        {
            // Arrange
            string name = "Test Group";
            Group group = groups.Insert(name, new(), new(), new());
            // Act
            Group? foundGroup = groups.FindById(group.Id);
            // Assert
            Assert.AreEqual(foundGroup, group);
            // Cleanup
            groups.DeleteById(group.Id);
        }

        [TestMethod()]
        public void UpdateByIdTest()
        {
            // Arrange
            string name = "Test Group";
            Group group = groups.Insert(name, new(), new(), new());
            string newName = "New Test Group";
            // Act
            group.Name = newName;
            Group? updatedGroup = groups.UpdateById(group.Id, group);
            // Assert
            Assert.AreEqual(updatedGroup?.Name, newName);
            // Cleanup
            groups.DeleteById(group.Id);
        }

        [TestMethod()]
        public void AddStudentTest()
        {
            // Arrange
            string name = "Test Group";
            Group group = groups.Insert(name, new(), new(), new());
            string studentName = "Test Student";
            string studentSurname = "Test Student";
            Student student = students.Insert(studentName, studentSurname);
            // Act
            Group updatedGroup = GroupService.AddStudent(group, student.Id);
            // Assert
            Assert.AreEqual(updatedGroup.Students[^1], student.Id);
            // Cleanup
            groups.DeleteById(group.Id);
            students.DeleteById(student.Id);
        }

        [TestMethod()]
        public void RemoveStudentTest()
        {
            // Arrange
            string name = "Test Group";
            Group group = groups.Insert(name, new(), new(), new());
            string studentName = "Test Student";
            string studentSurname = "Test Student";
            Student student = students.Insert(studentName, studentSurname);
            Group updatedGroup = GroupService.AddStudent(group, student.Id);
            // Act
            Group removedGroup = GroupService.RemoveStudent(updatedGroup, student.Id);
            // Assert
            Assert.AreEqual(removedGroup.Students.Count, 0);
            // Cleanup
            groups.DeleteById(group.Id);
            students.DeleteById(student.Id);
        }

        [TestMethod()]
        public void AddSubjectTest()
        {
            // Arrange
            string name = "Test Group";
            Group group = groups.Insert(name, new(), new(), new());
            string subject = "Test Subject";
            // Act
            Group updatedGroup = GroupService.AddSubject(group, subject);
            // Assert
            Assert.AreEqual(updatedGroup.Subjects[^1], subject);
            // Cleanup
            groups.DeleteById(group.Id);
        }

        [TestMethod()]
        public void RemoveSubjectTest()
        {
            // Arrange
            string name = "Test Group";
            Group group = groups.Insert(name, new(), new(), new());
            string subject = "Test Subject";
            Group updatedGroup = GroupService.AddSubject(group, subject);
            // Act
            Group removedGroup = GroupService.RemoveSubject(updatedGroup, subject);
            // Assert
            Assert.AreEqual(removedGroup.Subjects.Count, 0);
            // Cleanup
            groups.DeleteById(group.Id);
        }

        [TestMethod()]
        public void AddMarkTest()
        {
            // Arrange
            string name = "Test Group";
            Group group = groups.Insert(name, new() { 1 }, new() { }, new() { new() { new() } });
            string subject = "Test Subject";
            Group updatedGroup = GroupService.AddSubject(group, subject);
            int studentId = 1;
            // Act
            Group updatedGroup2 = GroupService.AddMark(updatedGroup, studentId, subject, 5);
            // Assert
            Assert.AreEqual(updatedGroup2.Marks[0][0][0], 5);
            // Cleanup
            groups.DeleteById(group.Id);
        }

        [TestMethod()]
        public void SetMarksTest()
        {
            // Arrange
            string name = "Test Group";
            Group group = groups.Insert(name, new() { 1 }, new() { "Test Subject" }, new() { new() { new() } });
            int studentId = 1;
            // Act
            Group updatedGroup2 = GroupService.SetMarks(group, studentId, "Test Subject", new() { 5, 4, 3, 2, 1 });
            // Assert
            Assert.AreEqual(updatedGroup2.Marks[0][0][0], 5);
            // Cleanup
            groups.DeleteById(group.Id);
        }

        [TestMethod()]
        public void RemoveMarkTest()
        {
            // Arrange
            string name = "Test Group";
            Group group = groups.Insert(name, new() { 1 }, new() { "Test Subject" }, new() { new() { new() { 5 } } });
            int studentId = 1;
            // Act
            Group updatedGroup2 = GroupService.RemoveMark(group, studentId, "Test Subject", 0);
            // Assert
            Assert.AreEqual(updatedGroup2.Marks[0][0].Count, 0);
            // Cleanup
            groups.DeleteById(group.Id);
        }

        [TestMethod()]
        public void DeleteStudentTest()
        {
            // Arrange
            string name = "Test Group";
            Student student = students.Insert("Test Student", "Test Student");
            Group group = groups.Insert(name, new() { student.Id }, new() { "Test Subject" }, new() { new() { new() { 5 } } });
            // Act
            groups.DeleteStudent(student.Id);
            // Assert
            Assert.AreEqual(group.Students.Count, 0);
            // Cleanup
            groups.DeleteById(group.Id);
            students.DeleteById(student.Id);
        }

        [TestMethod()]
        public void SearchByStudentTest()
        {
            // Arrange
            string name = "Test Group";
            Student student = students.Insert("Test Student", "Test Student");
            Group group = groups.Insert(name, new() { student.Id }, new() { "Test Subject" }, new() { new() { new() { 5 } } });
            // Act
            List<Group> foundGroups = groups.SearchByStudent(student.Id);
            // Assert
            Assert.AreEqual(foundGroups[0], group);
            // Cleanup
            groups.DeleteById(group.Id);
            students.DeleteById(student.Id);
        }

        [TestMethod()]
        public void SearchByAvgTest()
        {
            // Arrange
            string name = "Test Group";
            Student student = students.Insert("Test Student", "Test Student");
            Group group = groups.Insert(name, new() { student.Id }, new() { "Test Subject" }, new() { new() { new() { 5 } } });
            // Act
            List<Tuple<Group, List<Student>>> foundGroups = groups.SearchByAvg(5, students);
            // Assert
            Assert.AreEqual(foundGroups[0].Item1, group);
            // Cleanup
            groups.DeleteById(group.Id);
            students.DeleteById(student.Id);
        }

        [TestMethod()]
        public void StudentsMarksOnSubjectTest()
        {
            // Arrange
            string name = "Test Group";
            Student student = students.Insert("Test Student", "Test Student");
            Group group = groups.Insert(name, new() { student.Id }, new() { "Test Subject" }, new() { new() { new() { 5 } } });
            // Act
            List<Tuple<int, int>> foundGroups = GroupService.StudentsMarksOnSubject(group, 0);
            // Assert
            Assert.AreEqual(foundGroups[0].Item1, student.Id);
            // Cleanup
            groups.DeleteById(group.Id);
            students.DeleteById(student.Id);
        }

        [TestMethod()]
        public void StudentsMarksTest()
        {
            // Arrange
            string name = "Test Group";
            Student student = students.Insert("Test Student", "Test Student");
            Group group = groups.Insert(name, new() { student.Id }, new() { "Test Subject" }, new() { new() { new() { 5 } } });
            // Act
            List<Tuple<int, int>> foundGroups = GroupService.StudentsMarks(group);
            // Assert
            Assert.AreEqual(foundGroups[0].Item1, student.Id);
            // Cleanup
            groups.DeleteById(group.Id);
            students.DeleteById(student.Id);
        }

        [TestMethod()]
        public void DeleteByIdTest()
        {
            // Arrange
            string name = "Test Group";
            Group group = groups.Insert(name, new(), new(), new());
            // Act
            Group? deletedGroup = groups.DeleteById(group.Id);
            // Assert
            Assert.AreEqual(deletedGroup, group);
        }
    }
}