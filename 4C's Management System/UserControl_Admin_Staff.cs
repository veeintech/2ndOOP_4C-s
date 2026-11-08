using (SqlConnection conn = new SqlConnection(connStr))
{
    conn.Open();

    SqlDataAdapter pendingAdapter = new SqlDataAdapter("SELECT * FROM Staff WHERE EmploymentStatus = 'Pending'", conn);
    DataTable pendingTable = new DataTable();
    pendingAdapter.Fill(pendingTable);
    Request_Dgv.DataSource = pendingTable;

    SqlDataAdapter staffAdapter = new SqlDataAdapter("SELECT * FROM Staff WHERE EmploymentStatus = 'Active'", conn);
    DataTable staffTable = new DataTable();
    staffAdapter.Fill(staffTable);
    Staff_Directory_Dgv.DataSource = staffTable;
}