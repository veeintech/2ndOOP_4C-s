using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4C_s_Management_System
{
    internal class AppDB
    {
        /// <summary>
        /// Centralized database provider. All forms should use AppDb.Instance.
        /// </summary>
        private static readonly Lazy<IDatabase> _instance = new Lazy<IDatabase>(() =>
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["AppDB"].ConnectionString;
                return new SqlDatabase(connStr);
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw new InvalidOperationException("Failed to initialize the database.", ex);
            }
        });

        /// <summary>
        /// The shared database instance used across the app.
        /// </summary>
        public static IDatabase Instance => _instance.Value;

        /// <summary>
        /// Call this on app exit to clean up.
        /// </summary>
        public static void Dispose()
        {
            if (_instance.IsValueCreated)
                _instance.Value.Dispose();
        }
    }
}
