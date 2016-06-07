namespace DbReader
{
    internal class ErrorMessages
    {
        public const string ConstructorNotFound = "The type ({0}) does not contain a public parameterless constructor.";
        public const string DuplicateFieldName = "The data record contains a duplicate field name ({0}). Make sure that every column/field name in the data record is unique.";
        public const string IncompatibleTypes = "The property ({0}) is not compatible with the column ({1})returned from the data record. Please make sure that the property is declared as ({2}) or that the column is returned as ({3}). Alternatively register a custom conversion function using the ValueConverter class.";
        public const string MissingKeyProperties = "The type ({0}) does not contain any properties that is considered a key property.";
        public const string UnmappedKeyProperty = "The property ({0}) is considered a key property, but is not available in the result set. Please make sure that the result set contains a field that can be mapped this property.";
    }
}