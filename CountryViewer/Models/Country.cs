using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace CountryViewer;

public class Country
{
    [JsonPropertyName("name")]
    public Name? Name { get; set; }

    [JsonPropertyName("capital")]
    public List<string?>? Capital { get; set; }

    [JsonPropertyName("region")]
    public string? Region { get; set; }

    [JsonPropertyName("subregion")]
    public string? Subregion { get; set; }

    [JsonPropertyName("population")]
    public int? Population { get; set; }

    [JsonPropertyName("gini")]
    public Gini? Gini { get; set; }

    [JsonPropertyName("flags")]
    public Flags? Flags { get; set; }

    public string GetCommonName()
    {
        return Name?.Common ?? "No information";
    }

    public string GetOfficialName()
    {
        return Name?.Official ?? "No information";
    }

    public string GetCapitals()
    {
        if (Capital != null && Capital.Any())
        {
            string capitals = "";
            foreach (var capital in Capital)
            {
                if (capital == Capital[0])
                {
                    capitals += capital;
                }
                else
                {
                    capitals += $", {capital}";
                }
            }
            return capitals;
        }
        else
        {
            return "No information";
        }
    }

    public string GetRegion()
    {
        return Region ?? "No information";
    }

    public string GetSubregion()
    {
        return Subregion ?? "No information";
    }

    public string GetPopulation()
    {
        return Population.HasValue ? Population.Value.ToString() : "No information";
    }

    public void DefineGini()
    {
        if (Gini != null)
        {
            Gini.LatestGiniValue = Gini.DefineLatestGiniValue();
            Gini.LatestGiniYear = Gini.DefineLatestGiniYear();
        }
    }

    public string GetGini()
    {
        if (Gini != null && Gini.LatestGiniValue != null && Gini.LatestGiniYear != null)
        {
            return $"{Gini.LatestGiniValue} {Gini.LatestGiniYear}";
        }
        else { return "No information"; }
    }

    public string GetFlag()
    {
        return Flags?.png ?? null;
    }
}

public class Name
{
    public string? Common { get; set; }
    public string? Official { get; set; }
}

public class Gini
{
    #region Api properties
    [JsonProperty("2019")]
    public double? _2019 { get; set; }

    [JsonProperty("2018")]
    public double? _2018 { get; set; }

    [JsonProperty("2017")]
    public double? _2017 { get; set; }

    [JsonProperty("2016")]
    public double? _2016 { get; set; }

    [JsonProperty("2015")]
    public double? _2015 { get; set; }

    [JsonProperty("2014")]
    public double? _2014 { get; set; }

    [JsonProperty("2013")]
    public double? _2013 { get; set; }

    [JsonProperty("2012")]
    public double? _2012 { get; set; }

    [JsonProperty("2011")]
    public double? _2011 { get; set; }

    [JsonProperty("2010")]
    public double? _2010 { get; set; }

    [JsonProperty("2009")]
    public double? _2009 { get; set; }
    #endregion

    public double? DefineLatestGiniValue()
    {
        if (_2019 != null)
        {
            return _2019;
        }
        else if (_2018 != null)
        {
            return _2018;
        }
        else if (_2017 != null)
        {
            return _2017;
        }
        else if (_2016 != null)
        {
            return _2016;
        }
        else if (_2015 != null)
        {
            return _2015;
        }
        else if (_2014 != null)
        {
            return _2014;
        }
        else if (_2013 != null)
        {
            return _2013;
        }
        else if (_2012 != null)
        {
            return _2012;
        }
        else if (_2011 != null)
        {
            return _2011;
        }
        else if (_2010 != null)
        {
            return _2010;
        }
        else if (_2009 != null)
        {
            return _2009;
        }
        else
        {
            return null;
        }
    }

    public string? DefineLatestGiniYear()
    {
        if (DefineLatestGiniValue() == _2019)
        {
            return "(2019)";
        }
        else if (DefineLatestGiniValue() == _2018)
        {
            return "(2018)";
        }
        else if (DefineLatestGiniValue() == _2017)
        {
            return "(2017)";
        }
        else if (DefineLatestGiniValue() == _2016)
        {
            return "(2016)";
        }
        else if (DefineLatestGiniValue() == _2015)
        {
            return "(2015)";
        }
        else if (DefineLatestGiniValue() == _2014)
        {
            return "(2014)";
        }
        else if (DefineLatestGiniValue() == _2013)
        {
            return "(2013)";
        }
        else if (DefineLatestGiniValue() == _2012)
        {
            return "(2012)";
        }
        else if (DefineLatestGiniValue() == _2011)
        {
            return "(2011)";
        }
        else if (DefineLatestGiniValue() == _2010)
        {
            return "(2010)";
        }
        else if (DefineLatestGiniValue() == _2009)
        {
            return "(2009)";
        }
        else
        {
            return null;
        }
    }

    public double? LatestGiniValue { get; set; }

    public string? LatestGiniYear { get; set; }
}

public class Flags
{
    public string? png { get; set; }
}
