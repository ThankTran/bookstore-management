using System;
using System.Windows;
using System.Configuration;

namespace bookstore_Management.Core.Utils
{
    /// <summary>
    /// Hô trợ đọc các thông tin từ app.config
    /// </summary>
    internal class ConfigManager
    {
        
        /// <summary>
        /// Hàm lấy chỗi từ app.config
        /// </summary>
        /// <param name="connectionStringName"> tên của connection string mặc định là "defaultConnection"</param>
        /// <returns></returns>
        /// <exception cref="ConfigurationErrorsException"></exception>
        public static string GetConnectionString(string connectionStringName = "defaultConnection")
        {
            try
            {
                var connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionStringName];
                
                return (connectionStringSettings == null)
                    ? throw new ConfigurationErrorsException($"Connection string '{connectionStringName}' not found in App.config")
                        : connectionStringSettings.ConnectionString;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi đọc connection string: {ex.Message}", "Configuration Error");
                return null;
            }
        }
        
        
        /// <summary>
        /// Hàm lấy giá trị từ app settings với kiểu dữ liệu xác định (generic)
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu trả về (int, bool, decimal...)</typeparam>
        /// <param name="key">Khóa cần lấy giá trị</param>
        /// <param name="defaultValue">Giá trị mặc định nếu không tìm thấy</param>
        /// <returns>Giá trị từ app settings hoặc giá trị mặc định</returns>
        public static T GetAppSetting<T>(string key, T defaultValue = default(T))
        {
            try
            {
                string value = ConfigurationManager.AppSettings[key];
                if (string.IsNullOrEmpty(value))
                    return defaultValue;

                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
