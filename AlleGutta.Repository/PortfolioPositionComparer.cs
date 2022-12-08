using AlleGutta.Portfolios.Models;
using System.Diagnostics.CodeAnalysis;

namespace AlleGutta.Repository;

class PortfolioPositionComparer : IEqualityComparer<PortfolioPosition>
{
    public bool Equals(PortfolioPosition? x, PortfolioPosition? y)
    {
        //Check whether the compared objects reference the same data.
        if (ReferenceEquals(x, y)) return true;

        //Check whether any of the compared objects is null.
        if (x is null || y is null) return false;

        //Check whether the products' properties are equal.
        return x?.Symbol == y?.Symbol && x?.Name == y?.Name;
    }

    public int GetHashCode([DisallowNull] PortfolioPosition pos)
    {
        //Check whether the object is null
        if (pos is null) return 0;

        //Get hash code for the Name field if it is not null.
        int hashName = (pos.Name?.GetHashCode()) ?? 0;

        //Get hash code for the Symbol field.
        int hashSymbol = pos.Symbol?.GetHashCode() ?? 0;

        //Calculate the hash code for the product.
        return hashName ^ hashSymbol;
    }
}