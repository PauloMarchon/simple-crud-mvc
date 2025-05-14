using CrudMVC.Models;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace CrudMVC.Data
{
    public class DataAccess
    {
        OracleConnection _connection;
        OracleCommand _command;

        public static IConfiguration Configuration { get; set; }

        private String GetConnectionString()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            return Configuration.GetConnectionString("DefaultConnection");
        }


        public List<Usuario> listarUsuarios()
        {
            List<Usuario> usuarios = new List<Usuario>();

            using (_connection = new OracleConnection(GetConnectionString()))
            {
                using (_command = new OracleCommand("SP_LISTAR_USUARIOS", _connection))
                {
                    _command.CommandType = System.Data.CommandType.StoredProcedure;
                    _command.Parameters.Add("P_CURSOR", OracleDbType.RefCursor, System.Data.ParameterDirection.Output);

                    try
                    {
                        _connection.Open();
                        using (OracleDataReader reader = _command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Usuario usuario = new Usuario();
                                byte[] idBytes = (byte[])reader["ID"];
                                usuario.Id = new Guid(idBytes);
                                usuario.Nome = reader["NOME"].ToString();
                                usuario.Email = reader["EMAIL"].ToString();
                                usuario.Cargo = reader["CARGO"].ToString();
                                usuarios.Add(usuario);
                            }
                        }
                    }
                    catch (OracleException ex)
                    {
                        Console.WriteLine(ex.Message);

                    }
                    finally
                    {
                        if (_connection.State == System.Data.ConnectionState.Open)
                        {
                            _connection.Close();
                        }
                    }
                }
            }
            return usuarios;
        }

        public bool CadastrarUsuario(Usuario usuario)
        {
            using (_connection = new OracleConnection(GetConnectionString()))
            {
                using (_command = new OracleCommand("SP_CRIAR_NOVO_USUARIO", _connection))
                {
                    Guid novoUsuarioId = Guid.NewGuid();
                    _command.CommandType = System.Data.CommandType.StoredProcedure;
                    _command.Parameters.Add("P_ID", OracleDbType.Raw).Value = novoUsuarioId.ToByteArray();
                    _command.Parameters.Add("P_NOME", OracleDbType.Varchar2).Value = usuario.Nome;
                    _command.Parameters.Add("P_EMAIL", OracleDbType.Varchar2).Value = usuario.Email;
                    _command.Parameters.Add("P_CARGO", OracleDbType.Varchar2).Value = usuario.Cargo;
                    try
                    {
                        _connection.Open();
                        _command.ExecuteNonQuery();
                        return true;
                    }
                    catch (OracleException ex)
                    {
                        Console.WriteLine(ex.Message);
                        return false;
                    }
                    finally
                    {
                        if (_connection.State == System.Data.ConnectionState.Open)
                        {
                            _connection.Close();
                        }
                    }
                }
            }
        }

        public bool EditarUsuario(Usuario usuario)
        {
            using (_connection = new OracleConnection(GetConnectionString()))
            {
                using (_command = new OracleCommand("SP_EDITAR_USUARIO_POR_ID", _connection))
                {
                    _command.CommandType = System.Data.CommandType.StoredProcedure;
                    _command.Parameters.Add("P_USUARIO_ID", OracleDbType.Raw, usuario.Id.ToByteArray(), System.Data.ParameterDirection.Input);
                    _command.Parameters.Add("P_NOVO_NOME", OracleDbType.Varchar2).Value = usuario.Nome;
                    _command.Parameters.Add("P_NOVO_EMAIL", OracleDbType.Varchar2).Value = usuario.Email;
                    _command.Parameters.Add("P_NOVO_CARGO", OracleDbType.Varchar2).Value = usuario.Cargo;
                    _command.Parameters.Add("P_LINHAS_AFETADAS", OracleDbType.Int32, System.Data.ParameterDirection.Output);
                    
                    try
                    {
                        _connection.Open();
                        _command.ExecuteNonQuery();

                        object linhasAfetadasValue = _command.Parameters["P_LINHAS_AFETADAS"].Value;

                        int linhasAfetadas = 0; 

                        if (linhasAfetadasValue != null && linhasAfetadasValue != DBNull.Value)
                        {
                            Oracle.ManagedDataAccess.Types.OracleDecimal oracleDecimalValue = (Oracle.ManagedDataAccess.Types.OracleDecimal)linhasAfetadasValue;

                            linhasAfetadas = oracleDecimalValue.ToInt32();
                        }

                        if (linhasAfetadas > 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }

                    }
                    catch (Oracle.ManagedDataAccess.Client.OracleException ex) // Catch a exceção específica do ODP.NET
                    {
                        Console.WriteLine($"Erro Oracle ao editar usuário: {ex.Message}");
                        // Logar ou tratar a exceção
                        return false;
                    }
                    catch (Exception ex) // Captura outras exceções gerais
                    {
                        Console.WriteLine($"Erro geral ao editar usuário: {ex.Message}");
                        // Logar ou tratar a exceção
                        return false;
                    }
                    finally
                    {
                        if (_connection.State == System.Data.ConnectionState.Open)
                        {
                            _connection.Close();
                        }
                    }
                }
            }
        }

        public Usuario BuscarUsuarioPorId(Guid id)
        {
            Usuario usuario = new Usuario();

            using (_connection = new OracleConnection(GetConnectionString()))
            {
                using (_command = new OracleCommand("SP_BUSCAR_USUARIO_POR_ID", _connection))
                {
                    _command.CommandType = System.Data.CommandType.StoredProcedure;
                    _command.Parameters.Add("P_ID", OracleDbType.Raw, id.ToByteArray(), System.Data.ParameterDirection.Input);
                    _command.Parameters.Add("P_CURSOR", OracleDbType.RefCursor, System.Data.ParameterDirection.Output);
                    
                    try
                    {
                        _connection.Open();
                        _command.ExecuteNonQuery();

                        using (OracleDataReader reader = ((OracleRefCursor)_command.Parameters["P_CURSOR"].Value).GetDataReader())
                        {
                            if (reader.Read())
                            {
                                byte[] idBytes = (byte[])reader["ID"];
                                usuario.Id = new Guid(idBytes);
                                usuario.Nome = reader["NOME"].ToString();
                                usuario.Email = reader["EMAIL"].ToString();
                                usuario.Cargo = reader["CARGO"].ToString();
                            }
                        }
                    }
                    catch (OracleException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        if (_connection.State == System.Data.ConnectionState.Open)
                        {
                            _connection.Close();
                        }
                    }
                }
            }
            return usuario;
        }

        public bool RemoverUsuario(Guid id)
        {
            using (_connection = new OracleConnection(GetConnectionString()))
            {
                using (_command = new OracleCommand("SP_REMOVER_USUARIO_POR_ID", _connection))
                {
                    _command.CommandType = System.Data.CommandType.StoredProcedure;
                    _command.Parameters.Add("P_ID", OracleDbType.Raw, id.ToByteArray(), System.Data.ParameterDirection.Input);
                    _command.Parameters.Add("P_LINHAS_AFETADAS", OracleDbType.Int32, System.Data.ParameterDirection.Output);

                    try
                    {
                        _connection.Open();
                        _command.ExecuteNonQuery();

                        object linhasAfetadasValue = _command.Parameters["P_LINHAS_AFETADAS"].Value;

                        int linhasAfetadas = 0;

                        if (linhasAfetadasValue != null && linhasAfetadasValue != DBNull.Value)
                        {
                            Oracle.ManagedDataAccess.Types.OracleDecimal oracleDecimalValue = (Oracle.ManagedDataAccess.Types.OracleDecimal)linhasAfetadasValue;

                            linhasAfetadas = oracleDecimalValue.ToInt32();
                        }

                        if (linhasAfetadas > 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    catch (OracleException ex)
                    {
                        Console.WriteLine(ex.Message);
                        return false;
                    }
                    finally
                    {
                        if (_connection.State == System.Data.ConnectionState.Open)
                        {
                            _connection.Close();
                        }
                    }
                }
            }
        }
    }
} 
