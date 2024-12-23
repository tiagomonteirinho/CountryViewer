using System.Data.SQLite;
using System.IO;
using System.Windows;

namespace CountryViewer.Services;

public class DataService
{
    private SQLiteConnection _connectionDataSource;
    private DialogService _dialogService;

    public DataService()
    {
        _dialogService = new DialogService();

        if (!Directory.Exists("data"))
        {
            Directory.CreateDirectory("data");
        }

        var path = @"data\Countries.sqlite";

        try
        {
            _connectionDataSource = new SQLiteConnection("Data Source =" + path);
            _connectionDataSource.Open();

            string _sqlCommand = "create table if not exists Countries(countryCommonName varchar(99), " +
                "countryOfficialName varchar(99), " +
                "countryCapitals varchar(99), " +
                "countryRegion varchar(99), " +
                "countrySubregion varchar(99), " +
                "countryPopulation int, " +
                "countryLatestGiniValue decimal(3,2), " +
                "countryLatestGiniYear varchar(6), " +
                "countryFlag varchar(99))";

            using (var command = new SQLiteCommand(_sqlCommand, _connectionDataSource))
            {
                command.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessage("Error during database initialization: " + ex.Message, "Error");
        }
        finally
        {
            _connectionDataSource.Close();
        }
    }

    public void SaveData(List<Country> countries)
    {
        if (countries == null || countries.Count == 0)
        {
            MessageBox.Show("Error saving data: No data found");
        }
        else
        {
            try
            {
                _connectionDataSource.Open();

                foreach (var country in countries)
                {
                    country.DefineGini();

                    string commonName = country.Name?.Common ?? "No information";
                    string officialName = country.Name?.Official ?? "No information";
                    string capitals = country.GetCapitals();
                    string region = country.GetRegion();
                    string subregion = country.GetSubregion();
                    int? population = ConvertPopulation(country.GetPopulation());
                    double? giniValue = country.Gini?.LatestGiniValue;
                    string giniYear = country.Gini?.LatestGiniYear;
                    string flag = country.GetFlag();

                    string _sqlCommand = "insert into Countries (countryCommonName, countryOfficialName, countryCapitals, countryRegion, countrySubregion, countryPopulation, countryLatestGiniValue, countryLatestGiniYear, countryFlag) " +
                                         "values (@commonName, @officialName, @capitals, @region, @subregion, @population, @giniValue, @giniYear, @flag)";

                    using (var command = new SQLiteCommand(_sqlCommand, _connectionDataSource)) // If methods return 'No information', save properties as 'DBNull' instead.
                    {
                        command.Parameters.AddWithValue("@commonName", commonName == "No information" ? DBNull.Value : commonName);
                        command.Parameters.AddWithValue("@officialName", officialName == "No information" ? DBNull.Value : officialName);
                        command.Parameters.AddWithValue("@capitals", capitals == "No information" ? DBNull.Value : capitals);
                        command.Parameters.AddWithValue("@region", region == "No information" ? DBNull.Value : region);
                        command.Parameters.AddWithValue("@subregion", subregion == "No information" ? DBNull.Value : subregion);
                        command.Parameters.AddWithValue("@population", population.HasValue ? population.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@population", population == null ? DBNull.Value : population.Value);
                        command.Parameters.AddWithValue("@giniValue", giniValue == null ? DBNull.Value : giniValue.Value);
                        command.Parameters.AddWithValue("@giniYear", giniYear == "No information" ? DBNull.Value : giniYear);
                        command.Parameters.AddWithValue("@flag", flag == null ? DBNull.Value : flag);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage("Error saving data: " + ex.Message, "Error");
            }
            finally
            {
                _connectionDataSource.Close();
            }
        }
    }
    private int? ConvertPopulation(string populationString) // Convert population string to 'int'.
    {
        if (int.TryParse(populationString, out int population))
        {
            return population;
        }

        return null;
    }

    public List<Country> GetData()
    {
        List<Country> countries = new List<Country>();

        try
        {
            _connectionDataSource.Open();

            string _sqlCommand = "select countryCommonName, " +
                "countryOfficialName, " +
                "countryCapitals, " +
                "countryRegion, " +
                "countrySubregion, " +
                "countryPopulation, " +
                "countryLatestGiniValue, " +
                "countryLatestGiniYear, " +
                "countryFlag from Countries";

            using (var command = new SQLiteCommand(_sqlCommand, _connectionDataSource))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        countries.Add(new Country()
                        {
                            Name = new Name()
                            {
                                Common = reader["countryCommonName"] != DBNull.Value ? (string)reader["countryCommonName"] : null,
                                Official = reader["countryOfficialName"] != DBNull.Value ? (string)reader["countryOfficialName"] : null,
                            },
                            Capital = reader["countryCapitals"] != DBNull.Value ? ((string)reader["countryCapitals"]).Split(',').Select(c => c.Trim()).ToList() : new List<string?>(),
                            Region = reader["countryRegion"] != DBNull.Value ? (string)reader["countryRegion"] : null,
                            Subregion = reader["countrySubregion"] != DBNull.Value ? (string)reader["countrySubregion"] : null,
                            Population = reader["countryPopulation"] != DBNull.Value ? (int)reader["countryPopulation"] : 0,
                            Gini = new Gini()
                            {
                                LatestGiniValue = reader["countryLatestGiniValue"] != DBNull.Value ? Convert.ToDouble(reader["countryLatestGiniValue"]) : null,
                                LatestGiniYear = reader["countryLatestGiniYear"] != DBNull.Value ? (string)reader["countryLatestGiniYear"] : null,
                            },
                            Flags = new Flags()
                            {
                                png = reader["countryFlag"] != DBNull.Value ? (string)reader["countryFlag"] : null,
                            }
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessage("Error retrieving data: " + ex.Message, "Error");
        }
        finally
        {
            _connectionDataSource.Close();
        }

        return countries;
    }

    public void DeleteData()
    {
        try
        {
            _connectionDataSource.Open();
            string _sqlCommand = "delete from Countries";
            using (var command = new SQLiteCommand(_sqlCommand, _connectionDataSource))
            {
                command.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessage("Error deleting data: " + ex.Message, "Error");
        }
        finally
        {
            _connectionDataSource.Close();
        }
    }
}
