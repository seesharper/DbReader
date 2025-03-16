using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace DbClient.Extensions
{
    /// <summary>
    /// Extends the <see cref="Expression"/> class.
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Flattens the <paramref name="expression"/> into an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="expression">The target <see cref="Expression"/>.</param>
        /// <returns>The <see cref="Expression"/> represented as a list of sub expressions.</returns>
        public static IEnumerable<Expression> AsEnumerable(this Expression expression)
        {
            var flattener = new ExpressionTreeFlattener();
            return flattener.Flatten(expression);
        }

        private class ExpressionTreeFlattener : ExpressionVisitor
        {
            private readonly ICollection<Expression> nodes = new Collection<Expression>();

            public IEnumerable<Expression> Flatten(Expression expression)
            {
                Visit(expression);
                return nodes;
            }

            public override Expression Visit(Expression node)
            {
                nodes.Add(node);
                return base.Visit(node);
            }
        }
    }
}