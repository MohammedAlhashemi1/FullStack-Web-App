using Microsoft.Data.SqlClient;

namespace ICA09.Net.Classes
{
    public static class Stored
    {
        static string connStr = "YOUR_CONNECTION_STRING_HERE";

        // --------------------------- GET STUDENTS ---------------------------
        public static List<object> GetStudents()
        {
            List<object> output = new();

            using SqlConnection conn = new(connStr);
            conn.Open();

            using SqlCommand cmd = new("GetStudents", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            using SqlDataReader r = cmd.ExecuteReader();
            while (r.Read())
            {
                output.Add(new
                {
                    studentId = r[0].ToString(),
                    firstName = r[1].ToString(),
                    lastName = r[2].ToString(),
                    schoolId = r[3].ToString()
                });
            }

            return output;
        }

        // --------------------------- GET CLASSES FOR ONE STUDENT ---------------------------
        public static List<object> GetClasses(int studentId)
        {
            List<object> output = new();

            using SqlConnection conn = new(connStr);
            conn.Open();

            using SqlCommand cmd = new("GetStudentClasses", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            // stored procedure expects @studentId
            cmd.Parameters.AddWithValue("@studentId", studentId);

            using SqlDataReader r = cmd.ExecuteReader();
            while (r.Read())
            {
                output.Add(new
                {
                    classId = r[0].ToString(),
                    classDesc = r[1].ToString(),
                    days = r[2].ToString(),
                    startDate = r[3].ToString(),
                    instructorId = r[4].ToString(),
                    instructorFirstName = r[5].ToString(),
                    instructorLastName = r[6].ToString()
                });
            }

            return output;
        }

        // --------------------------- GET ALL CLASSES ---------------------------
        public static List<object> GetAllClasses()
        {
            List<object> output = new();

            using SqlConnection conn = new(connStr);
            conn.Open();

            using SqlCommand cmd = new("GetAllClasses", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            using SqlDataReader r = cmd.ExecuteReader();
            while (r.Read())
            {
                output.Add(new
                {
                    classId = r[0].ToString(),
                    classDesc = r[1].ToString()
                });
            }

            return output;
        }

        // --------------------------- INSERT STUDENT ---------------------------
        public static string InsertStudent(string first, string last, int school, List<int> classIds)
        {
            string msg = "";
            int newId = -1;

            using SqlConnection conn = new(connStr);
            conn.Open();

            // INSERT STUDENT
            using (SqlCommand cmd = new("InsertStudent", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@first", first);
                cmd.Parameters.AddWithValue("@last", last);
                cmd.Parameters.AddWithValue("@school", school);

                SqlParameter outId = new("@newId", System.Data.SqlDbType.Int)
                {
                    Direction = System.Data.ParameterDirection.Output
                };
                cmd.Parameters.Add(outId);

                SqlParameter status = new("@status", System.Data.SqlDbType.NVarChar, 100)
                {
                    Direction = System.Data.ParameterDirection.Output
                };
                cmd.Parameters.Add(status);

                cmd.ExecuteNonQuery();

                msg = status.Value.ToString();
                newId = (int)outId.Value;
            }

            // INSERT CLASS LINKS
            foreach (int cid in classIds)
            {
                using SqlCommand cmd2 = new("InsertStudentClass", conn);
                cmd2.CommandType = System.Data.CommandType.StoredProcedure;

                cmd2.Parameters.AddWithValue("@studentId", newId);
                cmd2.Parameters.AddWithValue("@classId", cid);

                SqlParameter status = new("@status", System.Data.SqlDbType.NVarChar, 100)
                {
                    Direction = System.Data.ParameterDirection.Output
                };
                cmd2.Parameters.Add(status);

                cmd2.ExecuteNonQuery();
            }

            return msg;
        }

        // --------------------------- UPDATE STUDENT ---------------------------
        public static string UpdateStudent(int id, string first, string last, int school)
        {
            string msg = "";

            using SqlConnection conn = new(connStr);
            conn.Open();

            using SqlCommand cmd = new("UpdateStudent", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@first", first);
            cmd.Parameters.AddWithValue("@last", last);
            cmd.Parameters.AddWithValue("@school", school);

            SqlParameter status = new("@status", System.Data.SqlDbType.NVarChar, 100)
            {
                Direction = System.Data.ParameterDirection.Output
            };
            cmd.Parameters.Add(status);

            cmd.ExecuteNonQuery();

            msg = status.Value.ToString();
            return msg;
        }

        // --------------------------- DELETE STUDENT ---------------------------
        public static string DeleteStudent(int id)
        {
            string msg = "";

            using SqlConnection conn = new(connStr);
            conn.Open();

            using SqlCommand cmd = new("DeleteStudent", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id", id);

            SqlParameter status = new("@status", System.Data.SqlDbType.NVarChar, 100)
            {
                Direction = System.Data.ParameterDirection.Output
            };
            cmd.Parameters.Add(status);

            cmd.ExecuteNonQuery();

            msg = status.Value.ToString();
            return msg;
        }
    }
}