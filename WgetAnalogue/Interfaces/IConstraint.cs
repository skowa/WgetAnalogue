using System;

namespace WgetAnalogue.Interfaces
{
    /// <summary>
    /// Determines constrains.
    /// </summary>
    public interface IConstraint
    {
        /// <summary>
        /// Determines whether the <paramref name="analyzedUri"/> is a permissible html link.
        /// </summary>
        /// <param name="analyzedUri"> The uri to be analyzed.</param>
        /// <param name="parentUri"> The parent uri of <paramref name="analyzedUri"/></param>
        /// <returns>True, if the <paramref name="analyzedUri"/> is valid by the constraint; otherwise, false. </returns>
        bool IsHtmlLinkPermissible(Uri analyzedUri, Uri parentUri);

        /// <summary>
        /// Determines whether the <paramref name="uri"/> is a permissible source link.
        /// </summary>
        /// <param name="uri"> The uri to be analyzed.</param>
        /// <returns>True, if the <paramref name="uri"/> is valid by the constraint; otherwise, false. </returns>
        bool IsSourceLinkPermissible(Uri uri);
    }
}