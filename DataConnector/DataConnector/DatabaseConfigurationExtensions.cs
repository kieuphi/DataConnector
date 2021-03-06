using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataConnector
{
    /// <summary>
    ///     A static helper class where extensions for <see cref="IDatabaseBuildConfiguration" /> are placed.
    /// </summary>
    public static class DatabaseConfigurationExtensions
    {
        internal const string CommandTimeout = "CommandTimeout";

        internal const string EnableAutoSelect = "EnableAutoSelect";

        internal const string EnableNamedParams = "EnableNamedParams";

        internal const string Provider = "Provider";

        internal const string ConnectionString = "ConnectionString";

        internal const string ConnectionStringName = "ConnectionStringName";

        internal const string DefaultMapper = "DefaultMapper";

        internal const string IsolationLevel = "IsolationLevel";

        private static void SetSetting(this IDatabaseBuildConfiguration source, string key, object value)
        {
            ((IBuildConfigurationSettings)source).SetSetting(key, value);
        }

        /// <summary>
        ///     Adds a command timeout - see <see cref="IDatabase.CommandTimeout" />.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <param name="seconds">The timeout in seconds.</param>
        /// <exception cref="ArgumentException">Thrown when seconds is less than 1.</exception>
        /// <returns>The configuration source to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration UsingCommandTimeout(this IDatabaseBuildConfiguration source, int seconds)
        {
            if (seconds < 1)
                throw new ArgumentException("Timeout value must be greater than zero.");
            source.SetSetting(CommandTimeout, seconds);
            return source;
        }

        /// <summary>
        ///     Enables named params - see <see cref="IDatabase.EnableNamedParams" />.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <returns>The configuration source to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration WithNamedParams(this IDatabaseBuildConfiguration source)
        {
            source.SetSetting(EnableNamedParams, true);
            return source;
        }

        /// <summary>
        ///     Disables named params - see <see cref="IDatabase.EnableNamedParams" />.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <returns>The configuration source to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration WithoutNamedParams(this IDatabaseBuildConfiguration source)
        {
            source.SetSetting(EnableNamedParams, false);
            return source;
        }

        /// <summary>
        ///     Specifies the provider to be used. - see <see cref="IDatabase.Provider" />.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <param name="provider">The provider to use.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="provider" /> is null.</exception>
        /// <returns>The configuration source to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration UsingProvider<T>(this IDatabaseBuildConfiguration source, T provider)
            where T : class, IProvider
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            source.SetSetting(Provider, provider);
            return source;
        }

        /// <summary>
        ///     Specifies the provider to be used. - see <see cref="IDatabase.Provider" />.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <param name="configure">The configure provider callback.</param>
        /// <param name="provider">The provider to use.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="provider" /> is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="configure" /> is null.</exception>
        /// <returns>The configuration source to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration UsingProvider<T>(this IDatabaseBuildConfiguration source, T provider, Action<T> configure)
            where T : class, IProvider
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (configure == null)
                throw new ArgumentNullException("configure");
            source.SetSetting(Provider, provider);
            return source;
        }

        /// <summary>
        ///     Specifies the provider to be used. - see <see cref="IDatabase.Provider" />.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <typeparam name="T">The provider type.</typeparam>
        /// <returns>The configuration source to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration UsingProvider<T>(this IDatabaseBuildConfiguration source)
            where T : class, IProvider, new()
        {
            source.SetSetting(Provider, new T());
            return source;
        }

        /// <summary>
        ///     Specifies the provider to be used. - see <see cref="IDatabase.Provider" />.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <param name="configure">The configure provider callback.</param>
        /// <typeparam name="T">The provider type.</typeparam>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="configure" /> is null.</exception>
        /// <returns>The configuration source to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration UsingProvider<T>(this IDatabaseBuildConfiguration source, Action<T> configure)
            where T : class, IProvider, new()
        {
            if (configure == null)
                throw new ArgumentNullException("configure");
            var provider = new T();
            configure(provider);
            source.SetSetting(Provider, provider);
            return source;
        }

        /// <summary>
        ///     Enables auto select - see <see cref="IDatabase.EnableAutoSelect" />.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <returns>The configuration source to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration WithAutoSelect(this IDatabaseBuildConfiguration source)
        {
            source.SetSetting("EnableAutoSelect", true);
            return source;
        }

        /// <summary>
        ///     Disables auto select - see <see cref="IDatabase.EnableAutoSelect" />.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <returns>The configuration source to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration WithoutAutoSelect(this IDatabaseBuildConfiguration source)
        {
            source.SetSetting("EnableAutoSelect", false);
            return source;
        }

        /// <summary>
        ///     Adds a connection string - see <see cref="IDatabase.ConnectionString" />.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="connectionString" /> is null or empty.</exception>
        /// <returns>The configuration source to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration UsingConnectionString(this IDatabaseBuildConfiguration source, string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Argument is null or empty", "connectionString");
            source.SetSetting(ConnectionString, connectionString);
            return source;
        }

        /// <summary>
        ///     Adds a connection string name.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <param name="connectionStringName">The connection string name.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="connectionStringName" /> is null or empty.</exception>
        /// <returns>The configuration source to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration UsingConnectionStringName(this IDatabaseBuildConfiguration source, string connectionStringName)
        {
            if (string.IsNullOrEmpty(connectionStringName))
                throw new ArgumentException("Argument is null or empty", "connectionStringName");
            source.SetSetting(ConnectionStringName, connectionStringName);
            return source;
        }

        /// <summary>
        ///     Specifies the default mapper to use when no specific mapper has been registered.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <param name="mapper">The mapper to use as the default.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="mapper" /> is null.</exception>
        /// <returns>The configuration source to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration UsingDefaultMapper<T>(this IDatabaseBuildConfiguration source, T mapper)
            where T : class, IMapper
        {
            if (mapper == null)
                throw new ArgumentNullException("mapper");
            source.SetSetting(DefaultMapper, mapper);
            return source;
        }

        /// <summary>
        ///     Specifies the default mapper to use when no specific mapper has been registered.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <param name="mapper">The mapper to use as the default.</param>
        /// <param name="configure">The configure mapper callback.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="mapper" /> is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="configure" /> is null.</exception>
        /// <returns>The configuration source to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration UsingDefaultMapper<T>(this IDatabaseBuildConfiguration source, T mapper, Action<T> configure)
            where T : class, IMapper
        {
            if (mapper == null)
                throw new ArgumentNullException("mapper");
            if (configure == null)
                throw new ArgumentNullException("configure");
            configure(mapper);
            source.SetSetting(DefaultMapper, mapper);
            return source;
        }

        /// <summary>
        ///     Specifies the default mapper to use when no specific mapper has been registered.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <typeparam name="T">The mapper type.</typeparam>
        /// <returns>The configuration source to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration UsingDefaultMapper<T>(this IDatabaseBuildConfiguration source)
            where T : class, IMapper, new()
        {
            source.SetSetting(DefaultMapper, new T());
            return source;
        }

        /// <summary>
        ///     Specifies the default mapper to use when no specific mapper has been registered.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <param name="configure">The configure mapper callback.</param>
        /// <typeparam name="T">The mapper type.</typeparam>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="configure" /> is null.</exception>
        /// <returns>The configuration source to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration UsingDefaultMapper<T>(this IDatabaseBuildConfiguration source, Action<T> configure)
            where T : class, IMapper, new()
        {
            if (configure == null)
                throw new ArgumentNullException("configure");

            var mapper = new T();
            configure(mapper);
            source.SetSetting(DefaultMapper, mapper);
            return source;
        }

        /// <summary>
        ///     Specifies the transaction isolation level to use.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <param name="isolationLevel"></param>
        /// <returns>The configuration source to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration UsingIsolationLevel(this IDatabaseBuildConfiguration source, IsolationLevel isolationLevel)
        {
            source.SetSetting(IsolationLevel, isolationLevel);
            return source;
        }

        /// <summary>
        ///     Creates an instance of PetaPooc using the specified <paramref name="source" />.
        /// </summary>
        /// <param name="source">The configuration source used to create and configure an instance of PetaPoco.</param>
        /// <returns>An instance of PetaPoco.</returns>
        public static IDatabase Create(this IDatabaseBuildConfiguration source)
        {
            return new Database(source);
        }
    }
}