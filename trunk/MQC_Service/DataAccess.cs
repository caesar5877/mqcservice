using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace MQC_Service
{
    class DataAccess
    {
        protected string m_ConnectionString;
        protected SqlConnection m_DbConnection;

        // Constructor
        public DataAccess(string sConnection)
        {
            this.m_DbConnection = new SqlConnection();
            this.m_DbConnection.ConnectionString = sConnection;
        }

        // Property to Get and Set Connection String
        public string ConnectionString
        {
            get { return m_ConnectionString; }
            set { m_ConnectionString = value; }
        }

        // Property to Get an set Database connection Object
        public SqlConnection ConnectionObject
        {
            get { return m_DbConnection; }
            set { m_DbConnection = value; }
        }

        // Function to open database connection
        protected Boolean OpenConnection()
        {
            Boolean b_rtn = false;

            if (m_DbConnection.State == ConnectionState.Closed)
            {
                try
                {
                    m_DbConnection.Open();
                    b_rtn = true;
                }
                catch (Exception e)
                {
                    b_rtn = false;
                }
            }

            return b_rtn;
        }

        // Function to setup database command object
        protected SqlCommand DBCommand()
        {
            SqlCommand myCommand = new SqlCommand();

            this.OpenConnection();
            myCommand = m_DbConnection.CreateCommand();

            return myCommand;
        }


        // Function to get datareader with sql statement
        public SqlDataReader GetDataReader(string sqlquery)
        {
            SqlCommand myCommand = new SqlCommand();
            SqlDataReader myDataReader;

            myCommand = this.DBCommand();
            myCommand.CommandText = sqlquery;
            myDataReader = myCommand.ExecuteReader();

            return myDataReader;
        }


        // Function to get dataset with sql statement
        public DataSet GetDataSet(string sqlquery)
        {
            SqlDataAdapter myDataAdapter;
            SqlCommand myCommand = new SqlCommand();
            DataSet myDataSet;

            myCommand = this.DBCommand();
            myCommand.CommandText = sqlquery;
            myDataAdapter = new SqlDataAdapter();
            myDataAdapter.SelectCommand = myCommand;
            myDataSet = new DataSet();

            myDataAdapter.Fill(myDataSet);
            return myDataSet;
        }

        // Function to get datatable with sql statement
        public DataTable GetDataTable(string sqlquery)
        {
            DataSet myDataSet = new DataSet();
            myDataSet = this.GetDataSet(sqlquery);

            return myDataSet.Tables[0];
        }

        // function to get single value with sql statement
        public Object GetValue(string sqlquery)
        {
            SqlCommand myCommand = new SqlCommand();
            Object obj = new object();

            myCommand = this.DBCommand();
            myCommand.CommandText = sqlquery;

            try
            {
                obj = myCommand.ExecuteScalar().ToString();

                if (obj == null) { return null; }
                else { return obj; }
            }
            catch (Exception e)
            {
                return null;
            }

        }

        // Function to execute sql with sql statement return row affected
        public int ExecuteSql(string sqlquery)
        {
            SqlCommand myCommand = new SqlCommand();

            myCommand = this.DBCommand();
            myCommand.CommandText = sqlquery;

            try
            {
                return myCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        // function to dispose unuse object
        public void Dispose()
        {
            if (m_DbConnection != null)
            {
                m_DbConnection = null;
            }
        }

        // Destructor
        ~DataAccess()
        {
            this.Dispose();
        }
    }
}
