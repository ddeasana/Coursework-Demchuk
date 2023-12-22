using BLL;
using DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLL.Tests
{
    [TestClass()]
    public class StudentServiceTests
    {
        GroupService groups = new();
        StudentService students = new();
        [TestMethod()]
        public void InsertTest()
        {
            // Arrange
            string name = "Test Student";
            string surname = "Test Student";
            // Act
            Student student = students.Insert(name, surname);
            // Assert
            Assert.AreEqual(student.Name, name);
            Assert.AreEqual(student.Surname, surname);
            // Cleanup
            students.DeleteById(student.Id);
        }

        [TestMethod()]
        public void FindByIdTest()
        {
            // Arrange
            string name = "Test Student";
            string surname = "Test Student";
            Student student = students.Insert(name, surname);
            // Act
            Student? foundStudent = students.FindById(student.Id);
            // Assert
            Assert.AreEqual(foundStudent, student);
            // Cleanup
            students.DeleteById(student.Id);
        }

        [TestMethod()]
        public void UpdateByIdTest()
        {
            // Arrange
            string name = "Test Student";
            string surname = "Test Student";
            Student student = students.Insert(name, surname);
            string newName = "New Test Student";
            string newSurname = "New Test Student";
            // Act
            student.Name = newName;
            student.Surname = newSurname;
            Student? updatedStudent = students.UpdateById(student.Id, student);
            // Assert
            Assert.AreEqual(updatedStudent?.Name, newName);
            Assert.AreEqual(updatedStudent?.Surname, newSurname);
            // Cleanup
            students.DeleteById(student.Id);
        }

        [TestMethod()]
        public void DeleteByIdTest()
        {
            // Arrange
            string name = "Test Student";
            string surname = "Test Student";
            Student student = students.Insert(name, surname);
            // Act
            Student? deletedStudent = students.DeleteById(student.Id);
            // Assert
            Assert.AreEqual(deletedStudent, student);
        }

        [TestMethod()]
        public void SearchTest()
        {
            // Arrange
            string name = "Test Student";
            string surname = "Test Student";
            Student student = students.Insert(name, surname);
            // Act
            List<Student> foundStudents = students.Search("Test");
            // Assert
            Assert.IsTrue(foundStudents.Contains(student));
            // Cleanup
            students.DeleteById(student.Id);
        }
    }
}