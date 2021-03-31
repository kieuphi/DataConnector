using System;
using System.Reflection;

namespace DataConnector
{
    /// <summary>
    ///     Hold information about a column in the database.
    /// </summary>
    /// <remarks>
    ///     Typically ColumnInfo is automatically populated from the attributes on a POCO object and it's properties. It can
    ///     however also be returned from the IMapper interface to provide your owning bindings between the DB and your POCOs.
    /// </remarks>
    public class ColumnInfo
    {
        /// <summary>
        ///     The SQL name of the column
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        ///     True if this column returns a calculated value from the database and shouldn't be used in Insert and Update
        ///     operations.
        /// </summary>
        public bool ResultColumn { get; set; }

        /// <summary>
        ///     True if time and date values returned through this column should be forced to UTC DateTimeKind. (no conversion is
        ///     applied - the Kind of the DateTime property
        ///     is simply set to DateTimeKind.Utc instead of DateTimeKind.Unknown.
        /// </summary>
        public bool ForceToUtc { get; set; }

        /// <summary>
        ///     The insert template. If not null, this template is used for generating the insert section instead of the deafult
        ///     string.Format("{0}{1}", paramPrefix, index"). Setting this allows DB related interactions, such as "CAST({0}{1} AS
        ///     json)"
        /// </summary>
        public string InsertTemplate { get; set; }

        /// <summary>
        ///     The update template. If not null, this template is used for generating the update section instead of the deafult
        ///     string.Format("{0} = {1}{2}", colName, paramPrefix, index"). Setting this allows DB related interactions, such as "{0} = CAST({1}{2} AS
        ///     json)"
        /// </summary>
        public string UpdateTemplate { get; set; }

        /// <summary>
        ///     Creates and populates a ColumnInfo from the attributes of a POCO property.
        /// </summary>
        /// <param name="propertyInfo">The property whose column info is required</param>
        /// <returns>A ColumnInfo instance</returns>
        public static ColumnInfo FromProperty(PropertyInfo propertyInfo)
        {
            // Check if declaring poco has [Explicit] attribute
            var explicitColumns =
                propertyInfo.DeclaringType.GetCustomAttributes(typeof(ExplicitColumnsAttribute), true).Length > 0;

            // Check for [Column]/[Ignore] Attributes
            var colAttrs = propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), true);
            if (explicitColumns)
            {
                if (colAttrs.Length == 0)
                    return null;
            }
            else
            {
                if (propertyInfo.GetCustomAttributes(typeof(IgnoreAttribute), true).Length != 0)
                    return null;
            }

            var ci = new ColumnInfo();

            // Read attribute
            if (colAttrs.Length > 0)
            {
                var colattr = (ColumnAttribute)colAttrs[0];
                ci.InsertTemplate = colattr.InsertTemplate;
                ci.UpdateTemplate = colattr.UpdateTemplate;
                ci.ColumnName = colattr.Name == null ? propertyInfo.Name : colattr.Name;
                ci.ForceToUtc = colattr.ForceToUtc;
                if ((colattr as ResultColumnAttribute) != null)
                    ci.ResultColumn = true;
            }
            else
            {
                ci.ColumnName = propertyInfo.Name;
                ci.ForceToUtc = false;
                ci.ResultColumn = false;
            }

            return ci;
        }
    }

    /// <summary>
    ///     Represents an attribute which can decorate a Poco property to mark the property as a column. It may also optionally
    ///     supply the DB column name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        /// <summary>
        ///     The SQL name of the column
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     True if time and date values returned through this column should be forced to UTC DateTimeKind. (no conversion is
        ///     applied - the Kind of the DateTime property
        ///     is simply set to DateTimeKind.Utc instead of DateTimeKind.Unknown.
        /// </summary>
        public bool ForceToUtc { get; set; }

        /// <summary>
        ///     The insert template. If not null, this template is used for generating the insert section instead of the deafult
        ///     string.Format("{0}{1}", paramPrefix, index"). Setting this allows DB related interactions, such as "CAST({0}{1} AS
        ///     json)"
        /// </summary>
        public string InsertTemplate { get; set; }

        /// <summary>
        ///     The update template. If not null, this template is used for generating the update section instead of the deafult
        ///     string.Format("{0} = {1}{2}", colName, paramPrefix, index"). Setting this allows DB related interactions, such as "{0} = CAST({1}{2} AS
        ///     json)"
        /// </summary>
        public string UpdateTemplate { get; set; }

        /// <summary>
        ///     Constructs a new instance of the <seealso cref="ColumnAttribute" />.
        /// </summary>
        public ColumnAttribute()
        {
            ForceToUtc = false;
        }

        /// <summary>
        ///     Constructs a new instance of the <seealso cref="ColumnAttribute" />.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        public ColumnAttribute(string name)
        {
            Name = name;
            ForceToUtc = false;
        }
    }

    /// <summary>
    ///     Represents the attribute which decorates a poco class to state all columns must be explicitly mapped using either a
    ///     <seealso cref="ColumnAttribute" /> or <seealso cref="ResultColumnAttribute" />.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ExplicitColumnsAttribute : Attribute
    {
    }

    /// <summary>
    ///     Represents an attribute which can decorate a Poco property to ensure PetaPoco does not map column, and therefore
    ///     ignores the column.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    {
    }

    /// <summary>
    ///     Is an attribute, which when applied to a Poco class, specifies primary key column. Additionally, specifies whether
    ///     the column is auto incrementing and the optional sequence name for Oracle sequence columns.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PrimaryKeyAttribute : Attribute
    {
        /// <summary>
        ///     The column name.
        /// </summary>
        /// <returns>
        ///     The column name.
        /// </returns>
        public string Value { get; private set; }

        /// <summary>
        ///     The sequence name.
        /// </summary>
        /// <returns>
        ///     The sequence name.
        /// </returns>
        public string SequenceName { get; set; }

        /// <summary>
        ///     A flag which specifies if the primary key is auto incrementing.
        /// </summary>
        /// <returns>
        ///     True if the primary key is auto incrementing; else, False.
        /// </returns>
        public bool AutoIncrement { get; set; }

        /// <summary>
        ///     Constructs a new instance of the <seealso cref="PrimaryKeyAttribute" />.
        /// </summary>
        /// <param name="primaryKey">The name of the primary key column.</param>
        public PrimaryKeyAttribute(string primaryKey)
        {
            Value = primaryKey;
            AutoIncrement = true;
        }
    }

    /// <summary>
    ///     Represents an attribute which can decorate a poco property as a result only column. A result only column is a
    ///     column that is only populated in queries and is not used for updates or inserts operations.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ResultColumnAttribute : ColumnAttribute
    {
        /// <summary>
        ///     Constructs a new instance of the <seealso cref="ResultColumnAttribute" />.
        /// </summary>
        public ResultColumnAttribute()
        {
        }

        /// <summary>
        ///     Constructs a new instance of the <seealso cref="ResultColumnAttribute" />.
        /// </summary>
        /// <param name="name">The name of the DB column.</param>
        public ResultColumnAttribute(string name)
            : base(name)
        {
        }
    }

    /// <summary>
    ///     Represents an attribute, which when applied to a Poco class, specifies the the DB table name which it maps to
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableNameAttribute : Attribute
    {
        /// <summary>
        ///     The table nane of the database that this entity maps to.
        /// </summary>
        /// <returns>
        ///     The table nane of the database that this entity maps to.
        /// </returns>
        public string Value { get; private set; }

        /// <summary>
        ///     Constructs a new instance of the <seealso cref="TableNameAttribute" />.
        /// </summary>
        /// <param name="tableName">The table nane of the database that this entity maps to.</param>
        public TableNameAttribute(string tableName)
        {
            Value = tableName;
        }
    }

    /// <summary>
    /// Represents an attribute which can decorate a Poco property conver value from database type to property type and conversely.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class ValueConverterAttribute : Attribute
    {
        /// <summary>
        /// Function to convert property value to database type value.
        /// </summary>
        /// <param name="value">Property value</param>
        /// <returns>Converted database value</returns>
        public abstract object ConvertToDb(object value);

        /// <summary>
        /// Function to convert database value to property type value.
        /// </summary>
        /// <param name="value">Database value</param>
        /// <returns>Converted property type value</returns>
        public abstract object ConvertFromDb(object value);
    }

    /// <summary>
    ///     Wrap strings in an instance of this class to force use of DBType.AnsiString
    /// </summary>
    public class AnsiString
    {
        /// <summary>
        ///     The string value
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        ///     Constructs an AnsiString
        /// </summary>
        /// <param name="str">The C# string to be converted to ANSI before being passed to the DB</param>
        public AnsiString(string str)
        {
            Value = str;
        }
    }

    /// <summary>
    ///     Hold information about a column in the database.
    /// </summary>
    /// <remarks>
    ///     Typically ColumnInfo is automatically populated from the attributes on a POCO object and it's properties. It can
    ///     however also be returned from the IMapper interface to provide your owning bindings between the DB and your POCOs.
    /// </remarks>
}