using Microsoft.Data.SqlClient;

namespace EmployeeDeatialsAdo.Models
{
    public class SqlEmployee : IEmployee
    {
        private readonly IConfiguration _configure;



        public SqlEmployee(IConfiguration configuration)
        {
            _configure = configuration;
        }

        private string connectionString = "Connection Strings";
        public Employee InsertEmployee(Employee employee)
        {
            using (SqlConnection connection = new SqlConnection(_configure.GetSection(connectionString).Value))
            {
                using (SqlCommand command = new SqlCommand("", connection))
                {
                    string myQuery = "INSERT INTO employees (fName, lName, age, email, salary, phone, country, state, gender) OUTPUT INSERTED.id VALUES " +
                        "(@fName, @lName, @age, @email, @salary, @phone, @country, @state, @gender)";

                    command.CommandText = myQuery;
                    command.CommandType = System.Data.CommandType.Text;

                    connection.Open();

                    command.Parameters.AddWithValue("@fName", employee.fName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@lName", employee.lName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@age", employee.age ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@email", employee.email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@salary", employee.salary ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@phone", employee.phone ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@country", employee.country ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@state", employee.state ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@gender", employee.gender ?? (object)DBNull.Value);

                    employee.id = (int)command.ExecuteScalar();

                    foreach (var detail in employee.EmployeeDetails)
                    {
                        using (SqlCommand detailCommand = new SqlCommand("", connection))
                        {
                            string mydetailQuey = "INSERT INTO employeeDetails (Employeeid, city, experience, jobTitle) VALUES " +
                                "(@employeeId,@city,@experience,@jobTitle)";

                            command.CommandText = mydetailQuey;
                            command.CommandType = System.Data.CommandType.Text;

                            command.Parameters.AddWithValue("@employeeId", employee.id);
                            command.Parameters.AddWithValue("@city", detail.city ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@experience", detail.experience ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@jobTitle", detail.jobTitle ?? (object)DBNull.Value);

                            command.ExecuteNonQuery();
                        }
                    }

                }
                connection.Close();
            }
            return employee;
        }


        public Employee GetEmployee(int id)
        {

            Employee employee = null; // يبدأ بقيمة null
            using (SqlConnection connection = new SqlConnection(_configure.GetSection(connectionString).Value))
            {
                connection.Open(); // افتح الاتصال في بداية الدالة

                using (SqlCommand command = new SqlCommand("", connection))
                {
                    // استعلام لجلب بيانات الموظف
                    string myQuery = "SELECT * FROM employees WHERE id = @Id"; // استخدم معلمة لتجنب SQL Injection
                    command.CommandText = myQuery;
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read()) // تحقق إذا كان هناك بيانات
                        {
                            employee = new Employee
                            {
                                id = Convert.ToInt32(reader["id"] == DBNull.Value ? 0 : reader["id"]),
                                fName = Convert.ToString(reader["fName"] == DBNull.Value ? "" : reader["fName"]),
                                lName = Convert.ToString(reader["lName"] == DBNull.Value ? "" : reader["lName"]),
                                age = Convert.ToInt32(reader["age"] == DBNull.Value ? 0 : reader["age"]),
                                email = Convert.ToString(reader["email"] == DBNull.Value ? "" : reader["email"]),
                                salary = Convert.ToDecimal(reader["salary"] == DBNull.Value ? 0 : reader["salary"]),
                                phone = Convert.ToString(reader["phone"] == DBNull.Value ? "" : reader["phone"]),
                                country = Convert.ToString(reader["country"] == DBNull.Value ? "" : reader["country"]),
                                state = Convert.ToString(reader["state"] == DBNull.Value ? "" : reader["state"]),
                                gender = Convert.ToString(reader["gender"] == DBNull.Value ? "" : reader["gender"]),
                                EmployeeDetails = new List<EmployeeDetails>() // تهيئة قائمة التفاصيل
                            };
                        }
                    }
                }

                if (employee != null) // فقط إذا تم العثور على موظف
                {
                    using (SqlCommand detailCommand = new SqlCommand("SELECT * FROM employeeDetails WHERE Employeeid = @EmployeeId", connection))
                    {
                        detailCommand.Parameters.AddWithValue("@EmployeeId", employee.id); // استخدم معلمة

                        using (SqlDataReader detailsReader = detailCommand.ExecuteReader())
                        {
                            while (detailsReader.Read())
                            {
                                EmployeeDetails detail = new EmployeeDetails
                                {
                                    id = Convert.ToInt32(detailsReader["id"] == DBNull.Value ? 0 : detailsReader["id"]),
                                    city = Convert.ToString(detailsReader["city"] == DBNull.Value ? "" : detailsReader["city"]),
                                    jobTitle = Convert.ToString(detailsReader["jobTitle"] == DBNull.Value ? "" : detailsReader["jobTitle"]),
                                    experience = detailsReader["experience"] != DBNull.Value ? (int?)Convert.ToInt32(detailsReader["experience"]) : null
                                };

                                employee.EmployeeDetails.Add(detail); // إضافة التفاصيل إلى قائمة employee.EmployeeDetails
                            }
                        }
                    }
                }
                connection.Close();
            }

            return employee; // ارجع كائن الموظف أو null إذا لم يتم العثور عليه
        }


        public string DeleteEmployee(int id)
        {
            if (id > 0)
            {
                using (SqlConnection connection = new SqlConnection(_configure.GetSection(connectionString).Value))
                {
                    using (SqlCommand command = new SqlCommand("", connection))
                    {
                        string myQuery = "DELETE FROM employees WHERE id = " + id.ToString();

                        command.CommandText = myQuery;
                        command.CommandType = System.Data.CommandType.Text;

                        connection.Open();

                        command.ExecuteNonQuery();

                        using (SqlCommand detailCommand = new SqlCommand("", connection))
                        {
                            string detailQuery = "DELETE FROM employeeDetails WHERE Employeeid = " + id.ToString();

                            detailCommand.CommandText = detailQuery;
                            detailCommand.CommandType = System.Data.CommandType.Text;

                            detailCommand.ExecuteNonQuery();
                        }
                    }
                }
                return "Delete Successfully";
            }
            else
            {
                return "Invalid Id";
            }
        }

        public string UpdateEmployee(Employee employee)
        {
            using (SqlConnection connection = new SqlConnection(_configure.GetSection(connectionString).Value))
            {
                using (SqlCommand command = new SqlCommand("", connection))
                {
                    string myQuery = "UPDATE employees SET fName = @fName, lName = @lName, age = @age, email = @email, salary = @salary, phone = @phone, country = @country, state = @state, gender = @gender WHERE id = @id";

                    command.CommandText = myQuery;
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("id", employee.id);
                    command.Parameters.AddWithValue("@fName", employee.fName);
                    command.Parameters.AddWithValue("@lName", employee.lName);
                    command.Parameters.AddWithValue("@age", employee.age);
                    command.Parameters.AddWithValue("@email", employee.email);
                    command.Parameters.AddWithValue("@salary", employee.salary);
                    command.Parameters.AddWithValue("@phone", employee.phone);
                    command.Parameters.AddWithValue("@country", employee.country);
                    command.Parameters.AddWithValue("@state", employee.state);
                    command.Parameters.AddWithValue("@gender", employee.gender);

                    connection.Open();

                    command.ExecuteNonQuery();

                    foreach (var detail in employee.EmployeeDetails)
                    {
                        using (SqlCommand detailCommand = new SqlCommand("", connection))
                        {
                            string detailQuery = "UPDATE employeeDetails SET city = @city, jobTitle = @jobTitle, experience = @experience WHERE Employeeid = @EmployeeId";

                            detailCommand.CommandText = detailQuery;
                            detailCommand.CommandType = System.Data.CommandType.Text;
                            detailCommand.Parameters.AddWithValue("@Employeeid", employee.id);
                            detailCommand.Parameters.AddWithValue("@city", detail.city);
                            detailCommand.Parameters.AddWithValue("@jobTitle", detail.jobTitle);
                            detailCommand.Parameters.AddWithValue("@experience", detail.experience);

                            detailCommand.ExecuteNonQuery();
                        }
                    }
                }
                connection.Close();
            }
            return "Update Successfully";
        }
    }
}

