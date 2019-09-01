using System;
using System.Globalization;

public class Example
{
    private static void ParsingFormat()
    {
        string[] values = { "1,304.16", "$1,456.78", "1,094", "152",
                          "123,45 €", "1 304,16", "Ae9f" }; //String values to parse into numbers readable as doubles
        double number;
        CultureInfo culture = null; //New object of CultureInfo

        foreach (string value in values)
        {
            try
            {
                culture = CultureInfo.CreateSpecificCulture("en-US"); //Sets to USD
                number = Double.Parse(value, culture);
                Console.WriteLine("{0}: {1} --> {2}", culture.Name, value, number);
            }
            catch (FormatException)
            {
                Console.WriteLine("{0}: Unable to parse '{1}'.",
                                  culture.Name, value);
                culture = CultureInfo.CreateSpecificCulture("fr-FR"); //Sets to FR if unable to Parse to USD
                try
                {
                    number = Double.Parse(value, culture);
                    Console.WriteLine("{0}: {1} --> {2}", culture.Name, value, number);
                }
                catch (FormatException)
                {
                    Console.WriteLine("{0}: Unable to parse '{1}'.",
                                      culture.Name, value);
                }
            }
            Console.WriteLine();
        }
    }
    // The example displays the following output:
    //    en-US: 1,304.16 --> 1304.16
    //    
    //    en-US: Unable to parse '$1,456.78'.
    //    fr-FR: Unable to parse '$1,456.78'.
    //    
    //    en-US: 1,094 --> 1094
    //    
    //    en-US: 152 --> 152
    //    
    //    en-US: Unable to parse '123,45 €'.
    //    fr-FR: Unable to parse '123,45 €'.
    //    
    //    en-US: Unable to parse '1 304,16'.
    //    fr-FR: 1 304,16 --> 1304.16
    //    
    //    en-US: Unable to parse 'Ae9f'.
    //    fr-FR: Unable to parse 'Ae9f'.

    public static void ParsingNumberStyles()
    {
        string value = "1,304";
        int number;
        IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-US");
        if (Int32.TryParse(value, out number))
            Console.WriteLine("{0} --> {1}", value, number);
        else
            Console.WriteLine("Unable to convert '{0}'", value);

        if (Int32.TryParse(value, NumberStyles.Integer | NumberStyles.AllowThousands,
                          provider, out number))
            Console.WriteLine("{0} --> {1}", value, number);
        else
            Console.WriteLine("Unable to convert '{0}'", value);
    }
    // The example displays the following output:
    //       Unable to convert '1,304'
    //       1,304 --> 1304
    public static void ParsingUnicode()
    {
        string value;
        // Define a string of basic Latin digits 1-5.
        value = "\u0031\u0032\u0033\u0034\u0035";
        ParseDigits(value);

        // Define a string of Fullwidth digits 1-5.
        value = "\uFF11\uFF12\uFF13\uFF14\uFF15";
        ParseDigits(value);

        // Define a string of Arabic-Indic digits 1-5.
        value = "\u0661\u0662\u0663\u0664\u0665";
        ParseDigits(value);

        // Define a string of Bangla digits 1-5.
        value = "\u09e7\u09e8\u09e9\u09ea\u09eb";
        ParseDigits(value);
    }

    static void ParseDigits(string value)
    {
        try
        {
            int number = Int32.Parse(value);
            Console.WriteLine("'{0}' --> {1}", value, number);
        }
        catch (FormatException)
        {
            Console.WriteLine("Unable to parse '{0}'.", value);
        }
    }
    // The example displays the following output:
    //       '12345' --> 12345
    //       Unable to parse '１２３４５'.
    //       Unable to parse '١٢٣٤٥'.
    //       Unable to parse '১২৩৪৫'.

    static void ParseString()
    {
        string phrase1 = "The quick brown fox jumps over the lazy dog.";
        string[] words1 = phrase1.Split(' '); //Array of words split by spaces

        foreach (var word in words1)
        {
            System.Console.WriteLine($"<{word}>");
        }

        string phrase2 = "The quick brown    fox     jumps over the lazy dog.";
        string[] words2 = phrase2.Split(' ');

        foreach (var word in words2)
        {
            System.Console.WriteLine($"<{word}>"); //Disregards length of spaces when using a Split
        }

        char[] delimiterChars1 = { ' ', ',', '.', ':', '\t' };

        string text = "one\ttwo three:four,five six seven";
        System.Console.WriteLine($"Original text: '{text}'"); 

        string[] words3 = text.Split(delimiterChars1); //Takes delimeter characters and uses them to split code into seperate words in an array
        System.Console.WriteLine($"{words3.Length} words in text:"); //Will count out seven (the # of words)

        foreach (var word in words3)
        {
            System.Console.WriteLine($"<{word}>");
        }

        char[] delimiterChars2 = { ' ', ',', '.', ':', '\t' };

        string text = "one\ttwo :,five six seven";
        System.Console.WriteLine($"Original text: '{text}'");

        string[] words4 = text.Split(delimiterChars2); //When recognized, it will split between any delimeter in string and will output empty strings if there is nothing in between
        System.Console.WriteLine($"{words4.Length} words in text:"); //Even if there are multiple delimeters in the text, it will output 7 strings (# of strings)

        foreach (var word in words4)
        {
            System.Console.WriteLine($"<{word}>");
        }

        string[] separatingStrings = { "<<", "..." }; //Used to get rid of multiple delimeter characters as a phrase

        string text = "one<<two......three<four";
        System.Console.WriteLine($"Original text: '{text}'");

        string[] words5 = text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries); //Removes empty strings after split completes from the delimeters
        System.Console.WriteLine($"{words5.Length} substrings in text:");

        foreach (var word in words5)
        {
            System.Console.WriteLine(word);
        }
    }
}


