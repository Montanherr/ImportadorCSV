using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Globalization;

namespace WebContracts.Converters
{
    public class CustomDateTimeConverter : DateTimeConverter
    {
        private readonly string _format = "dd/MM/yyyy";

        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (DateTime.TryParseExact(text, _format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                return date;
            }

            return base.ConvertFromString(text, row, memberMapData);
        }
    }
}
