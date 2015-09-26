namespace Wyam.Modules.CodeAnalysis
{
    // Note that if we ever introduce code analysis for other formats (such as Java or CSS), the metadata should be kept as similar as possible
    internal static class MetadataKeys
    {
        public const string SymbolId = "SymbolId"; // string, a unique ID that identifies this symbol
        public const string Symbol = "Symbol"; // ISymbol
        public const string Name = "Name"; // string, empty string if the symbol has no name (like the top-level namespace)
        public const string FullName = "FullName"; // string, namespaces = all containing namespaces, types = with generic type parameters
        public const string QualifiedName = "QualifiedName"; // string, the qualified name including containing namespaces
        public const string DisplayName = "DisplayName"; // string, namespace = QualifiedName, type = FullName
        public const string Kind = "Kind"; // string, the general kind of symbol (Namespace, NamedType, etc.)
        public const string SpecificKind = "SpecificKind"; // string, the more specific kind of the symbol (Class, Struct, etc.)
        public const string DocumentationCommentXml = "DocumentationCommentXml"; // string, the XML documentation comments (if any) or an empty string
        public const string Documentation = "Documentation"; // string, the documentation specific to this symbol (as gathered from documentation comment XML on this or a parent) or an empty string

        // Namespace
        public const string ContainingNamespace = "ContainingNamespace"; // IDocument, null if not nested
        public const string MemberTypes = "MemberTypes"; // IReadOnlyList<IDocument>, only contains direct children, not all descendants
        public const string MemberNamespaces = "MemberNamespaces"; // IReadOnlyList<IDocument>, empty if none

        // Type
        public const string ContainingType = "ContainingType"; // IDocument, null if not nested
        public const string BaseType = "BaseType"; // IDocument
        public const string AllInterfaces = "AllInterfaces"; // IEnumerable<IDocument>
        //                  ContainingNamespace
        //                  MemberTypes

        // Method
        //                  ContainingType
    }
}