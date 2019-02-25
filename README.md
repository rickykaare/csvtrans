# csvtrans
_Tool for building translations from Google Sheet_

The idea behind the tool is to read translations from a Google Sheet and output them as string resources for iOS or Android. 

### Installation
Tool can be installed using the command

    dotnet tool install -g csvtrans
    
### Input
The tool supports the following input sources:

- A local CSV file
- An online CSV source
- A Google Sheet

The column headers in the input identifies the target languages (e.g. en, en-US, da-DK etc.) with the following special columns:

|Column Name |Purpose |
|------------|--------|
Key|Mandatory identifier for the string
Comment|Optional comment for the trasnslation
Default|Optional default value

An example sheet can be found here: [SampleTranslations](https://docs.google.com/spreadsheets/d/1SpSu13Gtk8aBsGK4b-iRK4wmmTtywx3twN1yABAVTOA/edit?usp=sharing)

### Output
Supported outputs are:

- Apple string resources (iOS)
- Android string resources
- Resx resources

### CLI interface

    USAGE: csvtrans [--help] [--sheet <document id> <sheet name>] [--csv <url or path>] --format <resx|ios|android>
                    --output <folder path>

    OPTIONS:

        --sheet <document id> <sheet name>
                              use the specified Google Sheet as input.
        --csv <url or path>   use a cvs file as input.
        --format <resx|ios|android>
                              specify the output translation format.
        --output <folder path>
                              specify the output folder
        --help                display this list of options.
