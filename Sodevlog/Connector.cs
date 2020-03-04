using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Sodevlog;
using Microsoft.Extensions.Logging;
using Sodevlog.Tools;

namespace Sodevlog.Connector
{
    public enum AccessMode { Input };

    public class SqlServerConnector
    {
        private static readonly ILogger Logger = ApplicationLogging.LoggerFactory.CreateLogger<SqlServerConnector>();

        public bool IsOpen { get; set; }

        public string SqlQueryString { get; set; }

        private AccessMode access = new AccessMode();

        private SqlConnection connection { get; set; }

        private SqlCommand command { get; set; }

        public SqlServerConnector()
        {
            IsOpen = false;
        }

        public SqlServerConnector( string connexionString )
        {
            connection = new SqlConnection( connexionString );
            command = new SqlCommand( SqlQueryString, connection );
        }

        public bool TestConnection()
        {
            try
            {
                command.Connection.Open();
                command.Connection.Close();
                return true;
            }
            catch ( Exception e )
            {
                Logger.LogError( e.Message, $"Error lors de la connexion." );
                return false;
            }
        }

        public void Initialize( AccessMode input )
        {
            access = input;
        }

        public void Open()
        {
            command.Connection.Open();
            IsOpen = true;
        }

        public void OpenAsync()
        {
            command.Connection.OpenAsync();
            IsOpen = true;
        }

        public void Close()
        {
            command.Connection.Close();
            IsOpen = false;
        }

        public async Task<string> ExecuteReaderAsyncToJson()
        {
            String result = null;
            SqlDataReader sdr;

            if ( IsOpen )
            {
                //SqlQueryString += " FOR JSON PATH"; BRY_WORK_201912
                command.CommandText = SqlQueryString;
                sdr = await command.ExecuteReaderAsync();
                result = sdr.ToJson();
            }

            return result;
        }

        public async Task<String> ExecuteReaderPreviewAsyncToJson( int topRowNumber )
        {
            String result = null;
            SqlDataReader sdr;

            if ( IsOpen )
            {
                command.Parameters.AddWithValue( "id", topRowNumber );
                //SqlQueryString += " FOR JSON PATH"; BRY_WORK_201912
                command.CommandText = SqlQueryString;
                sdr = await command.ExecuteReaderAsync();
                result = sdr.ToJson();
            }

            return result;
        }
    }
}
