namespace DbReader
{
    internal class ErrorMessages
    {
        public const string ConstructorNotFound = "The type ({0}) does not contain a public parameterless constructor.";
        public const string DuplicateFieldName = "The data record contains a duplicate field name ({0}). Make sure that every column/field name in the data record is unique.";
        public const string IncompatibleTypes = "The property ({0}) is not compatible with the column ({1})returned from the data record. Please make sure that the property is declared as ({2}) or that the column is returned as ({3}). Alternatively register a custom conversion function using the DbReaderOptions class.";
        public const string MissingKeyProperties = "The type ({0}) does not contain any properties that is considered a key property.";
        public const string UnmappedKeyProperty = "The property ({0}) is considered a key property, but is not available in the result set. Please make sure that the result set contains a field that can be mapped this property.";
        public const string MissingArgument = "Unable to resolve an argument value for parameter ({0}). Please make sure that the argument object passed into the Read method has a property named '{0}'";

        public const string DuplicateParameter =
                "The parameter {0} is already specified. If this parameter is added in the DbReaderOptions.CommandInitializer method there is no need to define the argument when executing the query."
            ;

        public const string InvalidCollectionType =
            "The navigation property (one-to-many) {0} must have one of the following types {1}";

        public const string SimpleProjectType =
            "{0} Simple types such as string and int are not allowed as the item type for a collection property.";
    }
}