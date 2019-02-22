# csvtrans
_Tool for building translations from CSV input_

The idea behind the tool is to read translations from a google sheet and output translations formatted as string resources for iOS or Android. 

### CLI interface

    USAGE: csvtrans [--help] [--sheet <document id> <sheet name>] [--url <url>] [--file <path>] --format <ios|android>
                    --output <folder path>

    OPTIONS:

        --sheet <document id> <sheet name>
                              use the specified Google Sheet as input.
        --url <url>           use an online cvs file as input.
        --file <path>         use a local csv file as input.
        --format <ios|android>
                              specify the output translation format.
        --output <folder path>
                              specify the output folder
        --help                display this list of options.
