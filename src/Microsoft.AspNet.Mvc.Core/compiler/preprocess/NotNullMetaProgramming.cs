using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Framework.Runtime;
using Microsoft.Framework.Runtime.Roslyn;

namespace Microsoft.AspNet.Cryptography.KeyDerivation.Compiler.Preprocess
{
    public class NotNullMetaProgramming : ICompileModule
    {
        private readonly NotNullRewriter _rewriter = new NotNullRewriter();

        public NotNullMetaProgramming(IServiceProvider services)
        {
        }

        public void BeforeCompile(IBeforeCompileContext context)
        {
            // Replace each compilation tree (file) with the rewritten file
            var syntaxTrees = context.Compilation.SyntaxTrees;
            foreach (var item in syntaxTrees)
            {
                if (String.IsNullOrEmpty(item.FilePath))
                {
                    continue;
                }

                // Visit all nodes and apply [NotNull] logic
                var replaced = _rewriter.Visit(item.GetRoot());
                var trivia1 = SyntaxFactory.ParseLeadingTrivia(GeneratePragmaChecksum(item.FilePath));
                var trivia2 = SyntaxFactory.ParseLeadingTrivia(Environment.NewLine + "#line 1 \"" + item.FilePath + "\"" + Environment.NewLine);
                replaced = replaced.WithLeadingTrivia(trivia1.Concat(trivia2).Concat(replaced.GetLeadingTrivia()));
                var newTree = item.WithRootAndOptions(replaced, item.Options).WithFilePath("");
                context.Compilation = context.Compilation.ReplaceSyntaxTree(item, newTree);
            }
        }

        public void AfterCompile(IAfterCompileContext context)
        {
            // no-op
        }

        private static string GeneratePragmaChecksum(string path)
        {
            byte[] allBytes;
            using (var sha1 = SHA1.Create())
            {
                allBytes = sha1.ComputeHash(File.ReadAllBytes(path));
            }

            return String.Format(CultureInfo.InvariantCulture,
                @"#pragma checksum ""{0}"" ""{{ff1816ec-aa5e-4d10-87f7-6f4963833460}}"" ""{1}""" + Environment.NewLine + Environment.NewLine,
                path,
                String.Concat(allBytes.Select(b => b.ToString("X2", CultureInfo.InvariantCulture))));
        }
    }

    internal sealed class NotNullRewriter : CSharpSyntaxRewriter
    {
        private const string NOT_NULL_CHECK_FORMAT = @"if (@{0} == null) {{ throw new System.ArgumentNullException(nameof(@{0})); }}";

        public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            // Abstract, extern, and partial methods don't have bodies. Ignore them all.
            if (node.Body != null)
            {
                var updatedBody = GetUpdatedBodyBlock(node.ParameterList, node.Body);
                if (updatedBody != null)
                {
                    return node.WithBody(updatedBody);
                }
            }

            return node;
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            // Abstract, extern, and partial methods don't have bodies. Ignore them all.
            if (node.Body != null)
            {
                var updatedBody = GetUpdatedBodyBlock(node.ParameterList, node.Body);
                if (updatedBody != null)
                {
                    return node.WithBody(updatedBody);
                }
            }

            return node;
        }

        private static BlockSyntax GetUpdatedBodyBlock(ParameterListSyntax parameterList, BlockSyntax existingBody)
        {
            // Check all parameters for [NotNull] attribute declarations.
            StringBuilder sb = null;
            foreach (var parameter in parameterList.Parameters)
            {
                foreach (var attribute in parameter.AttributeLists.SelectMany(list => list.Attributes))
                {
                    if (attribute.Name.ToString() == "NotNullCheck")
                    {
                        // Append the null check to the string builder.
                        if (sb == null)
                        {
                            sb = new StringBuilder();
                            sb.Append('{');
                        }
                        sb.AppendFormat(CultureInfo.InvariantCulture, NOT_NULL_CHECK_FORMAT, parameter.Identifier.ToString());
                        sb.AppendLine();
                    }
                }
            }

            // No [NotNull] attribute found? Bail now.
            if (sb == null)
            {
                return null;
            }

            sb.Append('}');

            // Prepend the null check statements, then return the updated method body.
            var nullCheckStatementBlock = SyntaxFactory.ParseStatement(sb.ToString(), consumeFullText: true);
            var firstStatement = existingBody.Statements.FirstOrDefault();
            if (firstStatement != null)
            {
                var lineno = firstStatement.GetLocation().GetLineSpan().StartLinePosition.Line;
                nullCheckStatementBlock = nullCheckStatementBlock.WithTrailingTrivia(SyntaxFactory.ParseLeadingTrivia(Environment.NewLine + "#line " + lineno + " \"" + firstStatement.GetLocation().SourceTree.FilePath + "\"" + Environment.NewLine));
                nullCheckStatementBlock = nullCheckStatementBlock.WithLeadingTrivia(SyntaxFactory.ParseLeadingTrivia(Environment.NewLine + "#line hidden" + Environment.NewLine));
            }

            var newBody = existingBody.WithStatements(new SyntaxList<StatementSyntax>().Add(nullCheckStatementBlock).AddRange(existingBody.Statements));
            return newBody;
        }
    }
}