using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DXSample {
    public class ViewModel {
        public Department Department { get; set; }
        public ViewModel() {
            Department = new Department { Name = "Department" };
            Department.Employees.Add(new Employee { Name = "Jett Mitchell" });
            Department.Employees.Add(new Employee { Name = "Garrick Stiedemann DVM" });
            Department.Employees.Add(new Employee { Name = "Hettie Runte" });
        }
    }
    public class Department {
        public Department() {
            Employees = new BindingList<Employee>();
        }
        public string Name { get; set; }
        public BindingList<Employee> Employees { get; set; }
    }
    public class Employee {
        [CustomEditorAttribute("ButtonEdit")]
        public string Name { get; set; }
    }
    public class CustomEditorAttribute : Attribute {
        public string Name { get; private set; }
        public CustomEditorAttribute(string _name) {
            Name = _name;
        }
    }
}