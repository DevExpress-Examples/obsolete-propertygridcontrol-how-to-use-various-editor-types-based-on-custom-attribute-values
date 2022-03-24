using System;
using System.ComponentModel;

namespace DXSample {
    public class ViewModel {
        public Department Department { get; set; }
        public ViewModel() {
            Department = new Department { Name = "Department" };
            Department.Employees.Add(new Employee { ButtonEdit = "Jett Mitchell", });
            Department.Employees.Add(new Employee { ButtonEdit = "Garrick Stiedemann DVM" });
            Department.Employees.Add(new Employee { ButtonEdit = "Hettie Runte" });
        }
    }
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Department {
        public Department() {
            Employees = new BindingList<Employee>();
        }
        public string Name { get; set; }
        public BindingList<Employee> Employees { get; set; }
    }
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Employee {
        [CustomEditor("ButtonEdit")]
        public string ButtonEdit { get; set; }
    }
    public class CustomEditorAttribute : Attribute {
        public string Name { get; private set; }
        public CustomEditorAttribute(string _name) {
            Name = _name;
        }
    }
}