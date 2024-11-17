/// <summary>
/// Used to store information about a crop.
/// </summary>
public class Crop
{
    private string name;
    private string type;
    private string season;
    private int amount;
    private int numDaysToGrow;

    /// <summary>
    /// Constructor for Crop class
    /// </summary>
    /// <param name="n">The name of the crop.</param>
    /// <param name="t">The type of the crop.</param>
    /// <param name="s">The season the crop is grown in.</param>
    /// <param name="a">The amount of the crop.</param>
    /// <param name="d">The number of days to grow the crop.</param>
    public Crop(string n, string t, string s, int a, int d)
    {
        name = n;
        type = t;
        season = s;
        amount = a;
        numDaysToGrow = d;
    }

    // Getters
    public string GetName()
    {
        return name;
    }

    public string GetType()
    {
        return type;
    }

    public string GetSeason()
    {
        return season;
    }

    public int GetAmount()
    {
        return amount;
    }

    public int GetNumDaysToGrow()
    {
        return numDaysToGrow;
    }

    // Setters
    public void SetName(string n)
    {
        name = n;
    }

    public void SetType(string t)
    {
        type = t;
    }

    public void SetSeason(string s)
    {
        season = s;
    }

    public void SetAmount(int a)
    {
        amount = a;
    }

    public void SetNumDaysToGrow(int d)
    {
        numDaysToGrow = d;
    }
}
