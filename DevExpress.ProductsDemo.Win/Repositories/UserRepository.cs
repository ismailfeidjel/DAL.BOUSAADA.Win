using DevExpress.ProductsDemo.Win.Core.Helpers;
using DevExpress.ProductsDemo.Win.Domain;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace DevExpress.ProductsDemo.Win.Repositories
{
    public class UserRepository
    {
        private readonly DbHelper _db = new DbHelper();

        public List<UserItem> GetAll()
        {
            var list = new List<UserItem>();
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string sql = "SELECT id, username, full_name, role, is_active FROM users ORDER BY username";
                using (var cmd = new MySqlCommand(sql, conn))
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new UserItem
                        {
                            Id = rd.GetInt32(rd.GetOrdinal("id")),
                            Username = rd.GetString(rd.GetOrdinal("username")),
                            FullName = rd.GetString(rd.GetOrdinal("full_name")),
                            Role = rd.GetString(rd.GetOrdinal("role")),
                            IsActive = rd.GetBoolean(rd.GetOrdinal("is_active"))
                        });
                    }
                }
            }
            return list;
        }

        public void Insert(UserItem user)
        {
            PasswordHasher.CreateHash(user.PlainPassword, out string hash, out string salt);

            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string sql = @"INSERT INTO users (username, full_name, password_hash, password_salt, role, is_active, created_at)
                               VALUES (@username, @full_name, @hash, @salt, @role, @is_active, NOW())";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@username", user.Username);
                    cmd.Parameters.AddWithValue("@full_name", user.FullName);
                    cmd.Parameters.AddWithValue("@hash", hash);
                    cmd.Parameters.AddWithValue("@salt", salt);
                    cmd.Parameters.AddWithValue("@role", user.Role);
                    cmd.Parameters.AddWithValue("@is_active", user.IsActive);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Profile fields only — never touches the password
        public void Update(UserItem user)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string sql = @"UPDATE users SET username=@username, full_name=@full_name,
                               role=@role, is_active=@is_active WHERE id=@id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@username", user.Username);
                    cmd.Parameters.AddWithValue("@full_name", user.FullName);
                    cmd.Parameters.AddWithValue("@role", user.Role);
                    cmd.Parameters.AddWithValue("@is_active", user.IsActive);
                    cmd.Parameters.AddWithValue("@id", user.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void ResetPassword(int userId, string newPlainPassword)
        {
            PasswordHasher.CreateHash(newPlainPassword, out string hash, out string salt);

            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string sql = "UPDATE users SET password_hash=@hash, password_salt=@salt WHERE id=@id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@hash", hash);
                    cmd.Parameters.AddWithValue("@salt", salt);
                    cmd.Parameters.AddWithValue("@id", userId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string sql = "DELETE FROM users WHERE id=@id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Validates credentials. Handles two cases:
        /// 1. password_salt IS NULL (legacy plaintext row) — compares password_hash directly
        ///    as plaintext; on success, immediately re-hashes and upgrades that row so it's
        ///    never compared as plaintext again.
        /// 2. password_salt is set — normal PBKDF2 verification.
        /// Returns null if credentials are invalid or the account is inactive.
        /// </summary>
        public UserItem ValidateLogin(string username, string password)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT id, username, full_name, password_hash, password_salt, role, is_active
                               FROM users WHERE username=@username";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    using (var rd = cmd.ExecuteReader())
                    {
                        if (!rd.Read()) return null;

                        bool isActive = rd.GetBoolean(rd.GetOrdinal("is_active"));
                        if (!isActive) return null;

                        string storedHash = rd.GetString(rd.GetOrdinal("password_hash"));
                        int saltOrdinal = rd.GetOrdinal("password_salt");
                        string storedSalt = rd.IsDBNull(saltOrdinal) ? null : rd.GetString(saltOrdinal);

                        int id = rd.GetInt32(rd.GetOrdinal("id"));
                        string fullName = rd.GetString(rd.GetOrdinal("full_name"));
                        string role = rd.GetString(rd.GetOrdinal("role"));

                        bool passwordOk;

                        if (storedSalt == null)
                        {
                            // Legacy plaintext row — direct comparison
                            passwordOk = storedHash == password;

                            if (passwordOk)
                            {
                                // Upgrade silently on successful legacy login
                                ResetPassword(id, password);
                            }
                        }
                        else
                        {
                            passwordOk = PasswordHasher.Verify(password, storedHash, storedSalt);
                        }

                        if (!passwordOk) return null;

                        return new UserItem
                        {
                            Id = id,
                            Username = username,
                            FullName = fullName,
                            Role = role,
                            IsActive = isActive
                        };
                    }
                }
            }
        }

        public bool UsernameExists(string username, int excludeId = 0)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string sql = "SELECT COUNT(*) FROM users WHERE username=@username AND id <> @excludeId";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@excludeId", excludeId);
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }
    }
}