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

    USAGE: csvtrans [--help] [--sheet <document id> <sheet name>]
                    [--csv <url or path>] [--format <apple|android|resx>]
                    [--outputdir <directory path>] [--name <string>]
                    [--convert-placeholders <regex pattern>]

    OPTIONS:

        --sheet, -s <document id> <sheet name>
                            specify a Google Sheet as input.
        --csv, -c <url or path>
                            specify a online or local cvs file as input.
        --format, -f <apple|android|resx>
                            specify the output format.
        --outputdir, -o <directory path>
                            specify the output directory.
        --name, -n <string>   specify an optional name for the output.
        --convert-placeholders, -p <regex pattern>
                            convert placeholders to match the output format.
        --help                display this list of options.