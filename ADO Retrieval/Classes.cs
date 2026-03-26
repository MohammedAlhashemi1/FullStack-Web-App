using Microsoft.Data.SqlClient;

namespace ICA08.Net.My_Class
{
    public static class Classes
    {
        static string connStr = "YOUR_CONNECTION_STRING_HERE";


        // ==========================================================
        // GET STUDENTS (FirstName starts with E or F)
        // ==========================================================
        public static List<object> GetStudents()
        {
            List<object> list = new();
            using SqlConnection conn = new(connStr);
            conn.Open();


            string sql = @"
                SELECT student_id, first_name, last_name, school_id
                FROM Students
                WHERE first_name LIKE 'E%' OR first_name LIKE 'F%'
                ORDER BY first_name, last_name";


            using SqlCommand cmd = new(sql, conn);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new
                {
                    studentId = reader[0].ToString(),
                    firstName = reader[1].ToString(),
                    lastName = reader[2].ToString(),
                    schoolId = reader[3].ToString()
                });
            }
            return list;
        }


        // ==========================================================
        // GET CLASSES FOR ONE STUDENT
        // ==========================================================
        public static List<object> GetClasses(int studentId)
        {
            List<object> list = new();
            using SqlConnection conn = new(connStr);
            conn.Open();


            string sql = @"
                SELECT c.class_id, c.class_desc, c.days, c.start_date,
                       i.instructor_id, i.first_name, i.last_name
                FROM Classes c
                JOIN class_to_student sc ON c.class_id = sc.class_id
                JOIN Instructors i ON c.instructor_id = i.instructor_id
                WHERE sc.student_id = @sid";


            using SqlCommand cmd = new(sql, conn);
            cmd.Parameters.AddWithValue("@sid", studentId);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new
                {
                    classId = reader[0],
                    classDesc = reader[1].ToString(),
                    days = reader[2] == null? (int)reader[2] : 0,
                    startDate = reader[3].ToString(),
                    instructorId = reader[4].ToString(),
                    instructorFirstName = reader[5].ToString(),
                    instructorLastName = reader[6].ToString()
                });
            }
            return list;
        }
    }
}


