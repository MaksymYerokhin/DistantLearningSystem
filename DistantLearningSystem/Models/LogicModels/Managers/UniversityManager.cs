using DistantLearningSystem.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DistantLearningSystem.Models.LogicModels.Managers
{
    public class UniversityManager : Manager
    {
        public Department AddDepartment(Department department)
        {
            var dept = entities.Departments.Add(department);
            SaveChanges();
            return dept;
        }

        public IEnumerable<StudentGroup> GetGroups()
        {
            return entities.StudentGroups;
        }

        public Faculty AddFaculty(Faculty faculty)
        {
            var _faculty = entities.Faculties.Add(faculty);
            SaveChanges();
            return _faculty;
        }

        public Faculty GetFaculty(int facultyId)
        {
            return entities.Faculties.FirstOrDefault(x => x.Id == facultyId);
        }

        public StudentGroup GetGroup(int Id)
        {
            return entities.StudentGroups.ToList().FirstOrDefault(x => x.Id == Id);
        }

        public ProcessResult SetGroupLecturer(int lecturerId, int groupId)
        {
            var group = GetGroup(groupId);
            if (group.LecturerId == lecturerId)
                return ProcessResults.YouAreAlreadyLecturer;

            group.LecturerId = lecturerId;
            SaveChanges();
            return ProcessResults.GroupWasSuccessfullyAdded;
        }

        public bool RemoveFaculty(int facultyid)
        {
            var facultyToRemove = GetFaculty(facultyid);
            if (facultyToRemove == null)
                return false;
            entities.Faculties.Remove(facultyToRemove);
            return true;
        }

        public Department GetDepartment(int departmentId)
        {
            return entities.Departments.FirstOrDefault(x => x.Id == departmentId);
        }

        public bool RemoveDepartment(int departmentId)
        {
            var deptToDelete = GetDepartment(departmentId);
            if (deptToDelete == null)
                return false;
            entities.Departments.Remove(deptToDelete);
            SaveChanges();
            return true;
        }
    }
}