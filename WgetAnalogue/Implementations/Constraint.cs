using System;
using System.Collections.Generic;
using System.Linq;
using WgetAnalogue.Interfaces;

namespace WgetAnalogue.Implementations
{
    /// <summary>
    /// The constraint class.
    /// </summary>
    public class Constraint : IConstraint
    {
        private readonly LinkTransitionConstraintType _transitionType;
        private readonly IEnumerable<string> _availableSourceExtensions;

        /// <summary>
        /// The ctor.
        /// </summary>
        /// <param name="type"> The type of constraint by the transition link.</param>
        /// <param name="availableSourceExtensions"> The available extensions of source links.</param>
        /// <exception cref="ArgumentNullException"> Thrown, when <paramref name="availableSourceExtensions"/> is null.</exception>
        public Constraint(LinkTransitionConstraintType type, IEnumerable<string> availableSourceExtensions)
        {
            _transitionType = type;
            _availableSourceExtensions = availableSourceExtensions ?? throw new ArgumentNullException(nameof(availableSourceExtensions));
        }

        /// <summary>
        /// Determines whether the <paramref name="analyzedUri"/> is a permissible html link.
        /// </summary>
        /// <param name="analyzedUri"> The uri to be analyzed.</param>
        /// <param name="parentUri"> The parent uri of <paramref name="analyzedUri"/></param>
        /// <returns>True, if the <paramref name="analyzedUri"/> is valid by the constraint; otherwise, false. </returns>
        public bool IsHtmlLinkPermissible(Uri analyzedUri, Uri parentUri)
        {
            switch (_transitionType)
            {
                case LinkTransitionConstraintType.All:
                    return true;
                case LinkTransitionConstraintType.CurrentDomain:
                    return analyzedUri.Host == parentUri.Host;
                case LinkTransitionConstraintType.NotHigherThanInitialUrl:
                    return parentUri.IsBaseOf(analyzedUri);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether the <paramref name="uri"/> is a permissible source link.
        /// </summary>
        /// <param name="uri"> The uri to be analyzed.</param>
        /// <returns>True, if the <paramref name="uri"/> is valid by the constraint; otherwise, false. </returns>
        public bool IsSourceLinkPermissible(Uri uri)
        {
            var uriAsAString = uri.ToString();

            return _availableSourceExtensions.Any(ext => uriAsAString.EndsWith(ext));
        }
    }

    /// <summary>
    /// The type of constraint by link transition.
    /// </summary>
    public enum LinkTransitionConstraintType
    {
        All,
        CurrentDomain,
        NotHigherThanInitialUrl
    }
}