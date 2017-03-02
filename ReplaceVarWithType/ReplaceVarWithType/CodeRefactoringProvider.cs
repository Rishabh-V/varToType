// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeRefactoringProvider.cs" company="Rishabh">
//   Rishabh 2016
// </copyright>
// <summary>
//   The replace var with type code refactoring provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ReplaceVarWithType
{
    using System.Collections.Generic;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeRefactorings;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Formatting;
    using Microsoft.CodeAnalysis.Options;
    using Microsoft.CodeAnalysis.Rename;

    /// <summary>
    /// The replace with type code refactoring provider.
    /// </summary>
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(ReplaceVarWithTypeCodeRefactoringProvider)), Shared]
    internal class ReplaceVarWithTypeCodeRefactoringProvider : CodeRefactoringProvider
    {
        /// <summary>
        /// The compute async.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public sealed override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            
            IEnumerable<LocalDeclarationStatementSyntax> nodes = root.DescendantNodes().OfType<LocalDeclarationStatementSyntax>();

            foreach (var node in nodes)
            {
                if (node.Declaration.Type.IsVar)
                {
                    continue;
                }

                VariableDeclaratorSyntax variable = node.Declaration.Variables.First();
                EqualsValueClauseSyntax initialiser = variable.Initializer;
                if (initialiser == null)
                {
                    continue;
                }

                // For any type declaration node, create a code action to reverse the identifier text.
                CodeAction action = CodeAction.Create("Replace var with Type", c => this.ReplaceVarWithTypeAsync(context.Document, node, c));

                // Register this code action.
                context.RegisterRefactoring(action);
            }
        }

        private async Task<Document> ReplaceVarWithTypeAsync(Document document, LocalDeclarationStatementSyntax varDeclaration, CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            // Get the symbol representing the type to be renamed.
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            ////var typeSymbol = semanticModel.GetDeclaredSymbol(varDeclaration, cancellationToken);
            SymbolInfo typeSymbol = semanticModel.GetSymbolInfo(varDeclaration.Declaration.Type);

            IdentifierNameSyntax varTypeName = SyntaxFactory.IdentifierName("var").WithAdditionalAnnotations(Formatter.Annotation);
            LocalDeclarationStatementSyntax newDeclaration = varDeclaration.ReplaceNode(varTypeName, varDeclaration.Declaration.Type);
            SyntaxNode newRoot = root.ReplaceNode(varDeclaration, newDeclaration);
            return document.WithSyntaxRoot(newRoot);
            ////// Produce a new solution that has all references to that type renamed, including the declaration.
            ////var originalSolution = document.Project.Solution;
            ////var optionSet = originalSolution.Workspace.Options;
            ////var newSolution = await Renamer.RenameSymbolAsync(document.Project.Solution, typeSymbol, "", optionSet, cancellationToken).ConfigureAwait(false);

            ////// Return the new solution with the now-uppercase type name.
            ////return newSolution;
        }

        /// <summary>
        /// The reverse type name async.
        /// </summary>
        /// <param name="document">
        /// The document.
        /// </param>
        /// <param name="typeDecl">
        /// The type decl.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<Solution> ReverseTypeNameAsync(Document document, TypeDeclarationSyntax typeDecl, CancellationToken cancellationToken)
        {
            // Produce a reversed version of the type declaration's identifier token.
            SyntaxToken identifierToken = typeDecl.Identifier;
            string newName = new string(identifierToken.Text.ToCharArray().Reverse().ToArray());

            // Get the symbol representing the type to be renamed.
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            INamedTypeSymbol typeSymbol = semanticModel.GetDeclaredSymbol(typeDecl, cancellationToken);

            // Produce a new solution that has all references to that type renamed, including the declaration.
            Solution originalSolution = document.Project.Solution;
            OptionSet optionSet = originalSolution.Workspace.Options;
            Solution newSolution = await Renamer.RenameSymbolAsync(document.Project.Solution, typeSymbol, newName, optionSet, cancellationToken).ConfigureAwait(false);

            // Return the new solution with the now-uppercase type name.
            return newSolution;
        }
    }
}