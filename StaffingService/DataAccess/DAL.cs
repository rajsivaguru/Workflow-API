using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace WorkFlow.DataAccess
{
    public class DAL
    {
        SqlConnection objCon=null;

        public DAL(string strCon)
        {
            objCon = new SqlConnection(strCon);
            objCon.Open();
        }

        public DAL()
        {
            objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString);
            objCon.Open();
        }

        public void Close()
        {
            try
            {
                if (objCon != null)
                    objCon.Close();
            }
            catch (Exception objExp)
            {
                throw objExp;
            }
        }

        public int ExecuteSP(string spName, object[][] parms)
        {
            try
            {

                SqlCommand cmd = objCon.CreateCommand();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = spName;

                if (parms != null)
                {
                    for (int iParam = 0; iParam < parms.Length; iParam++)
                    {
                        cmd.Parameters.Add((string)parms[iParam][0], (parms[iParam][1] != null) ? parms[iParam][1] : DBNull.Value);
                    }
                }

                return cmd.ExecuteNonQuery();

            }
            catch (Exception objExp)
            {
                throw objExp;
            }
        }


        public IDataReader ExecuteReader(string spName, object[][] parms)
        {
            IDataReader objRead = null;

            try
            {
                SqlCommand cmd = objCon.CreateCommand();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = spName;

                if (parms != null)
                {
                    for (int iParam = 0; iParam < parms.Length; iParam++)
                    {
                        cmd.Parameters.Add((string)parms[iParam][0], (parms[iParam][1] != null) ? parms[iParam][1] : DBNull.Value);
                    }
                }

                objRead = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception objExp)
            {
                throw objExp;
            }

            return objRead;
        }



        public int ExecuteTable(string spName, DataTable myTable, string xml)
        {
            DataSet objDS = null;
            int iResult;

            try
            {

                SqlCommand cmd = objCon.CreateCommand();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = spName;

                //if (parms != null)
                //{
                //    for (int iParam = 0; iParam < parms.Length; iParam++)
                //    {
                //        cmd.Parameters.Add((string)parms[iParam][0], (parms[iParam][1] != null) ? parms[iParam][1] : DBNull.Value);
                //    }
                //}
                SqlParameter sqlParam = cmd.Parameters.AddWithValue("@JobCodesToRemove", myTable);
                sqlParam.SqlDbType = SqlDbType.Structured;

                SqlParameter sqlParam2 = cmd.Parameters.AddWithValue("@XMLJobs", xml);
                sqlParam2.SqlDbType = SqlDbType.Xml;

                iResult = cmd.ExecuteNonQuery();

                //objDS = new DataSet();
                //SqlDataAdapter sqladap = new SqlDataAdapter(cmd);
                //sqladap.Fill(objDS);
            }
            catch (Exception objExp)
            {
                iResult = 0;
            }

            return iResult;
        }


        public DataSet ExecuteDS(string spName, object[][] parms)
        {
            DataSet objDS = null;

            try
            {
                
                SqlCommand cmd = objCon.CreateCommand();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = spName;

                if (parms != null)
                {
                    for (int iParam = 0; iParam < parms.Length; iParam++)
                    {
                        cmd.Parameters.Add((string)parms[iParam][0], (parms[iParam][1] != null) ? parms[iParam][1] : DBNull.Value);
                    }
                }
                objDS = new DataSet();
                SqlDataAdapter sqladap = new SqlDataAdapter(cmd);
                sqladap.Fill(objDS);
            }
            catch (Exception objExp)
            {
                throw objExp;
            }

            return objDS;
        }


  



        public DataSet ExecuteDS(string sQuery)
        {
            DataSet objDS = null;

            try
            {
                SqlCommand cmd = objCon.CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = sQuery;
                objDS = new DataSet();
                SqlDataAdapter sqladap = new SqlDataAdapter(cmd);
                sqladap.Fill(objDS);
            }
            catch (Exception objExp)
            {
                throw objExp;
            }

            return objDS;
        }


        public IDataReader ExecuteReader(string sqlText)
        {
            IDataReader objRead = null;
            try
            {
                SqlCommand cmd = objCon.CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = sqlText;
                objRead = cmd.ExecuteReader( CommandBehavior.CloseConnection  );
            }
            catch (Exception objExp)
            {
                throw objExp;
            }
            return objRead;
        }


        public void ExecuteQuery(string sqlText)
        {
            try
            {
                SqlCommand cmd = objCon.CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = sqlText;
                cmd.ExecuteNonQuery();
            }
            catch (Exception objExp)
            {
                throw objExp;
            }
        }

        public int ExecuteOuput(string spName, object[][] parms, string outParam, int GroupID)
        {
            int iOutPut = -1;

            try
            {
                SqlCommand cmd = objCon.CreateCommand();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = spName;
                SqlParameter oParam =null;

                if (parms != null)
                {
                    for (int iParam = 0; iParam < parms.Length; iParam++)
                    {
                        cmd.Parameters.Add((string)parms[iParam][0], (parms[iParam][1] != null) ? parms[iParam][1] : DBNull.Value);
                    }
                }

                if ( outParam != "" )
                {
                    oParam = new SqlParameter(outParam,GroupID); 
                    oParam.DbType = DbType.Int32;
                    oParam.Direction = ParameterDirection.InputOutput ;
                    cmd.Parameters.Add(oParam);
                }

                cmd.ExecuteNonQuery();

                if ( oParam != null)
                {
                    iOutPut = Convert.ToInt32(oParam.Value);
                }

                return iOutPut;

            }
            catch (Exception objExp)
            {
                throw objExp;
            }

            return -1;
        }


        public object ExecuteScalar(string sqlText)
        {
            object objReturn =null;
            try
            {
                SqlCommand cmd = objCon.CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = sqlText;
                objReturn = cmd.ExecuteScalar();
            }
            catch (Exception objExp)
            {
                throw objExp;
            }

            return objReturn;
        }
    }
}
