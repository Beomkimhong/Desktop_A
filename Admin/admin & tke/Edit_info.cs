﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace admin___tke
{
    public partial class Edit_info : Form
    {
        public Edit_info()
        {
            InitializeComponent();
            InitializeDataGridView();
        }

        private void InitializeDataGridView()
        {
            dgv_Player.Columns.Add("UserID", "UserID");
            dgv_Player.Columns.Add("UserName", "UserName");
            dgv_Player.Columns.Add("Email", "Email");
            dgv_Player.Columns.Add("NGAYTHAMGIA", "NGAYTHAMGIA");
            dgv_Player.Columns.Add("TEN", "TEN");
            dgv_Player.Columns.Add("HO", "HO");
            dgv_Player.Columns.Add("SDT", "SDT");
            dgv_Player.Columns.Add("QUOCGIA", "QUOCGIA");
            dgv_Player.Columns.Add("THANHPHO", "THANHPHO");
            dgv_Player.Columns.Add("Address", "Address");

            // Add command button to DataGridView
            DataGridViewButtonColumn deleteButton = new DataGridViewButtonColumn();
            deleteButton.Name = "DeleteButton";
            deleteButton.HeaderText = "Xóa";
            deleteButton.Text = "Xóa";
            deleteButton.UseColumnTextForButtonValue = true;
            dgv_Player.Columns.Add(deleteButton);
        }

        private void btn_Cancle_Click(object sender, EventArgs e)
        {
            using (SqlConnection sqlConnection = admin___tke.Kết_nối.getConnection())
            {
                sqlConnection.Open();

                string query = "SELECT UserID, UserName, Email, NGAYTHAMGIA, TEN, HO, SDT, QUOCGIA, THANHPHO, Address FROM Users";

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                // Clear the DataGridView before adding new data
                dgv_Player.Rows.Clear();

                while (sqlDataReader.Read())
                {
                    // Add a new row to the DataGridView
                    int rowIndex = dgv_Player.Rows.Add();

                    // Set the values of the cells in the new row
                    dgv_Player.Rows[rowIndex].Cells[0].Value = sqlDataReader["UserID"].ToString();
                    dgv_Player.Rows[rowIndex].Cells[1].Value = sqlDataReader["UserName"].ToString();
                    dgv_Player.Rows[rowIndex].Cells[2].Value = sqlDataReader["Email"].ToString();
                    dgv_Player.Rows[rowIndex].Cells[3].Value = sqlDataReader["NGAYTHAMGIA"] == DBNull.Value ? null : sqlDataReader["NGAYTHAMGIA"].ToString();
                    dgv_Player.Rows[rowIndex].Cells[4].Value = sqlDataReader["TEN"].ToString();
                    dgv_Player.Rows[rowIndex].Cells[5].Value = sqlDataReader["HO"].ToString();
                    dgv_Player.Rows[rowIndex].Cells[6].Value = sqlDataReader["SDT"].ToString();
                    dgv_Player.Rows[rowIndex].Cells[7].Value = sqlDataReader["QUOCGIA"].ToString();
                    dgv_Player.Rows[rowIndex].Cells[8].Value = sqlDataReader["THANHPHO"].ToString();
                    dgv_Player.Rows[rowIndex].Cells[9].Value = sqlDataReader["Address"].ToString();
                }

                sqlDataReader.Close();

                // Refresh the DataGridView to display the new data
                dgv_Player.Refresh();

                sqlConnection.Close();
            }
        }

        private void dgv_Player_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgv_Player.Columns["DeleteButton"].Index && e.RowIndex >= 0)
            {
                // Get the UserID of the selected row
                int userId = Convert.ToInt32(dgv_Player.Rows[e.RowIndex].Cells["UserID"].Value);

                // Show confirmation dialog
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa người dùng này?", "Xác nhận", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    // Delete the user from the database
                    using (SqlConnection sqlConnection = admin___tke.Kết_nối.getConnection())
                    {
                        sqlConnection.Open();

                        string query = "DELETE FROM Users WHERE UserID = @UserId";
                        SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                        sqlCommand.Parameters.AddWithValue("@UserId", userId);
                        sqlCommand.ExecuteNonQuery();

                        sqlConnection.Close();
                    }

                    // Remove the row from the DataGridView
                    dgv_Player.Rows.RemoveAt(e.RowIndex);

                    // Show success message
                    MessageBox.Show("Xóa người dùng thành công.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        DataSet GetUser() { DataSet ds = new DataSet(); return ds; }

        private void btnInvite_Click(object sender, EventArgs e)
        {
            // Hiển thị hộp thoại thông báo khi button được nhấn
            DialogResult result = MessageBox.Show("Would you like to invite this user to become an administrator?", "Confirmation", MessageBoxButtons.OKCancel);

            // Xử lý kết quả từ hộp thoại thông báo
            if (result == DialogResult.OK)
            {
                // Người dùng chọn OK
                // Thực hiện hành động tương ứng ở đây
                MessageBox.Show("You have invited the user to become an administrator.");
            }
            else if (result == DialogResult.Cancel)
            {
                // Người dùng chọn Cancel
                // Thực hiện hành động tương ứng ở đây
                MessageBox.Show("You have cancelled the invitation.");
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            bool allRowsUpdatedSuccessfully = true;

            if (dgv_Player.RowCount > 0)
            {
                foreach (DataGridViewRow row in dgv_Player.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        int userId = Convert.ToInt32(row.Cells["UserID"].Value);
                        string userName = row.Cells["UserName"].Value.ToString();
                        string email = row.Cells["Email"].Value.ToString();
                        DateTime? joinDate = row.Cells["NGAYTHAMGIA"].Value == null ? (DateTime?)null : Convert.ToDateTime(row.Cells["NGAYTHAMGIA"].Value);
                        string firstName = row.Cells["TEN"].Value.ToString();
                        string lastName = row.Cells["HO"].Value.ToString();
                        string phoneNumber = row.Cells["SDT"].Value.ToString();
                        string country = row.Cells["QUOCGIA"].Value.ToString();
                        string city = row.Cells["THANHPHO"].Value.ToString();
                        string address = row.Cells["Address"].Value.ToString();

                        UpdateUser(userId, userName, email, joinDate, firstName, lastName, phoneNumber, country, city, address);
                    }
                }

                // Kiểm tra xem tất cả các hàng đã được cập nhật thành công hay chưa trước khi hiển thị hộp thông báo
                if (allRowsUpdatedSuccessfully)
                {
                    MessageBox.Show("Thông tin người dùng đã được cập nhật thành công.", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

        }

        private void UpdateUser(int userId, string userName, string email, DateTime? joinDate, string firstName, string lastName, string phoneNumber, string country, string city, string address)
        {
            using (SqlConnection sqlConnection = admin___tke.Kết_nối.getConnection())
            {
                sqlConnection.Open();

                // Create an SQL command to update the user record
                string query = "UPDATE Users SET UserName = @UserName, Email = @Email, NGAYTHAMGIA = @JoinDate, TEN = @FirstName, HO = @LastName, SDT = @PhoneNumber, QUOCGIA = @Country, THANHPHO = @City, Address = @Address WHERE UserID = @UserId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

                sqlCommand.Parameters.AddWithValue("@UserId", userId);
                sqlCommand.Parameters.AddWithValue("@UserName", userName);
                sqlCommand.Parameters.AddWithValue("@Email", email);
                sqlCommand.Parameters.AddWithValue("@JoinDate", joinDate);
                sqlCommand.Parameters.AddWithValue("@FirstName", firstName);
                sqlCommand.Parameters.AddWithValue("@LastName", lastName);
                sqlCommand.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                sqlCommand.Parameters.AddWithValue("@Country", country);
                sqlCommand.Parameters.AddWithValue("@City", city);
                sqlCommand.Parameters.AddWithValue("@Address", address);

                sqlCommand.ExecuteNonQuery();

                sqlConnection.Close();
            }
        }
    }
}
