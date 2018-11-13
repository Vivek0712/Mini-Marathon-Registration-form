using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Marathon
{
    public partial class Register : System.Web.UI.Page
    {
        private SqlConnection con;
        private DataTable dt;
        private SqlDataAdapter sda;
        private DataTable Participant = new DataTable();
        int newbie=0;

        protected SqlConnection GetConnection()
        {
            string connetionString;
            //connetionString = @"Data Source=DESKTOP-NCLVKS1;Initial Catalog=MiniMarathon ;User ID=sa;Password=";
            con = new SqlConnection(connetionString);
            return con;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["Admnno"] = "12735";
            Participant = new DataTable();
            if (!IsPostBack)
            {
//change 1
                DateTime EndDate =new DateTime(2018, 08, 27);
                if (DateTime.Now.Date != EndDate.Date)
                {
                    displayCurrentStatus();
                }
                else
                {
                    DisplayResult();
                }
            }
        }
        protected void DisplayResult()
        {
            string sql = "Select * from ParticipationList where AdminNo = @admnno";
            SqlCommand cmd = new SqlCommand(sql);
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("admnno", Convert.ToInt32(Session["Admnno"]));
            Participant = LoadDataFromTable(cmd, param);
            if(Participant.Rows.Count == 1)
            {
                if(Convert.ToBoolean(Participant.Rows[0]["IsParticipating"]) == true)
                {
                    SelectionRadioList.Items[0].Enabled = false;
                    SelectionRadioList.Items[1].Enabled = false;
                    SubmitBtn.Enabled = false;
                    SelectionRadioList.Items[1].Selected = false;
                    SelectionRadioList.Items[0].Selected = false;
                    StatusResult.Text = " You are participating";
                    StatusResult.ForeColor = System.Drawing.Color.Green;
                    StatusResult.BackColor = System.Drawing.Color.White;
                }
                else
                {
                    SelectionRadioList.Items[0].Enabled = false;
                    SelectionRadioList.Items[1].Enabled = false;
                    SubmitBtn.Enabled = false;
                    SelectionRadioList.Items[1].Selected = false;
                    SelectionRadioList.Items[0].Selected = false;
                    StatusResult.Text = " You are not participating";
                    StatusResult.ForeColor = System.Drawing.Color.Red;
                    StatusResult.BackColor = System.Drawing.Color.White;
                }
            }
            else
            {
                SelectionRadioList.Items[0].Enabled = false;
                SelectionRadioList.Items[1].Enabled = false;
                SubmitBtn.Enabled = false;
                SelectionRadioList.Items[1].Selected = false;
                SelectionRadioList.Items[0].Selected = false;
                StatusResult.Text = " You have not selected";
                StatusResult.ForeColor = System.Drawing.Color.Red;
                StatusResult.BackColor = System.Drawing.Color.White;
            }
        }
 //change 1 till here
        protected void displayCurrentStatus()
        {
            string sql = "Select * from ParticipationList where AdminNo = @admnno";
            SqlCommand cmd = new SqlCommand(sql);
             SqlParameter[] param = new SqlParameter[1];
             param[0] = new SqlParameter("admnno", Convert.ToInt32(Session["Admnno"]));
            Participant = LoadDataFromTable(cmd,param);
            if (Participant.Rows.Count == 1)
            {
                DateTime LastUpdated = (DateTime)Participant.Rows[0]["LastUpdated"];
                if ((LastUpdated == DateTime.Now.Date) && (Convert.ToInt32(Participant.Rows[0]["Count"]) < 2))
                {
                    SelectionRadioList.Items[0].Enabled = true;
                    SelectionRadioList.Items[1].Enabled = true;
                    if (Convert.ToInt32(Participant.Rows[0]["IsParticipating"]) == 0)
                    {
                        SelectionRadioList.Items[1].Selected = true;
                        StatusResult.Text = "No";
                        StatusResult.ForeColor = System.Drawing.Color.Red;
                    }
                    else
                    {
                        SelectionRadioList.Items[0].Selected = true;
                        StatusResult.Text = "Yes";
                        StatusResult.ForeColor = System.Drawing.Color.Green;
                    }
                }
                else if ((Convert.ToInt32(Participant.Rows[0]["Count"]) >= 2) && (LastUpdated == DateTime.Now.Date))
                {
                    SelectionRadioList.Items[0].Enabled = false;
                    SelectionRadioList.Items[1].Enabled = false;
                    if (Convert.ToInt32(Participant.Rows[0]["IsParticipating"]) == 0)
                    {
                        SelectionRadioList.Items[1].Selected = true;
                        StatusResult.Text = "No";
                        StatusResult.ForeColor = System.Drawing.Color.Red;
                    }
                    else
                    {
                        SelectionRadioList.Items[0].Selected = true;
                        StatusResult.Text = "Yes";
                        StatusResult.ForeColor = System.Drawing.Color.Green;
                    }
                    ErrorLbl.Visible = true;
                    SubmitBtn.Visible = false;
                }
                else
                {
                    string sql1 = "Update ParticipationList set LastUpdated = @UpdateDate, Count = 0 where AdminNo = @admnno";
                    SqlCommand cmdUpdate = new SqlCommand(sql1);
                    SqlParameter[] param1 = new SqlParameter[2];
                    param1[0] = new SqlParameter("UpdateDate", DateTime.Now.Date);
                    param1[1] = new SqlParameter("admnno", Convert.ToInt32(Session["Admnno"]));
                    int res = ExecuteQuery(cmdUpdate, param1);
                    SelectionRadioList.Items[0].Enabled = true;
                    SelectionRadioList.Items[1].Enabled = true;
                    if (Convert.ToInt32(Participant.Rows[0]["IsParticipating"]) == 0)
                    {
                        SelectionRadioList.Items[1].Selected = true;
                        StatusResult.Text = "No";
                        StatusResult.ForeColor = System.Drawing.Color.Red;
                    }
                    else
                    {
                        SelectionRadioList.Items[0].Selected = true;
                        StatusResult.Text = "Yes";
                        StatusResult.ForeColor = System.Drawing.Color.Green;
                    }
                }
            }
            else
            {
//change 2
                SelectionRadioList.Items[0].Enabled = true;
                SelectionRadioList.Items[1].Enabled = true;
                StatusResult.Text = " You have not selected";
                StatusResult.ForeColor = System.Drawing.Color.Red;
                StatusResult.BackColor = System.Drawing.Color.White;
                System.Diagnostics.Debug.WriteLine(newbie);
//change 2 till here
            }
        }

        public DataTable LoadDataFromTable(SqlCommand cmd)
        {
            dt = new DataTable();
            try
            {
                con = GetConnection();
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    sda = new SqlDataAdapter(cmd.CommandText, con);

                    dt.Clear();
                    //ds.Tables.Clear();
                    //ds.Tables.Add(dt);
                    sda.Fill(dt);
                }
                return dt;

            }
            catch (SqlException se)
            {
                throw se;
            }
            catch (NullReferenceException nre)
            {
                throw nre;
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
                if (dt != null)
                    dt.Dispose();
                if (con != null && con.State == ConnectionState.Open)
                    con.Close();

            }
        }

        public DataTable LoadDataFromTable(SqlCommand cmd, SqlParameter[] param)
        {
            dt = new DataTable();
            try
            {
                con = GetConnection();
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    cmd.Parameters.Clear();
                    
                    cmd.Parameters.AddRange(param);
                    cmd.Connection = con;
                    sda = new SqlDataAdapter(cmd);

                    dt.Clear();
                    //ds.Tables.Clear();
                    //ds.Tables.Add(dt);
                    sda.Fill(dt);
                }
                return dt;

            }
            catch (SqlException se)
            {
                throw se;
            }
            catch (NullReferenceException nre)
            {
                throw nre;
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
                if (dt != null)
                    dt.Dispose();
                if (con != null && con.State == ConnectionState.Open)
                    con.Close();

            }
        }


        public int ExecuteQuery(SqlCommand cmd, SqlParameter[] param)
        {
            int res = 0;
            try
            {
                con = GetConnection();
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Clear();
                    for (int i = 0; i < param.Length; i++)
                        cmd.Parameters.Add(param[i]);
                    res = cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException se)
            {
                res = 0;
                throw se;
            }
            catch (NullReferenceException nre)
            {
                res = 0;
                throw nre;
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
                if (con != null && con.State == ConnectionState.Open)
                    con.Close();


            }

            return res;
        }

        protected void SubmitBtn_Click(object sender, EventArgs e)
        {
            string selected = SelectionRadioList.SelectedValue;
            int res;
            if (newbie == 0)
            {
                if (selected == "1")
                {
                    string sql = "Update ParticipationList set IsParticipating = 1, LastUpdated = @UpdateDate, Count = Count + 1 where AdminNo = @admnno";
                    SqlCommand cmd = new SqlCommand(sql);
                    SqlParameter[] param = new SqlParameter[2];
                    param[0] = new SqlParameter("UpdateDate", DateTime.Now.Date);
                    param[1] = new SqlParameter("admnno", Convert.ToInt32(Session["Admnno"]));
                    res = ExecuteQuery(cmd, param);
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Submission Successful')", true);
                    displayCurrentStatus();
                }
                else
                {
                    string sql = "Update ParticipationList set IsParticipating = 0, LastUpdated = @UpdateDate, Count = Count + 1 where AdminNo = @admnno";
                    SqlCommand cmd = new SqlCommand(sql);
                    SqlParameter[] param = new SqlParameter[2];
                    param[0] = new SqlParameter("UpdateDate", DateTime.Now.Date);
                    param[1] = new SqlParameter("admnno", Convert.ToInt32(Session["Admnno"]));
                    res = ExecuteQuery(cmd, param);
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Submission Successful')", true);
                    displayCurrentStatus();
                }
            }
            else
            {
                if (selected == "1")
                {
                    string sql1 = "Insert into ParticipationList values (@admnno,1,@date,1)";
                    SqlCommand cmdUpdate = new SqlCommand(sql1);
                    SqlParameter[] param1 = new SqlParameter[2];
                    param1[0] = new SqlParameter("admnno", Convert.ToInt32(Session["Admnno"]));
                    param1[1] = new SqlParameter("date", DateTime.Now.Date);
                    res = ExecuteQuery(cmdUpdate, param1);
                    displayCurrentStatus();
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Submission Successful')", true);
                    displayCurrentStatus();
                }
                else
                {
                    string sql1 = "Insert into ParticipationList values (@admnno,0,@date,1)";
                    SqlCommand cmdUpdate = new SqlCommand(sql1);
                    SqlParameter[] param1 = new SqlParameter[2];
                    param1[0] = new SqlParameter("admnno", Convert.ToInt32(Session["Admnno"]));
                    param1[1] = new SqlParameter("date", DateTime.Now.Date);
                    res = ExecuteQuery(cmdUpdate, param1);
                    displayCurrentStatus();
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Submission Successful')", true);
                    displayCurrentStatus();
                }
                
            }
           
        }
    }
}